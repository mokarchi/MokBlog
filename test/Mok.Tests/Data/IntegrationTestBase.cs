using System;
using Mok.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Mok.Tests.Data
{
    /// <summary>
    /// Base class for all integration tests, it provides the <see cref="ApplicationDbContext"/> with 
    /// in-memory database.
    /// </summary>
    public class IntegrationTestBase : IDisposable
    {
        /// <summary>
        /// A <see cref="ApplicationDbContext"/> built with Sqlite in-memory mode.
        /// </summary>
        protected readonly ApplicationDbContext _db;
        protected readonly ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

        /// <summary>
        /// Initializes DbContext with SQLite Database Provider in-memory mode with logging to
        /// console and ensure database created.
        /// </summary>
        public IntegrationTestBase()
        {
            var connection = new SqliteConnection() { ConnectionString = "Data Source=:memory:" };
            connection.Open();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                //.UseLoggerFactory(loggerFactory) // turn on logging to see generated SQL
                .UseSqlite(connection).Options;

            _db = new ApplicationDbContext(options, loggerFactory);
            _db.Database.EnsureCreated();
        }

        public void Dispose()
        {
            _db.Database.EnsureDeleted(); // important, otherwise SeedTestData is not erased
            _db.Dispose();
        }
    }
}
