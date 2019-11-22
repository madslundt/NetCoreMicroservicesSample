using System;
using System.Data.Common;
using System.IO;
using System.Reflection;

namespace Infrastructure.Outbox
{
    public static class OutboxInstaller
    {
        public static void Install(DbConnection connection, string schema = null)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));

            var script = GetInstallScript(schema);

            using (connection)
            {
                try
                {
                    // Open the connection.
                    connection.Open();

                    // Create and execute the DbCommand.
                    DbCommand command = connection.CreateCommand();
                    command.CommandText = script;
                    command.CommandTimeout = 0;
                    int rows = command.ExecuteNonQuery();

                    // Display number of rows inserted.
                    Console.WriteLine("Inserted {0} rows.", rows);
                }
                // Handle data errors.
                catch (DbException exDb)
                {
                    Console.WriteLine("DbException.GetType: {0}", exDb.GetType());
                    Console.WriteLine("DbException.Source: {0}", exDb.Source);
                    Console.WriteLine("DbException.ErrorCode: {0}", exDb.ErrorCode);
                    Console.WriteLine("DbException.Message: {0}", exDb.Message);
                }
                // Handle all other exceptions.
                catch (Exception ex)
                {
                    Console.WriteLine("Exception.Message: {0}", ex.Message);
                }
                finally
                {
                    connection.Dispose();
                }
            }
        }

        public static string GetInstallScript(string schema)
        {
            var script = GetStringResource(
                typeof(OutboxInstaller).GetTypeInfo().Assembly,
                "Outbox.Install.sql");

            script = script.Replace("$(OutboxSchema)", !string.IsNullOrWhiteSpace(schema) ? schema : "Outbox");

            return script;
        }

        private static string GetStringResource(Assembly assembly, string resourceName)
        {
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException(
                        $"Requested resource `{resourceName}` was not found in the assembly `{assembly}`.");
                }

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
