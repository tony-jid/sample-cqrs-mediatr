using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SampleCQRSMediatR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleCQRSMediatR.Data.Contexts.Master
{
    public class DataSeeder
    {
        public static void Seed(IServiceProvider serviceProvider)
        {
            using (var context = new DataMasterContext(serviceProvider.GetRequiredService<DbContextOptions<DataMasterContext>>()))
            {
                if (context.Employees.Any()) return;

                context.Employees.AddRangeAsync(
                    MockEmployees()
                );

                context.SaveChanges();
            }
        }

        public static Guid SingleEmployeeId = new Guid("4a968bbb-af79-4ff2-988c-1c5f80ade75d");
        public static Employee MockSingleEmployee()
        {
            return new Employee()
            {
                Id = SingleEmployeeId,
                Name = "Tony",
                Department = "IT",
                Salary = 150000
            };
        }

        public static List<Employee> MockEmployees()
        {
            return new List<Employee>()
            {
                new Employee()
                    {
                        Id = new Guid("4a968bbb-af79-4ff2-988c-1c5f80ade75d"),
                        Name = "Tony",
                        Department = "IT",
                        Salary = 100000
                    },
                    new Employee()
                    {
                        Id = new Guid("03e2edf8-6184-4d5d-ae21-1c3678036346"),
                        Name = "Sandy",
                        Department = "HR",
                        Salary = 50000
                    },
                    new Employee()
                    {
                        Id = new Guid("d771e0fd-e15e-4d7a-8bcb-c7cebaa9fcb6"),
                        Name = "Bob",
                        Department = "ACT",
                        Salary = 70000
                    }
            };
        }
    }
}
