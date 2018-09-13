using FluentValidation;
using Hubtel.PaymentProxy.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PaymentProxy.Models.Validators
{
    public class FeesRequestValidator : AbstractValidator<FeesRequestDto>
    {
        public FeesRequestValidator()
        {
            RuleFor(vm => vm.Amount).NotNull().NotEmpty().GreaterThan(0);
            RuleFor(vm => vm.ChargeCustomer).NotNull().NotEmpty();
            RuleFor(vm => vm.MomoPhoneNumber).NotNull().NotEmpty();
        }
    }
}
