using Microsoft.AspNetCore.Mvc;
using Moq;
using SampleCQRSMediatR.Controllers;
using SampleCQRSMediatR.Data.Contexts.Master;
using SampleCQRSMediatR.Models;
using SampleCQRSMediatR.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Xunit;

namespace XUnitMoqTests
{
    public class EmployeeControllerTests
    {
        private readonly Mock<IEmployeeRepo> _mockEmpRepo;
        private readonly EmployeeController _employeeController;

        public EmployeeControllerTests()
        {
            _mockEmpRepo = new Mock<IEmployeeRepo>();
            _employeeController = new EmployeeController(_mockEmpRepo.Object);
        }

        [Fact]
        public void GetAll_ShouldReturnExactNumbersOfEmployees()
        {
            // Arrange
            _mockEmpRepo.Setup(repo => repo.GetAll(CancellationToken.None))
                .ReturnsAsync(DataSeeder.MockEmployees);

            // Act
            var result = _employeeController.GetAll(CancellationToken.None).Result;

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
            var result = _employeeController.GetEmployee(Guid.NewGuid(), CancellationToken.None).Result;

            // Assert
            var notFoundResult = Assert.IsAssignableFrom<NotFoundResult>(result);
            Assert.Equal((int)HttpStatusCode.NotFound, notFoundResult.StatusCode);
        }

        [Fact]
        public void GetEmployee_ParamIdIsCorrect_ShouldReturnEmployee()
        {
            // Arrange
            var theEmployee = DataSeeder.MockSingleEmployee();
            _mockEmpRepo.Setup(repo => repo.GetById(DataSeeder.SingleEmployeeId, CancellationToken.None))
                .ReturnsAsync(theEmployee);

            // Act
            var result = _employeeController.GetEmployee(DataSeeder.SingleEmployeeId, CancellationToken.None).Result;

            // Assert
            var okResult = Assert.IsAssignableFrom<OkObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);

            var employee = Assert.IsAssignableFrom<Employee>(okResult.Value);
            Assert.Equal(theEmployee.Id, employee.Id);
            Assert.Equal(theEmployee.Name, employee.Name);
            Assert.Equal(theEmployee.Department, employee.Department);
        }

