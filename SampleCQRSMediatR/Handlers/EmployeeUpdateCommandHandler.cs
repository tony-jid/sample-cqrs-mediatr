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
    public class EmployeeUpdateCommandHandler : IRequestHandler<EmployeeUpdateCommand, Employee>
    {
        public readonly IEmployeeRepo _employeeRepo;

        public EmployeeUpdateCommandHandler(IEmployeeRepo employeeRepo)
        {
            _employeeRepo = employeeRepo;
        }

        public async Task<Employee> Handle(EmployeeUpdateCommand request, CancellationToken cancellationToken)
        {
            var updatingEmployee = new Employee()
            {
                Id = request.Id,
                Name = request.Name,
                Department = request.Department,
                Salary = request.Salary
            };

            return await _employeeRepo.Update(updatingEmployee, cancellationToken);
        }
    }
}
