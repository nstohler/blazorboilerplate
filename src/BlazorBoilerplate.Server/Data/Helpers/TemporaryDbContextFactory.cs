using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BlazorBoilerplate.Server.Data.Helpers
{
    internal class TemporaryDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

            //builder.UseSqlServer(DbMigrationConfig.SqlServerDbMigrationConnectionString);
            builder.UseSqlite(DbMigrationConfig.SqliteDbMigrationConnectionString);

            //var dummyDbContextConfig = new DbContextConfig()
            //{
            //    ConnectionString           = null,
            //    AddLibLogLoggerProvider    = false,
            //    EnableSensitiveDataLogging = false,
            //};

            return new ApplicationDbContext(builder.Options);
        }
    }
}
