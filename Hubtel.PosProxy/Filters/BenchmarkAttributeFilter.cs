using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Filters
{
    public class BenchmarkAttributeFilter : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            await next();

            stopWatch.Stop();
            context.HttpContext.Response.Headers.Add(
                "x-time-elapsed",
                stopWatch.Elapsed.ToString());
        }
    }
}
