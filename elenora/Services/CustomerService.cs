using elenora.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace elenora.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly DataContext context;
        private readonly IEmailService emailService;

        public CustomerService(DataContext context, IEmailService emailService)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public Customer GetOrCreateCustomer(string customerCookieId, string referrer = null)
        {
            var customer = context.Customers
                .Include(c => c.EmailAddress)
                .FirstOrDefault(c => c.CookieId == customerCookieId.ToLower());
            if (customer != null) return customer;
            customer = new Customer
            {
                CookieId = customerCookieId.ToLower(),
                CreatedDate = Helper.Now,
                Referrer = referrer
            };

            var showPopup = new Random().Next(100) < 50;
            if (showPopup)
            {
                var popups = context.Popups.ToList();
                var totalOdds = popups.Sum(p => p.Odds);
                Popup popup = null;
                if (totalOdds > 0)
                {
                    var random = new Random().Next(1, totalOdds + 1);
                    var i = 0;
                    var currentOds = 1;
                    while (i < popups.Count || popup == null)
                    {
                        if (currentOds <= random && random < currentOds + popups[i].Odds)
                        {
                            popup = popups[i];
                        }
                        currentOds += popups[i].Odds;
                        i++;
                    }
                }

                customer.CustomerPopupStats = new CustomerPopupStats
                {
                    Popup = popups.OrderBy(p => new Random().Next()).FirstOrDefault()
                };
            }

            context.Customers.Add(customer);
            context.SaveChanges();
            return customer;
        }

        public Customer GetCustomerById(int customerId)
        {
            return context.Customers.FirstOrDefault(c => c.Id == customerId);
        }

        public void AcceptPopupOffer(int customerId, string emailAddress)
        {
            var customer = context.Customers
                .Include(c => c.EmailAddress)
                .Include(c => c.CustomerPopupStats)
                .First(c => c.Id == customerId);
            var popupType = (PopupEnum)customer.CustomerPopupStats.PopupId;
            customer.EmailAddress = emailService.GetOrCreateEmailAddress(emailAddress, "popup-" + customer.CustomerPopupStats.PopupId, true);
            var coupon = GetCouponForPopup(popupType);
            customer.CustomerPopupStats.PopupActionExecutedCount++;
            context.SaveChanges();

            var plainTextMessage = GetCouponEmailContent(customer, coupon, popupType);
            var htmlMessage = Helper.GetEmailTemplate().Replace("[EMAIL-CONTENT]", GetCouponEmailContentHtml(customer, coupon, popupType));

            var subject = string.Empty;
            switch (popupType)
            {
                case PopupEnum.Popup1With10Percent:
                    subject = "10% kupon";
                    break;
                case PopupEnum.Popup2With2000Ft:
                    subject = "2 000 Ft kedvezmény";
                    break;
            }
            emailService.SendEmail(emailAddress, null, subject, plainTextMessage, htmlMessage, $"popup-{(int)popupType}");
        }

        public void ShowPopup(int customerId, string sourcePage)
        {
            var customer = context.Customers
                .Include(c => c.CustomerPopupStats)
                .First(c => c.Id == customerId);
            if (customer.CustomerPopupStats == null) customer.CustomerPopupStats = new CustomerPopupStats();
            customer.CustomerPopupStats.PopupDisplayedCount++;
            customer.CustomerPopupStats.PopupDisplayRemark += sourcePage + ", ";
            customer.CustomerPopupStats.PopupLastDisplayed = Helper.Now;
            context.SaveChanges();
        }

        public void DeleteOldInactiveCustomers()
        {
            var oldCustomers = context.Customers
                .Include(c => c.Orders)
                .Where(c => c.CreatedDate < Helper.Now.AddDays(-60) &&
                            c.Orders.Count == 0).ToList();
            context.RemoveRange(oldCustomers);
            context.SaveChanges();
        }

        public void SendAskForReviewEmails()
        {
            var order = context.Orders
                .Include(o => o.Customer)
                .Include(o => o.BillingAddress)
                .Include(o => o.EmailAddress)
                .Where(o => o.OrderedDate < DateTime.Now.AddDays(-60) &&
                            o.Status == OrderStatusEnum.OrderCompleted &&
                            o.Customer.ReviewEmailSent == null)
                .OrderBy(o => o.CreatedDate).FirstOrDefault();
            if (order == null) return;
            var orderWithThisEmail = context.Orders.Include(o => o.Customer).FirstOrDefault(o => o.EmailAddressId == order.EmailAddressId && o.Customer.ReviewEmailSent != null);
            if (orderWithThisEmail != null)
            {
                order.Customer.ReviewEmailSent = orderWithThisEmail.Customer.ReviewEmailSent;
            }
            else
            {
                emailService.SendEmail(order.EmailAddress.Address, order.BillingAddress.Name, "Érdeklődés a leadott rendeléseddel kapcsolatban", GetReviewEmailContent(), GetReviewEmailContentHtml(), "review");
                order.Customer.ReviewEmailSent = Helper.Now;
            }
            context.SaveChanges();
        }

        private Coupon GetCouponForPopup(PopupEnum popupType)
        {
            Coupon coupon = null;
            switch (popupType)
            {
                case PopupEnum.Popup1With10Percent:
                    coupon = context.Coupons.First(c => c.Code == "E10P1K");
                    break;
                case PopupEnum.Popup2With2000Ft:
                    coupon = context.Coupons.First(c => c.Code == "E2EP2K");
                    break;
            }
            return coupon;
        }

        private string GetCouponEmailContent(Customer customer, Coupon coupon, PopupEnum popupType)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Szia!");
            switch (popupType)
            {
                case PopupEnum.Popup1With10Percent:
                    sb.AppendLine("Vásárolj 1 órán belül és 10% kedvezmény adunk a vásárlásod összegéből.");
                    break;
                case PopupEnum.Popup2With2000Ft:
                    sb.AppendLine("Vásárolj 1 órán belül és 2000Ft kedvezmény adunk, melyet 10000Ft feletti vásárlás esetén tudsz beváltani.");
                    break;
            }
            sb.AppendLine($"A kupon beváltásához használd a {coupon.Code} kódot vagy kattints az alábbi linkre:");
            sb.AppendLine("https://www.elenora.hu/kosar/folytatas/" + customer.CookieId + "/" + coupon.Code + "?utm_medium=email&utm_source=popup&utm_campaign=popup-" + (int)popupType);
            return sb.ToString();
        }

        private string GetCouponEmailContentHtml(Customer customer, Coupon coupon, PopupEnum popupType)
        {
            var sb = new StringBuilder();
            sb.AppendLine(Helper.GetEmailTitleRow("Szia!"));
            switch (popupType)
            {
                case PopupEnum.Popup1With10Percent:
                    sb.AppendLine(Helper.GetEmailTextRow("Vásárolj 1 órán belül és 10% kedvezmény adunk a vásárlásod összegéből."));
                    break;
                case PopupEnum.Popup2With2000Ft:
                    sb.AppendLine(Helper.GetEmailTextRow("Vásárolj 1 órán belül és 2000Ft kedvezmény adunk, melyet 10000Ft feletti vásárlás esetén tudsz beváltani."));
                    break;
            }
            sb.AppendLine(Helper.GetEmailTextRow($"A kupon beváltásához használd a {coupon.Code} kódot vagy kattints az alábbi linkre:"));
            sb.Append(Helper.GetEmailTextRow($"Kupon kód: <b>{coupon.Code}</b>"));
            sb.Append(Helper.GetEmailTextRow("<br />"));
            sb.Append(Helper.GetEmailButton("KÉREM A KUPONT", "https://www.elenora.hu/kosar/folytatas/" + customer.CookieId + "/" + coupon.Code + "?utm_medium=email&utm_source=popup&utm_campaign=popup-" + (int)popupType));
            sb.Append(Helper.GetEmailTextRow("<br />"));
            return sb.ToString();
        }

        private string GetReviewEmailContent()
        {
            return @"Szia!

            A www.elenora.hu weboldalon leadott rendeléseddel kapcsolatban szeretnénk érdeklődni. 
            Az a célunk, hogy minden vásárlónk maximálisan elégedett legyen a karkötőinkkel, és hogy nyugodt szívvel ajánljanak másoknak is minket. De ez csak a ti visszajelzésetekkel lehetséges!

            Arra szeretnélek kérni, hogy írd le nekünk, hogy mennyire voltál megelégedve a kapott termékkel, a szállítással. Mennyire volt bonyolult a weboldalt használni, esetleg felmerűlt benned olyan kérdés, amire a weboldalon nem találtál választ? Olyan volt a termék, amire számítottál, meg voltál elégedve a szállítással és a csomagolással? Illetve, szerinted mivel tehetnénk jobbá a szolgáltatásunkat, mi volt az ami esetleg hiányzott?
            Minden appróságra kíváncsiak vagyunk.

            Ha válaszolt erre az emailre, akkor megajándékozunk egy 2000Ft értékű kuponnal, ami 10000Ft értékű vásárlás esetén váltható használható.

            Nagyon szépen köszönöm!
            Üdvözlettel,
            Boros Brigitta,
            www.elenora.hu";
        }

        private string GetReviewEmailContentHtml()
        {
            return @"Szia!<br />
            <br />
            A www.elenora.hu weboldalon leadott rendeléseddel kapcsolatban szeretnénk érdeklődni.<br />
            Az a célunk, hogy minden vásárlónk maximálisan elégedett legyen a karkötőinkkel, és hogy nyugodt szívvel ajánljanak másoknak is minket. De ez csak a ti visszajelzésetekkel lehetséges!<br />
            <br />
            Arra szeretnélek kérni, hogy írd le nekünk, hogy mennyire voltál megelégedve a kapott termékkel, a szállítással. Mennyire volt bonyolult a weboldalt használni, esetleg felmerűlt benned olyan kérdés, amire a weboldalon nem találtál választ? Olyan volt a termék, amire számítottál, meg voltál elégedve a szállítással és a csomagolással? Illetve, szerinted mivel tehetnénk jobbá a szolgáltatásunkat, mi volt az ami esetleg hiányzott?<br />
            Minden appróságra kíváncsiak vagyunk.<br />
            <br />
            Ha válaszolt erre az emailre, akkor megajándékozunk egy 2000Ft értékű kuponnal, ami 10000Ft értékű vásárlás esetén váltható használható.<br />
            <br />
            Nagyon szépen köszönöm!<br />
            Üdvözlettel,<br />
            Boros Brigitta<br />
            www.elenora.hu";
        }
    }
}
