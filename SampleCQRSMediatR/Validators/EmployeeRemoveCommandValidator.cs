using FluentValidation;
using SampleCQRSMediatR.Commands;
using SampleCQRSMediatR.Models;
using SampleCQRSMediatR.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SampleCQRSMediatR.Validators
{
    public class EmployeeRemoveCommandValidator : AbstractValidator<EmployeeRemoveCommand>
    {
        public EmployeeRemoveCommandValidator(IEmployeeRepo employeeRepo)
        {
            // The cancellationToken instance will be injected throughout instances of the request ValidateAsync is called in the ValidationBehavior
            RuleFor(x => x.Id)
                .NotEmpty()
                .MustAsync(async (id, cancellationToken) => await employeeRepo.EmployeeExists(id, cancellationToken))
                    .WithMessage(x => $"{nameof(Employee)}#{x.Id} is not found");
        }
    }
}
