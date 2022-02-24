using elenora.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Services
{
    public interface IActionLogService
    {
        void LogAction(int customerId, ActionLogInformation information, ActionEnum actionType, int? productId, string remark);
    }
}
