using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SampleCQRSMediatR.Data.Contexts.Master;
using SampleCQRSMediatR.Models;
using SampleCQRSMediatR.Repositories.Interfaces;

namespace SampleCQRSMediatR.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepo _employeeRepo;

        public EmployeeController(IEmployeeRepo employeeRepo)
        {
            _employeeRepo = employeeRepo;
        }

        // GET: api/Employee
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            return Ok(await _employeeRepo.GetAll(cancellationToken));
        }

        // GET: api/Employee/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee(Guid id, CancellationToken cancellationToken)
        {
            var employee = await _employeeRepo.GetById(id, cancellationToken);

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        // PUT: api/Employee/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(Guid id, Employee employee, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty) return BadRequest();
            if (string.IsNullOrWhiteSpace(employee.Name)) return BadRequest();

            employee.Id = id;
            var updatedEmployee = await _employeeRepo.Update(employee, cancellationToken);

            if (updatedEmployee == null) return BadRequest();

            return Ok(updatedEmployee);
        }

        // POST: api/Employee
        [HttpPost]
        public async Task<IActionResult> PostEmployee(Employee employee, CancellationToken cancellationToken)
        {
            if (employee == null) return BadRequest();
            if (string.IsNullOrWhiteSpace(employee.Name)) return BadRequest();

            await _employeeRepo.Add(employee, cancellationToken);

            return CreatedAtAction("GetEmployee", new { id = employee.Id }, employee);
        }

        // DELETE: api/Employee/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty) return BadRequest();

            var employee = await _employeeRepo.GetById(id, cancellationToken);
            if (employee == null)
            {
                return NotFound();
            }

            await _employeeRepo.Remove(id, cancellationToken);

            return Ok(employee);
        }
    }
}
