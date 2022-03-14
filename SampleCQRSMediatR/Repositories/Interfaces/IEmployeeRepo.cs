using SampleCQRSMediatR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SampleCQRSMediatR.Repositories.Interfaces
{
    public interface IEmployeeRepo
    {
        Task<IEnumerable<Employee>> GetAll(CancellationToken cancellationToken);
        Task<Employee> GetById(Guid id, CancellationToken cancellationToken);
        Task<Employee> Add(Employee newEmployee, CancellationToken cancellationToken);
        Task<Employee> Update(Employee updatingEmployee, CancellationToken cancellationToken);
        Task<Employee> UpdateV2(Employee updatingEmployee, CancellationToken cancellationToken);
        Task<Employee> Remove(Guid id, CancellationToken cancellationToken);
        Task<bool> EmployeeExists(Guid id, CancellationToken cancellationToken);
    }
}
