using FluentValidation;
using SampleCQRSMediatR.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleCQRSMediatR.Validators
{
    public class EmployeeGetByIdQueryValidator : AbstractValidator<EmployeeGetByIdQuery>
    {
        // Constructor also supports Dependency Injection
        public EmployeeGetByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();
        }
    }
}
