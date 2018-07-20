using FluentValidation;
using Hubtel.PosProxy.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Models.Validators
{
    public class BranchValidator : AbstractValidator<BranchDto>
    {
        public BranchValidator()
        {
            RuleFor(x => x.BranchId).NotEmpty().When(x => !string.IsNullOrEmpty(x.Name));
            RuleFor(x => x.Name).NotEmpty().When(x => !string.IsNullOrEmpty(x.BranchId));
        }
    }
}
