using FluentValidation;
using Hubtel.PosProxy.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Models.Validators
{
    public class OrderRequestValidator : AbstractValidator<CreateOrderRequestDto>
    {
        public OrderRequestValidator()
        {
            RuleFor(vm => vm.PosDevice).NotNull().NotEmpty();
            RuleFor(vm => vm.IntegrationChannel).NotNull().NotEmpty();
            RuleFor(vm => vm.Items).NotNull().NotEmpty();

            RuleFor(vm => vm.Employee).SetValidator(new EmployeeValidator());
            RuleFor(vm => vm.Branch).SetValidator(new BranchValidator());
            RuleFor(vm => vm.Customer).SetValidator(new CustomerValidator());
            RuleForEach(vm => vm.Items).SetValidator(new ItemValidator());
        }
    }
}
