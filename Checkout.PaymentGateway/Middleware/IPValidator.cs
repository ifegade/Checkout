using System.Collections.Generic;
using System.Linq;
using System.Net;
using Checkout.PaymentGateway.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace Checkout.PaymentGateway.Middleware
{
    public class IPValidator : ActionFilterAttribute
    {
        private readonly HashSet<IPAddress> _merchantIpAddress;

        public IPValidator(List<IpDto> safeList)
        {
            _merchantIpAddress = safeList.SelectMany(s => s.Ips.Select(IPAddress.Parse)).ToHashSet();
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var remoteIp = context.HttpContext.Connection.RemoteIpAddress;
            
            Log.Information("Remote IpAddress: {@RemoteIp}", remoteIp);

            if (remoteIp != null && remoteIp.IsIPv4MappedToIPv6)
            {
                remoteIp = remoteIp.MapToIPv4();
            }

            if (!_merchantIpAddress.Contains(remoteIp))
            {
                Log.Warning("Forbidden Request from IP: {RemoteIp}", remoteIp);
                context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                return;
            }
            base.OnActionExecuting(context);
        }
    }
}