using Hubtel.PosProxy.Extensions;
using Hubtel.PosProxy.Helpers;
using Hubtel.PosProxy.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Controllers
{
    [Authorize]
    [Route("api/sync-orders")]
    [ApiController]
    public class SyncOrdersController : BaseController
    {
        private readonly ISalesOrderZipFileService _salesOrderZipFileService;

        public SyncOrdersController(ISalesOrderZipFileService salesOrderZipFileService)
        {
            _salesOrderZipFileService = salesOrderZipFileService;
        }

        [HttpGet, Route("upload")]
        public async Task<IActionResult> GetSalesOrders()
        {
            var fileName = "C:\\Projects\\PosProxy\\Hubtel.PosProxy\\salesorders.7z";
            var outFile = "C:\\Projects\\PosProxy\\Hubtel.PosProxy\\salesorders1.json";
            var salesOrders = _salesOrderZipFileService.Extract(fileName, outFile);
            return Ok(salesOrders);
        }

        [Consumes("application/json", "application/json-patch+json", "multipart/form-data")]
        [AddSwaggerFileUploadButton]
        [HttpPost, Route("upload")]
        public async Task<IActionResult> uploadOrders([FromForm]IFormFile file)
        {
            var user = HttpContext.User;
            var accountId = UserHelper.GetAccountId(user);

            //save to cloud storage before processing
            var mimeType = file.ContentType;
            var extension = Path.GetExtension(file.FileName).Replace(".", string.Empty).ToLower();
            var path = $"{Path.GetFileNameWithoutExtension(file.FileName).ToFilePath(accountId)}.{extension}";
            var salesOrderZipFile = await _salesOrderZipFileService.SaveFileAsync(file, path, mimeType);

            return Ok();
        }
    }
}
