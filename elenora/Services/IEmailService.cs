using elenora.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Services
{
    public interface IEmailService
    {
        void SendEmail(string toEmail, string toName, string subject, string plainTextMessage, string htmlMessage, string category);
        void SendEmailToAdmins(string subject, string plainTextMessage, string htmlMessage, string category);
        EmailAddress GetOrCreateEmailAddress(string email, string source, bool subscribed);
        void SendEmailSequences();
        string GetEmailContentFromTemplate(EmailTemplate template, EmailAddress emailAddress);
        void Unsubscribe(string parameter);
        public string GetSalutation(string name);
    }
}
