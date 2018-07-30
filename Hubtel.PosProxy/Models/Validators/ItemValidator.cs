using FluentValidation;
using Hubtel.PosProxy.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Models.Validators
{
    public class ItemValidator : AbstractValidator<ItemDto>
    {
        public ItemValidator()
        {
            RuleFor(vm => vm.Name).NotNull().NotEmpty();
            RuleFor(vm => vm.Quantity).NotNull().NotEmpty();
            RuleFor(vm => vm.UnitPrice).NotNull().NotEmpty();
        }
    }
}
