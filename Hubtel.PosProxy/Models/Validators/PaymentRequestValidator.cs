using FluentValidation;
using Hubtel.PosProxy.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Models.Validators
{
    public class PaymentRequestValidator : AbstractValidator<CreatePaymentRequestDto>
    {
        private readonly IPaymentTypeConfiguration _paymentTypeConfiguration;

        public PaymentRequestValidator(IPaymentTypeConfiguration paymentTypeConfiguration)
        {
            _paymentTypeConfiguration = paymentTypeConfiguration;

            RuleFor(vm => vm.PaymentType).NotEmpty();
            RuleFor(vm => vm.PaymentType).Must(HaveValidPaymentType)
                .WithMessage("The payment type is not valid");
            RuleFor(vm => vm.Amount).NotNull().NotEmpty();
            RuleFor(vm => vm.PosDevice).NotNull().NotEmpty();
            RuleFor(vm => vm.SalesOrderId).NotNull().NotEmpty();
            RuleFor(vm => vm.CustomerPaysFee).NotNull().NotEmpty();
            RuleFor(vm => vm.Amount).GreaterThanOrEqualTo(0);
            RuleFor(vm => vm.MomoPhoneNumber).Must(HaveValidCustomerMsisdn).When(x => isMsisdnRequired(x.PaymentType));
            RuleFor(vm => vm.MomoPhoneNumber).Must(HaveValidChannel).When(x => isMsisdnRequired(x.PaymentType));
            RuleFor(vm => vm.MomoToken).NotEmpty().When(x => isChannelTokenRequired(x.MomoChannel));
            RuleFor(vm => vm.CustomerPaysFee).NotNull().NotEmpty().When(x => isMsisdnRequired(x.PaymentType));
            RuleFor(vm => vm.Employee).SetValidator(new EmployeeValidator());
            RuleFor(vm => vm.Branch).SetValidator(new BranchValidator());
            RuleFor(vm => vm.Customer).SetValidator(new CustomerValidator());
        }

        private bool HaveValidPaymentType(string paymentType)
        {
            var paymentTypeNames = _paymentTypeConfiguration.PaymentTypes.Select(x => x.Type.ToLower()).ToList();
            if (paymentTypeNames.Contains(paymentType.ToLower()))
            {
                return true;
            }
            return false;
        }

        private bool HaveValidCustomerMsisdn(string customerMsisdn)
        {
            //TODO: validate phone number
            if (string.IsNullOrEmpty(customerMsisdn))
                return false;

            return true;
        }

        private bool HaveValidChannel(string channel)
        {
            //TODO: validate channel
            if (string.IsNullOrEmpty(channel))
                return false;

            return true;
        }

        private bool isMsisdnRequired(string paymentTypeName)
        {
            var paymentType = _paymentTypeConfiguration.PaymentTypes
                .FirstOrDefault(x => x.Type.ToLower() == paymentTypeName.ToLower());

            if (paymentType != null && paymentType.RequireMsisdn.HasValue && paymentType.RequireMsisdn.Value)
            {
                return true;
            }
            return false;
        }

        private bool isChannelTokenRequired(string channelName)
        {
            if (string.IsNullOrEmpty(channelName))
            {
                return false;
            }
            var channel = _paymentTypeConfiguration.Channels
                .FirstOrDefault(x => x.Name.ToLower() == channelName.ToLower());

            if (channel != null && channel.Requiretoken.HasValue && channel.Requiretoken.Value)
            {
                return true;
            }
            return false;
        }
    }
}
