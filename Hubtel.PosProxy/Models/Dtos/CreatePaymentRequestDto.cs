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
        public string MomoPhoneNumber { get; set; }
        public string MomoChannel { get; set; }
        public string MomoToken { get; set; }
        public bool? CustomerPaysFee { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string Note { get; set; }
        public string PosDevice { get; set; }

        public CustomerDto Customer { get; set; }
        public EmployeeDto Employee { get; set; }
        public BranchDto Branch { get; set; }
    }

    [Validator(typeof(CustomerValidator))]
    public class CustomerDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

    [Validator(typeof(BranchValidator))]
    public class BranchDto
    {
        public string BranchId { get; set; }
        public string Name { get; set; }
    }

    [Validator(typeof(EmployeeValidator))]
    public class EmployeeDto
    {
        public string EmployeeId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
    }
}
