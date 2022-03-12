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
    public class EmployeeAddCommandHandler : IRequestHandler<EmployeeAddCommand, Employee>
    {
        public readonly IEmployeeRepo _employeeRepo;

        public EmployeeAddCommandHandler(IEmployeeRepo employeeRepo)
        {
            _employeeRepo = employeeRepo;
        }

        public async Task<Employee> Handle(EmployeeAddCommand request, CancellationToken cancellationToken)
        {
            var newEmployee = new Employee()
            {
                Name = request.Name,
                Department = request.Department,
                Salary = request.Salary
            };

            return await _employeeRepo.Add(newEmployee, cancellationToken);
        }
    }
}
