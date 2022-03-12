using MediatR;
using SampleCQRSMediatR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleCQRSMediatR.Commands
{
    public class EmployeeAddCommand : IRequest<Employee>
    {
        public string Name { get; set; }
        public string Department { get; set; }
        public int Salary { get; set; }
    }
}
