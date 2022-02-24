using elenora.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace elenora.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration configuration;
        private readonly DataContext context;

        public EmailService(IConfiguration configuration, DataContext context)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void SendEmail(string toEmail, string toName, string subject, string plainTextMessage, string htmlMessage, string category)
        {
            if (!configuration.GetValue<bool>("Settings:EnableEmails")) return;

            var client = new SendGridClient("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");
            var message = new SendGridMessage
            {
                From = new SendGrid.Helpers.Mail.EmailAddress("info@elenora.hu", "Elenora"),
                ReplyTo = new SendGrid.Helpers.Mail.EmailAddress("info@elenora.hu", "Elenora"),
                Subject = subject,
                PlainTextContent = plainTextMessage,
                HtmlContent = htmlMessage,
                TrackingSettings = new TrackingSettings
                {
                    OpenTracking = new OpenTracking
                    {
                        Enable = true
                    }
                },
                Categories = new List<string> { category }
            };

            if (string.IsNullOrWhiteSpace(toName)) message.AddTo(toEmail);
            else message.AddTo(toEmail, toName);
            LogEmail(subject, toEmail, toName, plainTextMessage, htmlMessage, category);

            var result = client.SendEmailAsync(message).Result;
            if (result.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                var errorMessage = new SendGridMessage
                {
                    From = new SendGrid.Helpers.Mail.EmailAddress("info@elenora.hu", "Elenora"),
                    ReplyTo = new SendGrid.Helpers.Mail.EmailAddress("info@elenora.hu", "Elenora"),
                    Subject = "Email Hiba " + result.StatusCode,
                    PlainTextContent = result.Body.ReadAsStringAsync().Result
                };
                errorMessage.AddTo("boros.csaba94@gmail.com");
                client.SendEmailAsync(errorMessage);
            }
        }

        public Models.EmailAddress GetOrCreateEmailAddress(string email, string source, bool subscribed)
        {
            if (email == null) return null;
            var emailAddress = context.EmailAddresses.FirstOrDefault(e => e.Address == email.ToLower());
            if (emailAddress == null)
            {
                emailAddress = new Models.EmailAddress
                {
                    Address = email.ToLower(),
                    AddedDate = Helper.Now,
                    Source = source,
                    Unsubscribed = !subscribed
                };
                context.EmailAddresses.Add(emailAddress);
                context.SaveChanges();
            }
            return emailAddress;
        }

        private void LogEmail(string subject, string emailAddress, string name, string bodyText, string bodyHtml, string category)
        {
            if (emailAddress == "boros.csaba94@gmail.com" || emailAddress == "brigitta.boros96@gmail.com") return;
            var emailLog = new EmailLog
            {
                Subject = subject,
                EmailAddress = GetOrCreateEmailAddress(emailAddress, "email-log", false),
                Name = name,
                SentDate = Helper.Now,
                BodyText = bodyText,
                BodyHtml = bodyHtml,
                Category = category
            };
            context.EmailLogs.Add(emailLog);
            var logsToDelete = context.EmailLogs.Where(e => e.SentDate < Helper.Now.AddDays(-30)).ToList();
            context.EmailLogs.RemoveRange(logsToDelete);
            context.SaveChanges();
        }

        public void SendEmailSequences()
        {
            const int delaySeconds = 10 * 60;
            const int maxEmailsAtOnce = 5;

            int sentEmails = 0;
            var lastSentEmail = context.EmailHistories.Max(e => (DateTime?)e.SentDate);
            if (lastSentEmail == null || lastSentEmail.Value < Helper.Now.AddSeconds(-delaySeconds))
            {
                var random = new Random();
                var emailAddresses = context.EmailAddresses
                    .Where(e => !e.Unsubscribed)
                    .Include(e => e.EmailHistories)
                    .ToList().OrderBy(e => random.Next());
                var emailTemplates = context.EmailTemplates
                    .Where(t => t.Active &&
                                t.StartDate < Helper.Now && Helper.Now < t.EndDate).ToList();
                foreach (var emailAddress in emailAddresses)
                {
                    if (emailAddress.AddedDate > Helper.Now.AddHours(-1)) continue;
                    foreach (var emailTemplate in emailTemplates)
                    {
                        if (emailAddress.EmailHistories.Any(e => e.EmailTemplateId == emailTemplate.Id)) continue;
                        if (emailTemplate.OnlyAfterEmailId != null && !emailAddress.EmailHistories.Any(e => e.EmailTemplateId == emailTemplate.OnlyAfterEmailId)) continue;

                        SendEmailSequence(emailAddress, emailTemplate);
                        sentEmails++;
                        if (sentEmails > maxEmailsAtOnce)
                        {
                            return;
                        }
                    }
                }
            }
        }

        public string GetEmailContentFromTemplate(EmailTemplate template, Models.EmailAddress emailAddress)
        {
            var htmlContent = template.HtmlContent;

            if (htmlContent.Contains("[UNSUBSCRIBE]"))
            {
                var hash = GetEmailAddressHash(emailAddress);
                var unsubscribeLink = $@"https://www.elenora.hu/leiratkozas?p={emailAddress.Id}-{template.Id}-{hash}[UTM&]";
                htmlContent = htmlContent.Replace("[UNSUBSCRIBE]", unsubscribeLink);
            }

            var utmParameters = $"utm_medium=email&utm_source=email-sequence&utm_campaign={template.Category}";
            htmlContent = htmlContent.Replace("[UTM]", "?" + utmParameters);
            htmlContent = htmlContent.Replace("[UTM&]", "&" + utmParameters);

            return htmlContent;
        }

        private void SendEmailSequence(Models.EmailAddress address, EmailTemplate template)
        {
            var htmlContent = GetEmailContentFromTemplate(template, address);
            SendEmail(address.Address, null, template.Subject, template.TextContent, htmlContent, template.Category);
            var emailHistory = new EmailHistory
            {
                EmailAddress = address,
                EmailTemplate = template,
                SentDate = Helper.Now
            };
            context.EmailHistories.Add(emailHistory);
            context.SaveChanges();
        }

        public void Unsubscribe(string parameter)
        {
            var emailAddressId = int.Parse(parameter.Split("-").First());
            var templateId = int.Parse(parameter.Split("-")[1]);
            var hash = parameter.Replace($"{emailAddressId}-{templateId}-", "");
            var emailAddress = context.EmailAddresses.First(e => e.Id == emailAddressId);
            var expectedHash = GetEmailAddressHash(emailAddress);
            if (hash != expectedHash) throw new ArgumentException($"Invalid email hash for Id {emailAddressId}!");
            var history = context.EmailHistories.First(h => h.EmailAddressId == emailAddressId && h.EmailTemplateId == templateId);
            history.Unsubscribed = true;
            emailAddress.Unsubscribed = true;
            context.SaveChanges();
        }

        private string GetEmailAddressHash(Models.EmailAddress emailAddress)
        {
            using (var md5Hash = MD5.Create())
            {
                var sourceBytes = Encoding.UTF8.GetBytes(emailAddress.Address);
                var hashBytes = md5Hash.ComputeHash(sourceBytes);
                return BitConverter.ToString(hashBytes);
            }
        }

        public string GetSalutation(string name)
        {
            const string defaultSalutation = "Szia!";
            if (string.IsNullOrWhiteSpace(name)) return defaultSalutation;
            var nameParts = name.Split(" ").ToList();

            nameParts.RemoveAll(n => n.Contains("-") || n.EndsWith("ai"));

            var salutationName = string.Empty;
            if (nameParts.Count == 3)
            {
                if (nameParts[0].EndsWith("né")) salutationName = nameParts.Last();
                else if (nameParts.Any(n => n.EndsWith("né"))) salutationName = nameParts.First(n => n.EndsWith("né"));
                else salutationName = nameParts[1];
            }
            else if (nameParts.Count > 0) salutationName = nameParts.Last();

            if (!string.IsNullOrWhiteSpace(salutationName)) return $"Kedves {salutationName},";
            return defaultSalutation;
        }

        public void SendEmailToAdmins(string subject, string plainTextMessage, string htmlMessage, string category)
        {
            var emailRecipients = GetAdminRecipients();
            foreach (var emailRecipient in emailRecipients)
            {
                SendEmail(emailRecipient, "Admin", subject, plainTextMessage, htmlMessage, "admin-" + category);
            }
        }

        private string[] GetAdminRecipients()
        {
            return configuration.GetValue<string>("Settings:AdminEmails").Split(",");
        }
    }
}
