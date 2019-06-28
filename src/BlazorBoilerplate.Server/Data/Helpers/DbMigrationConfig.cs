using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorBoilerplate.Server.Data.Helpers
{
    public static class DbMigrationConfig
    {
        // use this connection string to run all dbMigrations (in TemporaryDbContextFactory.cs)
        
        public const string SqliteDbMigrationConnectionString = @"Filename=data.db";
        // public const string SqlServerDbMigrationConnectionString = @"...xxx...";
    }
}