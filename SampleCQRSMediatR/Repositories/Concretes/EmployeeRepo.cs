using Microsoft.EntityFrameworkCore;
using SampleCQRSMediatR.Data.Contexts.Master;
using SampleCQRSMediatR.Models;
using SampleCQRSMediatR.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SampleCQRSMediatR.Repositories.Concretes
{
    public class EmployeeRepo : IEmployeeRepo
    {
        private DataMasterContext _context;

        public EmployeeRepo(DataMasterContext context)
        {
            _context = context;
        }

        public async Task<Employee> Add(Employee newEmployee, CancellationToken cancellationToken)
        {
            await _context.Employees.AddAsync(newEmployee, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return newEmployee;
        }

        public async Task<Employee> Remove(Guid id, CancellationToken cancellationToken)
        {
            var deleteingEmployee = await this.GetById(id, cancellationToken);
            if (deleteingEmployee == null) return null;

            _context.Employees.Remove(deleteingEmployee);
            await _context.SaveChangesAsync(cancellationToken);

            return deleteingEmployee;
        }

        public async Task<IEnumerable<Employee>> GetAll(CancellationToken cancellationToken)
        {
            return await _context.Employees.ToListAsync(cancellationToken);
        }

        public async Task<Employee> GetById(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Employees.FindAsync(new object[] { id }, cancellationToken: cancellationToken);
        }

        public async Task<Employee> Update(Employee updatingEmployee, CancellationToken cancellationToken)
        {
            if (await EmployeeExists(updatingEmployee.Id, cancellationToken))
            {
                _context.Entry(updatingEmployee).State = EntityState.Modified;
                await _context.SaveChangesAsync(cancellationToken);

                return updatingEmployee;
            }

            return null;
        }

        public async Task<Employee> UpdateV2(Employee updatingEmployee, CancellationToken cancellationToken)
        {
            _context.Entry(updatingEmployee).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);

            return updatingEmployee;
        }

        public async Task<bool> EmployeeExists(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Employees.AnyAsync(_ => _.Id == id, cancellationToken);
        }
    }
}
