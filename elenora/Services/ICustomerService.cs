using elenora.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Services
{
    public interface ICustomerService
    {
        public Customer GetOrCreateCustomer(string customerCookieId, string referrer = null);
        public Customer GetCustomerById(int customerId);
        public void AcceptPopupOffer(int customerId, string emailAddress);
        public void ShowPopup(int customerId, string sourcePage);
        public void DeleteOldInactiveCustomers();
        public void SendAskForReviewEmails();
    }
}
