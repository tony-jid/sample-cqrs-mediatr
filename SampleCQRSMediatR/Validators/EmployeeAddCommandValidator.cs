using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using SampleCQRSMediatR.Commands;

namespace SampleCQRSMediatR.Validators
{
    public class EmployeeAddCommandValidator : AbstractValidator<EmployeeAddCommand>
    {
        // Constructor also supports Dependency Injection
        public EmployeeAddCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.Department)
                .NotEmpty();

            RuleFor(x => x.Salary)
                .NotEmpty()
                .GreaterThan(0);
        }
    }
}
