using MediatR;
using SampleCQRSMediatR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleCQRSMediatR.Queries
{
    public class EmployeeGetByIdQuery : IRequest<Employee>
    {
        public Guid Id { get; set; }

        public EmployeeGetByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
