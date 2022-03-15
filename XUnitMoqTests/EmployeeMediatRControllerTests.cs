using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SampleCQRSMediatR.Commands;
using SampleCQRSMediatR.Controllers;
using SampleCQRSMediatR.Data.Contexts.Master;
using SampleCQRSMediatR.Handlers;
using SampleCQRSMediatR.Models;
using SampleCQRSMediatR.PipelineBehaviros;
using SampleCQRSMediatR.Queries;
using SampleCQRSMediatR.Repositories.Interfaces;
using SampleCQRSMediatR.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Xunit;

namespace XUnitMoqTests
{
    public class EmployeeMediatRControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly Mock<IEmployeeRepo> _mockEmployeeRepo;
        private readonly EmployeeMediatRController _employeeMediatRController;
        private readonly Employee _mockEmployeeData;
        private readonly IEnumerable<Employee> _mockEmployeeListData;

        public EmployeeMediatRControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _mockEmployeeRepo = new Mock<IEmployeeRepo>();
            _employeeMediatRController = new EmployeeMediatRController(_mockMediator.Object);
            _mockEmployeeData = DataSeeder.MockSingleEmployee();
            _mockEmployeeListData = DataSeeder.MockEmployees();
        }

        [Fact]
        public void GetAll_ShouldReturnExactNumbersOfEmployees()
        {
            // Arrange
            _mockMediator
                .Setup(mediator => mediator
                    .Send(It.IsAny<EmployeeGetAllQuery>(), It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(_mockEmployeeListData);

            // Act
            var result = _employeeMediatRController.GetAll(CancellationToken.None).Result;

            // Assert
            var okResult = Assert.IsAssignableFrom<OkObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);

            var employees = Assert.IsAssignableFrom<IEnumerable<Employee>>(okResult.Value);
            Assert.NotNull(employees);
            Assert.Equal(3, employees.Count());
        }

        [Fact]
        public void GetEmployee_ParamIdIsRandom_ShouldReturnNotFound()
        {
            // Arrange

            // Act
            var result = _employeeMediatRController.GetEmployee(Guid.NewGuid(), CancellationToken.None).Result;

            // Assert
            var notFoundResult = Assert.IsAssignableFrom<NotFoundResult>(result);
            Assert.Equal((int)HttpStatusCode.NotFound, notFoundResult.StatusCode);
        }

        [Fact]
        public void GetEmployee_ParamIdIsCorrect_ShouldReturnEmployee()
        {
            // Arrange

            // Mocking request handler
            _mockMediator
                .Setup(mediator => mediator
                    .Send(It.IsAny<EmployeeGetByIdQuery>(), It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(_mockEmployeeData);

            // Act
            var result = _employeeMediatRController.GetEmployee(DataSeeder.SingleEmployeeId, CancellationToken.None).Result;

            // Assert
            var okResult = Assert.IsAssignableFrom<OkObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);

            var employee = Assert.IsAssignableFrom<Employee>(okResult.Value);
            Assert.Equal(_mockEmployeeData.Id, employee.Id);
            Assert.Equal(_mockEmployeeData.Name, employee.Name);
            Assert.Equal(_mockEmployeeData.Department, employee.Department);
        }

        [Fact]
        public void PostEmployee_CommandIsNull_ShouldReturnBadRequest()
        {
            // Arrange

            // Act
            var result = _employeeMediatRController.PostEmployee(null, CancellationToken.None).Result;

            // Assert
            var badResult = Assert.IsAssignableFrom<BadRequestResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, badResult.StatusCode);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void PostEmployee_EmployeeNameIsNull_ShouldReturnBadRequest(string employeeName)
        {
            // Arrange
            var addCommand = new EmployeeAddCommand()
            {
                Name = employeeName,
                Department = _mockEmployeeData.Department,
                Salary = _mockEmployeeData.Salary
            };

            // Mock validator
            var validationBehavior = new ValidationBehavior<EmployeeAddCommand, Employee>(new List<EmployeeAddCommandValidator>()
            {
                new EmployeeAddCommandValidator()
            });

            // Act
            var exception = Record.ExceptionAsync(() =>
                validationBehavior.Handle(addCommand, CancellationToken.None, () =>
                {
                    return null;
                })
            ).Result;

            // Assert
            Assert.IsType<ValidationException>(exception);

            // This is the alternative to the act and the assert above
            //Assert.ThrowsAsync<ValidationException>(() =>
            //    validationBehavior.Handle(addCommand, CancellationToken.None, () =>
            //    {
            //        return null;
            //    })
            //);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void PostEmployee_DepartmentIsNull_ShouldReturnBadRequest(string department)
        {
            // Arrange
            var addCommand = new EmployeeAddCommand()
            {
                Name = _mockEmployeeData.Name,
                Department = department,
                Salary = _mockEmployeeData.Salary
            };

            // Mocking validator
            var validationBehavior = new ValidationBehavior<EmployeeAddCommand, Employee>(new List<EmployeeAddCommandValidator>()
            {
                new EmployeeAddCommandValidator()
            });

            // Act
            var exception = Record.ExceptionAsync(() =>
                validationBehavior.Handle(addCommand, CancellationToken.None, () =>
                {
                    return null;
                })
            ).Result;

            // Assert
            Assert.IsType<ValidationException>(exception);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void PostEmployee_SalaryIsNotValid_ShouldReturnBadRequest(int salary)
        {
            // Arrange
            var addCommand = new EmployeeAddCommand()
            {
                Name = _mockEmployeeData.Name,
                Department = _mockEmployeeData.Department,
                Salary = salary
            };

            // Mocking validator
            var validationBehavior = new ValidationBehavior<EmployeeAddCommand, Employee>(new List<EmployeeAddCommandValidator>()
            {
                new EmployeeAddCommandValidator()
            });

            // Act
            var exception = Record.ExceptionAsync(() =>
                validationBehavior.Handle(addCommand, CancellationToken.None, () =>
                {
                    return null;
                })
            ).Result;

            // Assert
            Assert.IsType<ValidationException>(exception);
        }

        [Fact]
        public void PostEmployee_EmployeeIsValid_ShouldReturnTheEmployee()
        {
            // Arrange
            var validationBehavior = new ValidationBehavior<EmployeeAddCommand, Employee>(new List<EmployeeAddCommandValidator>()
            {
                new EmployeeAddCommandValidator()
            });
            var addCommand = new EmployeeAddCommand()
            {
                Name = _mockEmployeeData.Name,
                Department = _mockEmployeeData.Department,
                Salary = _mockEmployeeData.Salary
            };

            // Mocking request handler
            _mockMediator
                .Setup(mediator => mediator
                    .Send(It.IsAny<EmployeeAddCommand>(), It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(_mockEmployeeData);

            // Act
            var exception = Record.ExceptionAsync(() =>
                validationBehavior.Handle(addCommand, CancellationToken.None, async () => 
                {
                    return _mockEmployeeData;
                })
            ).Result;
            var result = _employeeMediatRController.PostEmployee(addCommand, CancellationToken.None).Result;

            // Assert
            Assert.Null(exception);

            var createdAtActionResult = Assert.IsAssignableFrom<CreatedAtActionResult>(result);
            Assert.Equal((int)HttpStatusCode.Created, createdAtActionResult.StatusCode);

            var employee = Assert.IsAssignableFrom<Employee>(createdAtActionResult.Value);
            Assert.NotNull(employee);
            Assert.Equal(_mockEmployeeData.Id, employee.Id);
            Assert.Equal(_mockEmployeeData.Name, employee.Name);
            Assert.Equal(_mockEmployeeData.Department, employee.Department);
        }

        [Fact]
        public void PutEmployee_CommandIsNull_ShouldReturnBadRequest()
        {
            // Arrange

            // Act
            var result = _employeeMediatRController.PutEmployee(null, CancellationToken.None).Result;

            // Assert
            var badResult = Assert.IsAssignableFrom<BadRequestResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, badResult.StatusCode);
        }

        [Fact]
        public void PutEmployee_GuidIsNull_ShouldReturnBadRequest()
        {
            // Arrange
            var updateCommand = new EmployeeUpdateCommand()
            {
                Id = Guid.Empty,
                Name = _mockEmployeeData.Name,
                Department = _mockEmployeeData.Department,
                Salary = _mockEmployeeData.Salary
            };

            // Mocking request handler
            _mockEmployeeRepo
                .Setup(repo => repo.EmployeeExists(updateCommand.Id, CancellationToken.None))
                .ReturnsAsync(false);

            // Mocking validator
            var validationBehavior = new ValidationBehavior<EmployeeUpdateCommand, Employee>(new List<EmployeeUpdateCommandValidator>()
            {
                new EmployeeUpdateCommandValidator(_mockEmployeeRepo.Object)
            });

            // Act
            var exception = Record.ExceptionAsync(() =>
                validationBehavior.Handle(updateCommand, CancellationToken.None, () =>
                {
                    return null;
                })
            ).Result;

            // Assert
            Assert.IsType<ValidationException>(exception);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void PutEmployee_EmployeeNameIsNull_ShouldReturnBadRequest(string employeeName)
        {
            // Arrange
            var updatingEmployee = DataSeeder.MockSingleEmployee();
            var updateCommand = new EmployeeUpdateCommand()
            {
                Id = _mockEmployeeData.Id,
                Name = employeeName,
                Department = _mockEmployeeData.Department,
                Salary = _mockEmployeeData.Salary
            };

            // Mocking repo
            _mockEmployeeRepo
                .Setup(repo => repo.EmployeeExists(updateCommand.Id, CancellationToken.None))
                .ReturnsAsync(true);

            // Mocking validator
            var validationBehavior = new ValidationBehavior<EmployeeUpdateCommand, Employee>(new List<EmployeeUpdateCommandValidator>()
            {
                new EmployeeUpdateCommandValidator(_mockEmployeeRepo.Object)
            });

            // Act
            var exception = Record.ExceptionAsync(() =>
                validationBehavior.Handle(updateCommand, CancellationToken.None, () =>
                {
                    return null;
                })
            ).Result;

            // Assert
            Assert.IsType<ValidationException>(exception);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void PutEmployee_DepartmentIsNull_ShouldReturnBadRequest(string department)
        {
            // Arrange
            var updatingEmployee = DataSeeder.MockSingleEmployee();
            var updateCommand = new EmployeeUpdateCommand()
            {
                Id = _mockEmployeeData.Id,
                Name = _mockEmployeeData.Name,
                Department = department,
                Salary = _mockEmployeeData.Salary
            };

            // Mocking repo
            _mockEmployeeRepo
                .Setup(repo => repo.EmployeeExists(updateCommand.Id, CancellationToken.None))
                .ReturnsAsync(true);

            // Mocking validator
            var validationBehavior = new ValidationBehavior<EmployeeUpdateCommand, Employee>(new List<EmployeeUpdateCommandValidator>()
            {
                new EmployeeUpdateCommandValidator(_mockEmployeeRepo.Object)
            });

            // Act
            var exception = Record.ExceptionAsync(() =>
                validationBehavior.Handle(updateCommand, CancellationToken.None, () =>
                {
                    return null;
                })
            ).Result;

            // Assert
            Assert.IsType<ValidationException>(exception);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void PutEmployee_SalaryIsNotValid_ShouldReturnBadRequest(int salary)
        {
            // Arrange
            var updatingEmployee = DataSeeder.MockSingleEmployee();
            var updateCommand = new EmployeeUpdateCommand()
            {
                Id = _mockEmployeeData.Id,
                Name = _mockEmployeeData.Name,
                Department = _mockEmployeeData.Department,
                Salary = salary
            };

            // Mocking repo
            _mockEmployeeRepo
                .Setup(repo => repo.EmployeeExists(updateCommand.Id, CancellationToken.None))
                .ReturnsAsync(true);

            // Mocking validator
            var validationBehavior = new ValidationBehavior<EmployeeUpdateCommand, Employee>(new List<EmployeeUpdateCommandValidator>()
            {
                new EmployeeUpdateCommandValidator(_mockEmployeeRepo.Object)
            });

            // Act
            var exception = Record.ExceptionAsync(() =>
                validationBehavior.Handle(updateCommand, CancellationToken.None, () =>
                {
                    return null;
                })
            ).Result;

            // Assert
            Assert.IsType<ValidationException>(exception);
        }

        [Fact]
        public void PutEmployee_EmployeeIsValid_ShouldReturnTheEmployee()
        {
            // Arrange
            var updatingEmployee = new Employee()
            {
                Id = _mockEmployeeData.Id,
                Name = _mockEmployeeData.Name,
                Department = _mockEmployeeData.Department,
                Salary = _mockEmployeeData.Salary * 2
            };
            var updateCommand = new EmployeeUpdateCommand()
            {
                Id = updatingEmployee.Id,
                Name = updatingEmployee.Name,
                Department = updatingEmployee.Department,
                Salary = updatingEmployee.Salary
            };

            // Mocking request handler
            _mockMediator
                .Setup(mediator => mediator
                    .Send(It.IsAny<EmployeeUpdateCommand>(), It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(updatingEmployee);

            // Mocking repo
            _mockEmployeeRepo
                .Setup(repo => repo.EmployeeExists(updateCommand.Id, CancellationToken.None))
                .ReturnsAsync(true);

            // Mocking validator
            var validationBehavior = new ValidationBehavior<EmployeeUpdateCommand, Employee>(new List<EmployeeUpdateCommandValidator>()
            {
                new EmployeeUpdateCommandValidator(_mockEmployeeRepo.Object)
            });

            var exception = Record.ExceptionAsync(() =>
                validationBehavior.Handle(updateCommand, CancellationToken.None, async () =>
                {
                    return updatingEmployee;
                })
            ).Result;
            var result = _employeeMediatRController.PutEmployee(updateCommand, CancellationToken.None).Result;

            // Assert
            Assert.Null(exception);

            var okObjectResult = Assert.IsAssignableFrom<OkObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);

            var employee = Assert.IsAssignableFrom<Employee>(okObjectResult.Value);
            Assert.NotNull(employee);
            Assert.Equal(updatingEmployee.Id, employee.Id);
            Assert.Equal(updatingEmployee.Name, employee.Name);
            Assert.Equal(updatingEmployee.Department, employee.Department);
            Assert.Equal(updatingEmployee.Salary, employee.Salary);
        }

        [Fact]
        public void DeleteEmployee_GuidIsEmpty_ShouldReturnBadRequest()
        {
            // Arrange
            var removeCommand = new EmployeeRemoveCommand(Guid.Empty);

            // Mocking repo
            _mockEmployeeRepo
                .Setup(repo => repo.EmployeeExists(removeCommand.Id, CancellationToken.None))
                .ReturnsAsync(false);

            // Mocking validator
            var validationBehavior = new ValidationBehavior<EmployeeRemoveCommand, Employee>(new List<EmployeeRemoveCommandValidator>()
            {
                new EmployeeRemoveCommandValidator(_mockEmployeeRepo.Object)
            });

            // Act
            var exception = Record.ExceptionAsync(() =>
                validationBehavior.Handle(removeCommand, CancellationToken.None, () =>
                {
                    return null;
                })
            ).Result;

            var result = _employeeMediatRController.DeleteEmployee(removeCommand.Id, CancellationToken.None).Result;

            // Assert
            Assert.IsType<ValidationException>(exception);

            var notFoundResult = Assert.IsAssignableFrom<NotFoundResult>(result);
            Assert.Equal((int)HttpStatusCode.NotFound, notFoundResult.StatusCode);
        }

        [Fact]
        public void DeleteEmployee_EmployeeIsNotFound_ShouldReturnBadRequest()
        {
            // Arrange
            var removeCommand = new EmployeeRemoveCommand(_mockEmployeeData.Id);

            // Mocking repo
            _mockEmployeeRepo
                .Setup(repo => repo.EmployeeExists(removeCommand.Id, CancellationToken.None))
                .ReturnsAsync(false);

            // Mocking validator
            var validationBehavior = new ValidationBehavior<EmployeeRemoveCommand, Employee>(new List<EmployeeRemoveCommandValidator>()
            {
                new EmployeeRemoveCommandValidator(_mockEmployeeRepo.Object)
            });

            // Act
            var exception = Record.ExceptionAsync(() =>
                validationBehavior.Handle(removeCommand, CancellationToken.None, async () =>
                {
                    return null;
                })
            ).Result;

            var result = _employeeMediatRController.DeleteEmployee(Guid.NewGuid(), CancellationToken.None).Result;

            // Assert
            Assert.IsType<ValidationException>(exception);

            var notFoundResult = Assert.IsAssignableFrom<NotFoundResult>(result);
            Assert.Equal((int)HttpStatusCode.NotFound, notFoundResult.StatusCode);
        }

        [Fact]
        public void DeleteEmployee_GuidIsValid_ShouldReturnTheEmployee()
        {
            // Arrange
            var removeCommand = new EmployeeRemoveCommand(_mockEmployeeData.Id);

            // Mocking request handler
            _mockMediator
                .Setup(mediator => mediator
                    .Send(It.IsAny<EmployeeRemoveCommand>(), It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(_mockEmployeeData);

            // Mocking repo
            _mockEmployeeRepo
                .Setup(repo => repo.EmployeeExists(removeCommand.Id, CancellationToken.None))
                .ReturnsAsync(true);

            // Mocking validator
            var validationBehavior = new ValidationBehavior<EmployeeRemoveCommand, Employee>(new List<EmployeeRemoveCommandValidator>()
            {
                new EmployeeRemoveCommandValidator(_mockEmployeeRepo.Object)
            });

            // Act
            var exception = Record.ExceptionAsync(() =>
                validationBehavior.Handle(removeCommand, CancellationToken.None, async () =>
                {
                    return _mockEmployeeData;
                })
            ).Result;

            var result = _employeeMediatRController.DeleteEmployee(removeCommand.Id, CancellationToken.None).Result;

            // Assert
            Assert.Null(exception);

            var okResult = Assert.IsAssignableFrom<OkObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);

            var removedEmployee = Assert.IsAssignableFrom<Employee>(okResult.Value);
            Assert.Equal(removedEmployee.Id, _mockEmployeeData.Id);
            Assert.Equal(removedEmployee.Name, _mockEmployeeData.Name);
            Assert.Equal(removedEmployee.Department, _mockEmployeeData.Department);
            Assert.Equal(removedEmployee.Salary, _mockEmployeeData.Salary);
        }
    }
}
