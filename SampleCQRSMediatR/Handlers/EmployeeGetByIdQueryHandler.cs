using MediatR;
using SampleCQRSMediatR.Models;
using SampleCQRSMediatR.Queries;
using SampleCQRSMediatR.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SampleCQRSMediatR.Handlers
{
    public class EmployeeGetByIdQueryHandler : IRequestHandler<EmployeeGetByIdQuery, Employee>
    {
        public readonly IEmployeeRepo _employeeRepo;

        public EmployeeGetByIdQueryHandler(IEmployeeRepo employeeRepo)
        {
            _employeeRepo = employeeRepo;
        }

        public async Task<Employee> Handle(EmployeeGetByIdQuery request, CancellationToken cancellationToken)
        {
            return await _employeeRepo.GetById(request.Id, cancellationToken);
        }
    }
}
