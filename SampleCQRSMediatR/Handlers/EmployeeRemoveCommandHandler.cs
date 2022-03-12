using MediatR;
using SampleCQRSMediatR.Commands;
using SampleCQRSMediatR.Models;
using SampleCQRSMediatR.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SampleCQRSMediatR.Handlers
{
    public class EmployeeRemoveCommandHandler : IRequestHandler<EmployeeRemoveCommand, Employee>
    {
        public readonly IEmployeeRepo _employeeRepo;

        public EmployeeRemoveCommandHandler(IEmployeeRepo employeeRepo)
        {
            _employeeRepo = employeeRepo;
        }

        public async Task<Employee> Handle(EmployeeRemoveCommand request, CancellationToken cancellationToken)
        {
            return await _employeeRepo.Remove(request.Id, cancellationToken);
        }
    }
}
