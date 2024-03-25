using elenora.Models;
using System;

namespace elenora.Services
{
    public class ActionLogService: IActionLogService
    {
        private readonly DataContext context;

        public ActionLogService(DataContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void LogAction(int customerId, ActionLogInformation information, ActionEnum actionType, int? productId, string remark)
        {
            var actionLog = new ActionLog
            {
                CustomerId = customerId,
                IpAddress = information?.IpAddress,
                Action = actionType,
                ProductId = productId,
                Remark = remark,
                Date = Helper.Now,
                Referrer = information?.Referrer,
                DeviceType = information?.DeviceType
            };
            context.ActionLogs.Add(actionLog);
            context.SaveChanges();
        }
    }
}
