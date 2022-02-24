using elenora.Services;
using elenora.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.ViewComponents
{
    public class PopupViewComponent : ViewComponent
    {
        private readonly DataContext context;

        public PopupViewComponent(DataContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IViewComponentResult Invoke(int customerId)
        {
            var customer = context.Customers
                .Include(c => c.CustomerPopupStats)
                .First(c => c.Id == customerId);
            if (customer.CustomerPopupStats == null) return Content(string.Empty);

            if (customer.CustomerPopupStats.PopupId != null && customer.CustomerPopupStats.PopupDisplayedCount <= 0)
            {
                return View("Popup" + customer.CustomerPopupStats.PopupId);
            }
            return Content(string.Empty);
        }
    }
}
