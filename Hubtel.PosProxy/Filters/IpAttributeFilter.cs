using Hubtel.PosProxy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Filters
{
    public class IpAttributeFilter : ActionFilterAttribute
    {
        private readonly ILogger _logger;
        private readonly IMerchantAccountConfiguration _merchantAccountConfiguration;

        public IpAttributeFilter(ILogger<IpAttributeFilter> logger, IMerchantAccountConfiguration merchantAccountConfiguration)
        {
            _logger = logger;
            _merchantAccountConfiguration = merchantAccountConfiguration;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var remoteIpAddress = context.HttpContext.Connection.RemoteIpAddress.ToString();
            var ipAddressPool = _merchantAccountConfiguration.IpAddressPool.Split(';').Select(x => x.Trim()).ToList();
            if (!ipAddressPool.Contains(remoteIpAddress))
            {
                context.Result = new JsonResult(new { HttpStatusCode.Forbidden });
            }
            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            // do something after the action executes
        }
    }
}
