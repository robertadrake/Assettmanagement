using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace Assettmanagement.Database
{
    public class AccessDatabase
    {
        private readonly IConfiguration _configuration;

        public AccessDatabase(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SqliteConnection GetConnection()
        {
            string connectionString = _configuration.GetConnectionString("SqliteDatabase");
            var connection = new SqliteConnection(connectionString);
            connection.Open();

            if (connection.State == System.Data.ConnectionState.Open)
            {
                // Create the necessary tables if they don't exist
                string createAssetsTable = @"
                CREATE TABLE IF NOT EXISTS Assets (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Description TEXT,
                    SerialNumber TEXT NOT NULL,
                    AssetNumber TEXT NOT NULL,
                    Location TEXT NOT NULL,
                    AssetType TEXT NOT NULL,
                    UserId INTEGER,
                    FOREIGN KEY (UserId) REFERENCES Users (Id)
                );
                ";
                string createUsersTable = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    FirstName TEXT NOT NULL,
                    LastName TEXT NOT NULL
                );
                ";
                string createAssetHistoryTableQuery = @"
                CREATE TABLE IF NOT EXISTS AssetHistory (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    AssetId INTEGER NOT NULL,
                    UserId INTEGER NOT NULL,
                    Comment TEXT NOT NULL,
                    Timestamp TEXT NOT NULL,
                    FOREIGN KEY (AssetId) REFERENCES Asset (Id),
                    FOREIGN KEY (UserId) REFERENCES User (Id)
                );
                ";
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = createAssetsTable;
                    command.ExecuteNonQuery();
                    command.CommandText = createUsersTable;
                    command.ExecuteNonQuery();
                    command.CommandText = createAssetHistoryTableQuery;
                    command.ExecuteNonQuery();
                }
            }

            return connection;
        }
    }
}
