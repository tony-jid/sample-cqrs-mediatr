using MediatR;
using SampleCQRSMediatR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleCQRSMediatR.Commands
{
    public class EmployeeRemoveCommand : IRequest<Employee>
    {
        public Guid Id { get; set; }

        public EmployeeRemoveCommand(Guid id)
        {
            Id = id;
        }
    }
}
