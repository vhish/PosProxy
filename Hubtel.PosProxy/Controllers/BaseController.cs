using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Controllers
{
    public class BaseController : ControllerBase
    {
        public async Task<IActionResult> ActionResultAsync(HttpResponseMessage httpResponseMessage)
        {
            switch (httpResponseMessage.StatusCode)
            {
                case HttpStatusCode.OK:
                {
                    return Ok(await httpResponseMessage.Content.ReadAsAsync<object>());
                }
                case HttpStatusCode.NotFound:
                {
                    return NotFound(await httpResponseMessage.Content.ReadAsAsync<object>());
                }
                case HttpStatusCode.Unauthorized:
                {
                    return Unauthorized();
                }
                case HttpStatusCode.Forbidden:
                {
                    return Forbid();
                }
                case HttpStatusCode.BadRequest:
                {
                    return BadRequest(await httpResponseMessage.Content.ReadAsAsync<object>());
                }
                default:
                {
                    return BadRequest();
                }
            }
        }
    }
}
