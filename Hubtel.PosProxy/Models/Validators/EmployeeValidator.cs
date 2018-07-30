using FluentValidation;
using Hubtel.PosProxy.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Models.Validators
{
    public class EmployeeValidator : AbstractValidator<EmployeeDto>
    {
        public EmployeeValidator()
        {
            RuleFor(x => x.EmployeeId).NotEmpty().When(x => !string.IsNullOrEmpty(x.PhoneNumber.Trim()));
            RuleFor(x => x.PhoneNumber).NotEmpty().When(x => !string.IsNullOrEmpty(x.EmployeeId.Trim()));
        }
    }
}
