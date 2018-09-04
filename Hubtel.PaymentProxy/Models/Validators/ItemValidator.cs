using FluentValidation;
using Hubtel.PaymentProxy.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PaymentProxy.Models.Validators
{
    public class ItemValidator : AbstractValidator<OrderItemDto>
    {
        public ItemValidator()
        {
            RuleFor(vm => vm.Name).NotNull().NotEmpty();
            RuleFor(vm => vm.Quantity).NotNull().NotEmpty();
            RuleFor(vm => vm.UnitPrice).NotNull().NotEmpty();
        }
    }
}
