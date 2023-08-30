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
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class IPValidator : ActionFilterAttribute
    {
        private readonly List<string> _merchantIpAddress;

        public IPValidator(List<IpDto> safeList)
        {
            _merchantIpAddress = safeList.SelectMany(s => s.Ips).ToList(); //Split(";").ToList();
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var remoteIp = context.HttpContext.Connection.RemoteIpAddress;
            
            
            Log.Information("Remote IpAddress: {@RemoteIp}", remoteIp);
            
            var badIp = true;

            if (remoteIp != null && remoteIp.IsIPv4MappedToIPv6)
            {
                remoteIp = remoteIp.MapToIPv4();
            }
            
            foreach (var address in _merchantIpAddress)
            {
                if (IPAddress.Parse(address).Equals(remoteIp))
                {
                    badIp = false;
                    break;
                }
            }

            if (badIp)
            {
                Log.Warning("Forbidden Request from IP: {RemoteIp}", remoteIp);
                context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}