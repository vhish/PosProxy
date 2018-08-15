using FluentValidation;
using Hubtel.PosProxy.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Models.Validators
{
    public class OrderRequestValidator : AbstractValidator<OrderRequestDto>
    {
        public OrderRequestValidator()
        {
            RuleFor(vm => vm.PosDeviceId).NotNull().NotEmpty();
            RuleFor(vm => vm.IntegrationChannel).NotNull().NotEmpty();
            RuleFor(vm => vm.OrderItems).NotNull().NotEmpty();
            RuleFor(x => x.EmployeeId).NotEmpty().When(x => !string.IsNullOrEmpty(x.EmployeeName.Trim()));
            RuleFor(x => x.EmployeeName).NotEmpty().When(x => !string.IsNullOrEmpty(x.EmployeeId.Trim()));
            RuleFor(x => x.BranchId).NotEmpty().When(x => !string.IsNullOrEmpty(x.BranchName));
            RuleFor(x => x.BranchName).NotEmpty().When(x => !string.IsNullOrEmpty(x.BranchId));
            RuleForEach(vm => vm.OrderItems).SetValidator(new ItemValidator());
        }
    }
}
