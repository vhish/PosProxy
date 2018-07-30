using System;
using Hubtel.PosProxy.Models.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Hubtel.PosProxy.Models.Validators
{
    public class CreateTransactionRequestValidator : AbstractValidator<CreateTransactionRequestDto>
    {
        public CreateTransactionRequestValidator()
        {
            RuleFor(vm => vm.Payment).NotNull().NotEmpty();
            RuleFor(vm => vm.Payment.SalesOrderId).NotNull().NotEmpty().When(x => x.Order == null);
        }
    }
}
    