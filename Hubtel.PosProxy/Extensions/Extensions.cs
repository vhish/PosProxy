using Hubtel.PosProxy.Models.Responses;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Extensions
{
    public static class Extensions
    {
        public static List<HubtelPosProxyError> ToErrors(this ModelStateDictionary modelState)
        {
            var errors = new List<HubtelPosProxyError>();

            foreach (var key in modelState.Keys)
            {
                errors.Add(new HubtelPosProxyError
                {
                    Field = key,
                    Messages = modelState[key].Errors.Select(x => x.ErrorMessage).ToList()
                });
            }

            return errors;
        }

        public static List<HubtelPosProxyError> ToErrors(this UnifiedSalesErrorResponse modelState)
        {
            var errors = new List<HubtelPosProxyError>();

            if(modelState.Errors != null)
            {
                foreach (var key in modelState.Errors)
                {
                    errors.Add(new HubtelPosProxyError
                    {
                        Field = key.Field,
                        Messages = new List<string> { key.DeveloperMessage }
                    });
                }
            }
            
            return errors;
        }

        public static List<HubtelPosProxyError> ToErrors(this MerchantAccountErrorResponse modelState)
        {
            var errors = new List<HubtelPosProxyError>();

            if(modelState.Errors != null)
            {
                foreach (var key in modelState.Errors)
                {
                    errors.Add(new HubtelPosProxyError
                    {
                        Field = key.Field,
                        Messages = key.Messages
                    });
                }
            }
            
            return errors;
        }
    }
}