        [Fact]
        public void PostEmployee_EmployeeIsNull_ShouldReturnBadRequest()
        {
            // Arrange

            // Act
            var result = _employeeController.PostEmployee(null, CancellationToken.None).Result;

            // Assert
            var badResult = Assert.IsAssignableFrom<BadRequestResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, badResult.StatusCode);
        }

        [Fact]
        public void PostEmployee_EmployeeNameIsNull_ShouldReturnBadRequest()
        {
            // Arrange
            var newEmployee = new Employee();

            // Act
            var result = _employeeController.PostEmployee(newEmployee, CancellationToken.None).Result;

            // Assert
            var badResult = Assert.IsAssignableFrom<BadRequestResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, badResult.StatusCode);
        }

        [Fact]
        public void PostEmployee_EmployeeIsValid_ShouldReturnTheEmployee()
        {
            // Arrange
            var newEmployee = DataSeeder.MockSingleEmployee();
            _mockEmpRepo.Setup(repo => repo.Add(newEmployee, CancellationToken.None));

            // Act
            var result = _employeeController.PostEmployee(newEmployee, CancellationToken.None).Result;

            // Assert
            var createdAtActionResult = Assert.IsAssignableFrom<CreatedAtActionResult>(result);
            Assert.Equal((int)HttpStatusCode.Created, createdAtActionResult.StatusCode);

            var employee = Assert.IsAssignableFrom<Employee>(createdAtActionResult.Value);
            Assert.NotNull(employee);
            Assert.Equal(newEmployee.Id, employee.Id);
            Assert.Equal(newEmployee.Name, employee.Name);
            Assert.Equal(newEmployee.Department, employee.Department);
        }

        [Fact]
        public void PutEmployee_GuidIsEmpty_ShouldReturnBadRequest()
        {
            // Arrange

            // Act
            var result = _employeeController.PutEmployee(Guid.Empty, new Employee(), CancellationToken.None).Result;

            // Assert
            var badResult = Assert.IsAssignableFrom<BadRequestResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, badResult.StatusCode);
        }

        [Fact]
        public void PutEmployee_EmployeeNameIsNull_ShouldReturnBadRequest()
        {
            // Arrange

            // Act
            var result = _employeeController.PutEmployee(Guid.NewGuid(), new Employee(), CancellationToken.None).Result;

            // Assert
            var badResult = Assert.IsAssignableFrom<BadRequestResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, badResult.StatusCode);
        }

        [Fact]
        public void PutEmployee_EmployeeIsNotFound_ShouldReturnBadRequest()
        {
            // Arrange
            var employee = DataSeeder.MockSingleEmployee();
            _mockEmpRepo.Setup(repo => repo.Update(employee, CancellationToken.None))
                .ReturnsAsync(default(Employee));

            // Act
            var result = _employeeController.PutEmployee(Guid.NewGuid(), employee, CancellationToken.None).Result;

            // Assert
            var badResult = Assert.IsAssignableFrom<BadRequestResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, badResult.StatusCode);
        }

        [Fact]
        public void PutEmployee_EmployeeIsValid_ShouldReturnTheEmployee()
        {
            // Arrange
            var employee = DataSeeder.MockSingleEmployee();
            _mockEmpRepo.Setup(repo => repo.Update(employee, CancellationToken.None))
                .ReturnsAsync(employee);

            // Act
            var result = _employeeController.PutEmployee(employee.Id, employee, CancellationToken.None).Result;

            // Assert
            var okResult = Assert.IsAssignableFrom<OkObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);

            var updatedEmployee = Assert.IsAssignableFrom<Employee>(okResult.Value);
            Assert.Equal(updatedEmployee.Id, employee.Id);
            Assert.Equal(updatedEmployee.Name, employee.Name);
            Assert.Equal(updatedEmployee.Department, employee.Department);
        }

        [Fact]
        public void DeleteEmployee_GuidIsEmpty_ShouldReturnBadRequest()
        {
            // Arrange

            // Act
            var result = _employeeController.DeleteEmployee(Guid.Empty, CancellationToken.None).Result;

            // Assert
            var badResult = Assert.IsAssignableFrom<BadRequestResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, badResult.StatusCode);
        }

        [Fact]
        public void DeleteEmployee_EmployeeIsNotFound_ShouldReturnNotFound()
        {
            // Arrange
            _mockEmpRepo.Setup(repo => repo.GetById(Guid.NewGuid(), CancellationToken.None))
                .ReturnsAsync(default(Employee));

            // Act
            var result = _employeeController.DeleteEmployee(Guid.NewGuid(), CancellationToken.None).Result;

            // Assert
            var notFoundResult = Assert.IsAssignableFrom<NotFoundResult>(result);
            Assert.Equal((int)HttpStatusCode.NotFound, notFoundResult.StatusCode);
        }

        [Fact]
        public void DeleteEmployee_GuidIsValid_ShouldReturnTheEmployee()
        {
            // Arrange
            var employee = DataSeeder.MockSingleEmployee();
            _mockEmpRepo.Setup(repo => repo.GetById(employee.Id, CancellationToken.None))
                .ReturnsAsync(employee);
            _mockEmpRepo.Setup(repo => repo.Remove(employee.Id, CancellationToken.None));

            // Act
            var result = _employeeController.DeleteEmployee(employee.Id, CancellationToken.None).Result;

            // Assert
            var okResult = Assert.IsAssignableFrom<OkObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);

            var updatedEmployee = Assert.IsAssignableFrom<Employee>(okResult.Value);
            Assert.Equal(updatedEmployee.Id, employee.Id);
            Assert.Equal(updatedEmployee.Name, employee.Name);
            Assert.Equal(updatedEmployee.Department, employee.Department);
        }
    }
}
