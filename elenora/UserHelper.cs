using elenora.Models;
using elenora.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora
{
    public static class UserHelper
    {

        public static ActionLogInformation GetActionLogInformation(HttpRequest request)
        {
            var result = new ActionLogInformation
            {
                Referrer = GetReferrer(request),
                IpAddress = request.HttpContext.Connection.RemoteIpAddress?.ToString()
            };
            try
            {
                var userAgent = request.Headers["User-Agent"];
                var deviceDetector = DeviceDetectorNET.DeviceDetector.GetInfoFromUserAgent(userAgent)?.Match;
                if (deviceDetector != null)
                {
                    result.IsBot = deviceDetector.Bot != null;
                    result.DeviceType = string.Format("{0} - {1} - {2}",
                        deviceDetector.DeviceType ?? string.Empty,
                        deviceDetector.OsFamily ?? string.Empty,
                        deviceDetector.DeviceBrand ?? string.Empty);
                }
            }
            catch { };
            return result;
        }

        public static string GetReferrer(HttpRequest request)
        {
            if (request.Query.ContainsKey("ref"))
            {
                return request.Query["ref"].ToString();
            }
            return null;
        }
    }
}
