using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;

namespace UnitTest
{
    public static class DatabaseContextMock<T> where T : DbContext
    {
        public const string SQLITE_CONNECTION_STRING = "DataSource:memory:";

        public static DbContextOptions<T> InMemoryDatabase()
        {
            DbContextOptions<T> options = new DbContextOptionsBuilder<T>()
                  .UseInMemoryDatabase(Guid.NewGuid().ToString())
                  .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                  .Options;

            return options;
        }

        // Remember to open and close connection when using Sqlite
        //public static DbContextOptions<T> SQLite(SqliteConnection connection)
        //{
        //    DbContextOptions<T> options = new DbContextOptionsBuilder<T>()
        //          .UseSqlite(connection)
        //          .Options;

        //    return options;
        //}
    }
}
