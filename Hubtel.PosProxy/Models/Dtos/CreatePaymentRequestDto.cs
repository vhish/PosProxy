using FluentValidation.Attributes;
using Hubtel.PosProxy.Models.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Models.Dtos
{
    [Validator(typeof(PaymentRequestValidator))]
    public class CreatePaymentRequestDto
    {
        public string PaymentType { get; set; }
        public decimal Amount { get; set; }
        public string SalesOrderId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerMsisdn { get; set; }
        public string CustomerEmail { get; set; }
        public string Channel { get; set; }
        public string ChannelToken { get; set; }
        public EmployeeDto Employee { get; set; }
        public BranchDto Branch { get; set; }
    }

    public class BranchDto
    {
        public string BranchId { get; set; }
        public string Name { get; set; }
    }

    public class EmployeeDto
    {
        public string EmployeeId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
    }
}
