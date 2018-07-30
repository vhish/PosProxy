using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Constants
{
    public class ResponseCodes
    {
        public const string PAYMENT_REQUEST_SUCCESSFUL = "0001";

        public const string PAYMENT_SUCCESSFUL = "0000";
    
        // Success Codes
        public const int SUCCESS = 2000;

        // Validation Error Codes
        public const int VALIDATION_ERRORS = 4000;
        public const int REQUIRED_FIELD = 4001;

        // Unauthorized Codes
        public const int UNAUTHORIZED = 4010;
        public const int FORBIDDEN = 4030;

        // Not found codes
        public const int NOT_FOUND = 4040;

        // Other Error Codes
        public const int SERVER_ERROR = 5000;
        public const int GENERAL_ERROR = 5001;

        // External API error
        public const int EXTERNAL_ERROR = 4050;

    }
}
