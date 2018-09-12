using FluentValidation;
using Hubtel.PaymentProxy.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PaymentProxy.Models.Validators
{
    public class PaymentRequestValidator : AbstractValidator<PaymentRequestDto>
    {
        private readonly IPaymentTypeConfiguration _paymentTypeConfiguration;

        public PaymentRequestValidator(IPaymentTypeConfiguration paymentTypeConfiguration)
        {
            _paymentTypeConfiguration = paymentTypeConfiguration;

            RuleFor(vm => vm.Description).NotNull().NotEmpty();
            RuleFor(vm => vm.PaymentType).NotNull().NotEmpty();
            RuleFor(vm => vm.PaymentType).Must(HaveValidPaymentType)
                .WithMessage("The payment type is not valid");
            RuleFor(vm => vm.AmountPaid).NotNull().NotEmpty();
            RuleFor(vm => vm.PosDeviceId).NotNull().NotEmpty();
            RuleFor(vm => vm.AmountPaid).GreaterThanOrEqualTo(0);
            RuleFor(vm => vm.MomoPhoneNumber).Must(HaveValidCustomerMsisdn).When(x => isMsisdnRequired(x.PaymentType));
            RuleFor(vm => vm.MomoPhoneNumber).Must(HaveValidChannel).When(x => isMsisdnRequired(x.PaymentType));
            RuleFor(vm => vm.MomoToken).NotEmpty().When(x => isChannelTokenRequired(x.MomoChannel));
            RuleFor(vm => vm.ChargeCustomer).NotNull().NotEmpty().When(x => isMsisdnRequired(x.PaymentType));
            RuleFor(x => x.EmployeeId).NotEmpty().When(x => !string.IsNullOrEmpty(x.EmployeeName.Trim()));
            RuleFor(x => x.EmployeeName).NotEmpty().When(x => !string.IsNullOrEmpty(x.EmployeeId.Trim()));
            RuleFor(x => x.BranchId).NotEmpty().When(x => !string.IsNullOrEmpty(x.BranchName));
            RuleFor(x => x.BranchName).NotEmpty().When(x => !string.IsNullOrEmpty(x.BranchId));
        }

        private bool HaveValidPaymentType(string paymentType)
        {
            if (string.IsNullOrEmpty(paymentType))
                return false;

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
            if (string.IsNullOrEmpty(paymentTypeName))
                return false;

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
