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
    public class EmployeeGetAllQueryHandler : IRequestHandler<EmployeeGetAllQuery, IEnumerable<Employee>>
    {
        public readonly IEmployeeRepo _employeeRepo;

        public EmployeeGetAllQueryHandler(IEmployeeRepo employeeRepo)
        {
            _employeeRepo = employeeRepo;
        }

        public async Task<IEnumerable<Employee>> Handle(EmployeeGetAllQuery request, CancellationToken cancellationToken)
        {
            return await _employeeRepo.GetAll(cancellationToken);
        }
    }
}
