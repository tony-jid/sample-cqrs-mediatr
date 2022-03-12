using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleCQRSMediatR.Data.Contexts.Master
{
    public class DataMasterContext : DbContext
    {
        public DataMasterContext(DbContextOptions<DataMasterContext> options) : base(options) { }

        public DbSet<Models.Employee> Employees { get; set; }
    }
}
