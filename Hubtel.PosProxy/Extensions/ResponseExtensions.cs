using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Extensions
{
    public static class ResponseExtensions
    {
        public static void AddApplicationError(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-Error", message);
            // CORS
            response.Headers.Add("access-control-expose-headers", "Application-Error");
        }

    }

    public static class Responses
    {
        public static HubtelPosProxyResponse<T> SuccessResponse<T>(string message, T data, int code)
        {
            return new HubtelPosProxyResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Code = code,
                Errors = null
            };
        }

        public static HubtelPosProxyResponse<T> ErrorResponse<T>(List<HubtelPosProxyError> errors, string error = "", int code = 0)
        {
            return new HubtelPosProxyResponse<T>
            {
                Success = false,
                Message = error,
                Data = default(T),
                Code = code,
                Errors = errors
            };
        }

        public static HubtelPosProxyResponse<T> ErrorResponse<T>(List<HubtelPosProxyError> errors, T data, string error = "", int code = 0)
        {
            return new HubtelPosProxyResponse<T>
            {
                Data = data,
                Success = false,
                Message = error,
                Code = code,
                Errors = errors
            };
        }
    }

    public class HubtelPosProxyResponse<T>
    {
        public T Data { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public int Code { get; set; } = 0;
        public List<HubtelPosProxyError> Errors { get; set; }
    }

    public class StatusMessage
    {
        public static readonly string Created = "Successfully Created";
        public static readonly string Deleted = "Successfully Deleted";
        public static readonly string Updated = "Successfully Updated";
        public static readonly string Found = "Successfully Found";
        public static readonly string Empty = "Resultset Empty";
        public static readonly string NotFound = "Not Found";
        public static readonly string BadRequest = "Bad Request";
        public static readonly string ValidationErrors = "Validation Errors";
    }
    
    public class HubtelPosProxyError
    {
        public string Field { get; set; }
        public List<string> Messages { get; set; }
    }
}
