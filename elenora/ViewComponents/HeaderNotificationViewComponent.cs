using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.ViewComponents
{
    public class HeaderNotificationViewComponent : ViewComponent
    {
        private readonly DataContext context;
        private const string COOKIE_KEY = "ClosedNotifications";

        public HeaderNotificationViewComponent(DataContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IViewComponentResult Invoke()
        {
            var closedNotifications = new List<int>();
            var closedNotificationsString = Request.Cookies[COOKIE_KEY];
            if (!string.IsNullOrWhiteSpace(closedNotificationsString))
            {
                closedNotifications = closedNotificationsString.Split(",").Select(x => int.Parse(x)).ToList();
            }

            var notifications = context.Notifications
                .Where(n => n.IsActive &&
                            n.StartDate < Helper.Now && Helper.Now < n.EndDate)
                .Select(n => new Tuple<int, string>(n.Id, n.Text)).ToList()
                .Where(n => !closedNotifications.Contains(n.Item1)).ToList();

            return View(notifications);
        }
    }
}
