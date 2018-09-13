using Hubtel.PaymentProxy.Models.ApiResponses;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PaymentProxy.Extensions
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

        public static List<HubtelPosProxyError> ToErrors(this UnifiedSalesValidationErrorResponse modelState)
        {
            var errors = new List<HubtelPosProxyError>();

            if(modelState.Data != null)
            {
                foreach (var key in modelState.Data)
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

        public static List<HubtelPosProxyError> ToErrors(this UnifiedSalesErrorResponse modelState)
        {
            var errors = new List<HubtelPosProxyError>();

            if (modelState?.Data != null)
            {
                errors.Add(new HubtelPosProxyError
                {
                    Field = "",
                    Messages = new List<string> { modelState.Data as string }
                });
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
