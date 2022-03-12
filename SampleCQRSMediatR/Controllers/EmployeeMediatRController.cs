using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SampleCQRSMediatR.Commands;
using SampleCQRSMediatR.Models;
using SampleCQRSMediatR.Queries;
using SampleCQRSMediatR.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SampleCQRSMediatR.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EmployeeMediatRController : ControllerBase
    {
        private readonly IEmployeeRepo _employeeRepo;
        private readonly IMediator _mediator;

        public EmployeeMediatRController(IEmployeeRepo employeeRepo, IMediator mediator)
        {
            _employeeRepo = employeeRepo;
            _mediator = mediator;
        }

        // GET: api/Employee
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var query = new EmployeeGetAllQuery();
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        // GET: api/Employee/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee(Guid id, CancellationToken cancellationToken)
        {
            var query = new EmployeeGetByIdQuery(id);

            var result = await _mediator.Send(query, cancellationToken);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // POST: api/Employee
        [HttpPost]
        public async Task<IActionResult> PostEmployee([FromBody] EmployeeAddCommand command, CancellationToken cancellationToken)
        {
            if (command == null) return BadRequest();
            if (string.IsNullOrWhiteSpace(command.Name)) return BadRequest();

            var result = await _mediator.Send(command, cancellationToken);

            return CreatedAtAction(nameof(GetEmployee), new { id = result.Id }, result);
        }

        // PUT: api/Employee/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(Guid id, [FromBody] EmployeeUpdateCommand command, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty) return BadRequest();
            if (string.IsNullOrWhiteSpace(command.Name)) return BadRequest();

            command.Id = id;
            var updatedEmployee = await _mediator.Send(command, cancellationToken);

            return updatedEmployee != null ? (IActionResult) Ok(updatedEmployee) : BadRequest();
        }

        // DELETE: api/Employee/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty) return BadRequest();

            var command = new EmployeeRemoveCommand(id);

            var deletedEmployee = await _mediator.Send(command, cancellationToken);
            return deletedEmployee != null ? (IActionResult) Ok(deletedEmployee) : NotFound();
        }
    }
}
