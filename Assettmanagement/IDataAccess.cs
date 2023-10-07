using Assettmanagement.Database;
using Assettmanagement.Models;
using Microsoft.Data.Sqlite;

namespace Assettmanagement.Data
{
    public class IDataAccess
    {
        private readonly AccessDatabase _accessDatabase;

        public IDataAccess(AccessDatabase accessDatabase)
        {
            _accessDatabase = accessDatabase;
        }

        // Assets methods
        public async Task<List<Asset>> GetAssetsAsync()
        {
            using (var connection = _accessDatabase.GetConnection())
            {
                var assets = new List<Asset>();
                using (var command = new SqliteCommand("SELECT * FROM Assets", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            assets.Add(new Asset
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Description = reader.GetString(2),
                                SerialNumber = reader.GetString(3),
                                AssetNumber = reader.GetString(4),
                                Location = reader.GetString(5),
                                AssetType = reader.GetString(6),
                                UserId = reader.IsDBNull(7) ? (int?)null : reader.GetInt32(7)
                            });
                        }
                    }
                }

                return assets;
            }
        }

        public async Task<Asset> GetAssetAsync(int id)
        {
            using (var connection = _accessDatabase.GetConnection())
            {
                Asset asset = null;
                using (var command = new SqliteCommand("SELECT * FROM Assets WHERE Id = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            asset = new Asset
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Description = reader.GetString(2),
                                SerialNumber = reader.GetString(3),
                                AssetNumber = reader.GetString(4),
                                Location = reader.GetString(5),
                                AssetType = reader.GetString(6),
                                UserId = reader.IsDBNull(7) ? (int?)null : reader.GetInt32(7)
                            };
                        }
                    }
                }

                return asset;
            }
        }

        public async Task AddAssetAsync(Asset asset)
        {
            using (var connection = _accessDatabase.GetConnection())
            {
                using (var command = new SqliteCommand("INSERT INTO Assets (Name, Description, SerialNumber, AssetNumber, Location, Assettype) VALUES (@Name, @description, @serialNumber, @assetNumber, @location, @assettype)", connection))
                {
                    command.Parameters.AddWithValue("@Name", asset.Name);
                    command.Parameters.AddWithValue("@description", asset.Description);
                    command.Parameters.AddWithValue("@serialNumber", asset.SerialNumber);
                    command.Parameters.AddWithValue("@assetNumber", asset.AssetNumber);
                    command.Parameters.AddWithValue("@location", asset.Location);
                    command.Parameters.AddWithValue("@assettype", asset.AssetType);
                    command.Parameters.AddWithValue("@UserId", asset.UserId);
                    // command.Parameters.AddWithValue("@bookedOutTo", (object)asset.BookedOutTo ?? DBNull.Value);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteAssetAsync(int id)
        {
            using (var connection = _accessDatabase.GetConnection())
            {
                using (var command = new SqliteCommand("DELETE FROM Assets WHERE Id = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        // Users methods
        public async Task<List<User>> GetUsersAsync()
        {
            using (var connection = _accessDatabase.GetConnection())
            {
                var users = new List<User>();
                using (var command = new SqliteCommand("SELECT * FROM Users", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            users.Add(new User
                            {
                                Id = reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                Email = reader.IsDBNull(3) ? "NOT SET" : reader.GetString(3),
                                PasswordHash = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                IsAdministrator = reader.GetBoolean(5)
                            });
                        }
                    }
                }

                return users;
            }
        }

        public async Task AddUserAsync(User user)
        {
            using (var connection = _accessDatabase.GetConnection())
            {
                using (var command = new SqliteCommand("INSERT INTO Users (FirstName, LastName, Email, PasswordHash, IsAdministrator) VALUES (@firstName, @lastName, @email, @passwordHash, @IsAdministrator)", connection))
                {
                    command.Parameters.AddWithValue("@firstName", user.FirstName);
                    command.Parameters.AddWithValue("@lastName", user.LastName);
                    command.Parameters.AddWithValue("@email", user.Email);  // New line
                    command.Parameters.AddWithValue("@passwordHash", user.PasswordHash);  // New line
                    command.Parameters.AddWithValue("@IsAdministrator", user.IsAdministrator);  // New line
                    await command.ExecuteNonQueryAsync();
                }

            }
        }
        public async Task<User> GetUserByEmailAsync(string email)
        {
            using (var connection = _accessDatabase.GetConnection())
            {
                User user = null;
                using (var command = new SqliteCommand("SELECT * FROM Users WHERE Email = @Email", connection))
                {
                    command.Parameters.AddWithValue("@Email", email);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            user = new User
                            {
                                Id = reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                Email = reader.GetString(3),
                                PasswordHash = reader.GetString(4),
                                IsAdministrator = reader.GetBoolean(5)
                            };
                        }
                    }
                }

                return user;
            }
        }



        public async Task UpdateUserAsync(User user)
        {
            using (var connection = _accessDatabase.GetConnection())
            {
                using (var command = new SqliteCommand("UPDATE Users SET FirstName = @firstName, LastName = @lastName, Email = @email, PasswordHash = @passwordHash, IsAdministrator = @isAdministrator WHERE Id = @id", connection))
                {
                    command.Parameters.AddWithValue("@firstName", user.FirstName);
                    command.Parameters.AddWithValue("@lastName", user.LastName);
                    command.Parameters.AddWithValue("@email", user.Email);
                    command.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
                    command.Parameters.AddWithValue("@isAdministrator", user.IsAdministrator);
                    command.Parameters.AddWithValue("@id", user.Id);

                    await command.ExecuteNonQueryAsync();
                }

            }
        }

        public async Task DeleteUserAsync(int id)
        {
            using (var connection = _accessDatabase.GetConnection())
            {
                using (var command = new SqliteCommand("DELETE FROM Users WHERE Id = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        // Booking methods
        public async Task BookAssetAsync(int assetId, int userId)
        {
            using (var connection = _accessDatabase.GetConnection())
            {
                using (var command = new SqliteCommand("UPDATE Assets SET BookedOutTo = @userId WHERE Id = @assetId", connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@assetId", assetId);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<Asset>> GetAvailableAssetsAsync()
        {
            using (var connection = _accessDatabase.GetConnection())
            {
                var assets = new List<Asset>();
                using (var command = new SqliteCommand("SELECT * FROM Assets WHERE UserId IS NULL", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            assets.Add(new Asset
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Description = reader.GetString(2),
                                SerialNumber = reader.GetString(3),
                                AssetNumber = reader.GetString(4),
                                Location = reader.GetString(5),
                                AssetType = reader.GetString(6),
                                UserId = reader.IsDBNull(7) ? (int?)null : reader.GetInt32(7)
                            });
                        }
                    }
                }

                return assets;
            }
        }

        public async Task<List<Asset>> GetAvailableAssetsAsync(string assetType)
        {
            List<Asset> assets = new List<Asset>();

            using (var connection = _accessDatabase.GetConnection())
            {
                string query = "SELECT * FROM Assets WHERE UserId IS NULL";

                if (!string.IsNullOrEmpty(assetType))
                {
                    query += " AND AssetType = @AssetType";
                }

                using (var command = new SqliteCommand(query, connection))
                {
                    if (!string.IsNullOrEmpty(assetType))
                    {
                        command.Parameters.AddWithValue("@AssetType", assetType);
                    }

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var asset = new Asset
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                SerialNumber = reader.GetString(reader.GetOrdinal("SerialNumber")),
                                AssetNumber = reader.GetString(reader.GetOrdinal("AssetNumber")),
                                Location = reader.GetString(reader.GetOrdinal("Location")),
                                AssetType = reader.GetString(reader.GetOrdinal("AssetType")),
                                UserId = reader.IsDBNull(reader.GetOrdinal("UserId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("UserId"))
                            };

                            assets.Add(asset);
                        }
                    }
                }
            }

            return assets;
        }



        public async Task<List<Asset>> GetAssignedAssetsAsync(string assetType)
        {
            using (var connection = _accessDatabase.GetConnection())
            {
                var assets = new List<Asset>();
                string query = "SELECT a.*, u.* FROM Assets a INNER JOIN Users u ON a.UserId = u.Id WHERE a.UserId IS NOT NULL";

                if (!string.IsNullOrEmpty(assetType))
                {
                    query += " AND AssetType = @AssetType";
                }

                using (var command = new SqliteCommand(query, connection))
                {
                    if (!string.IsNullOrEmpty(assetType))
                    {
                        command.Parameters.AddWithValue("@AssetType", assetType);
                    }
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var asset = new Asset
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Description = reader.GetString(2),
                                SerialNumber = reader.GetString(3),
                                AssetNumber = reader.GetString(4),
                                Location = reader.GetString(5),
                                AssetType = reader.GetString(6),
                                UserId = reader.GetInt32(7)
                            };

                            asset.User = new User
                            {
                                Id = reader.GetInt32(8),
                                FirstName = reader.GetString(9),
                                LastName = reader.GetString(10)
                            };

                            assets.Add(asset);
                        }
                    }
                }

                return assets;
            }
        }

        public async Task AssignAssetToUserAsync(int assetId, int userId)
        {
            using (var connection = _accessDatabase.GetConnection())
            {
                using (var command = new SqliteCommand("UPDATE Assets SET UserId = @UserId WHERE Id = @AssetId", connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@AssetId", assetId);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

          public async Task ReturnAssetAsync(int assetId)
          {
              using (var connection = _accessDatabase.GetConnection())
              {
                  using (var command = new SqliteCommand("UPDATE Assets SET UserId = NULL WHERE Id = @AssetId", connection))
                  {
                      command.Parameters.AddWithValue("@AssetId", assetId);

                      await command.ExecuteNonQueryAsync();
                  }
              }
          }


        public async Task<List<Asset>> GetAssignedAssetsByUserAsync(int userId)
        {
            using (var connection = _accessDatabase.GetConnection())
            {
                var assets = new List<Asset>();

                using (var command = new SqliteCommand("SELECT * FROM Assets WHERE UserId = @UserId", connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            assets.Add(new Asset
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Description = reader.GetString(2),
                                SerialNumber = reader.GetString(3),
                                AssetNumber = reader.GetString(4),
                                Location = reader.GetString(5),
                                AssetType = reader.GetString(6),
                                UserId = reader.IsDBNull(7) ? null : reader.GetInt32(7)
                            });
                        }
                    }
                }

                return assets;
            }
        }

        public async Task<User> GetUserAsync(int userId)
        {
            using (var connection = _accessDatabase.GetConnection())
            {
                User user = null;

                using (var command = new SqliteCommand("SELECT * FROM Users WHERE Id = @UserId", connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            user = new User
                            {
                                Id = reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                            };
                        }
                    }
                }

                return user;
            }
        }



        public async Task<List<Asset>> GetAssetsWithUsersAsync()
        {
            using (var connection = _accessDatabase.GetConnection())
            {
                var assets = new List<Asset>();

                using (var command = new SqliteCommand("SELECT a.*, u.* FROM Assets a LEFT JOIN Users u ON a.UserId = u.Id", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            assets.Add(new Asset
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Description = reader.GetString(2),
                                SerialNumber = reader.GetString(3),
                                AssetNumber = reader.GetString(4),
                                Location = reader.GetString(5),
                                AssetType = reader.GetString(6),
                                UserId = reader.IsDBNull(7) ? null : reader.GetInt32(7),
                                User = reader.IsDBNull(8) ? null : new User
                                {
                                    Id = reader.GetInt32(8),
                                    FirstName = reader.GetString(9),
                                    LastName = reader.GetString(10),
                                }
                            });
                        }
                    }
                }

                return assets;
            }
        }


        public async Task UpdateAssetAsync(Asset asset)
        {
            using (var connection = _accessDatabase.GetConnection())
            {
                const string query = "UPDATE Assets SET Name = @Name, Description = @Description, SerialNumber = @SerialNumber, AssetNumber = @AssetNumber, Location = @Location, Assettype = @Assettype, UserId = @UserId WHERE Id = @Id;";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", asset.Id);
                    command.Parameters.AddWithValue("@Name", asset.Name);
                    command.Parameters.AddWithValue("@Description", asset.Description);
                    command.Parameters.AddWithValue("@SerialNumber", asset.SerialNumber);
                    command.Parameters.AddWithValue("@AssetNumber", asset.AssetNumber);
                    command.Parameters.AddWithValue("@Location", asset.Location);
                    command.Parameters.AddWithValue("@Assettype", asset.AssetType);
                    command.Parameters.AddWithValue("@UserId", (object)asset.UserId ?? DBNull.Value);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        // Assett history...
        public async Task AddAssetHistoryAsync(AssetHistory assetHistory)
        {
            using (var connection = _accessDatabase.GetConnection())
            {
                string insertQuery = @"INSERT INTO AssetHistory (AssetId, UserId, Timestamp, Comment)
                                VALUES (@AssetId, @UserId, @Timestamp, @Comment);";
                using (var command = new SqliteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@AssetId", assetHistory.AssetId);
                    command.Parameters.AddWithValue("@UserId", assetHistory.UserId);
                    command.Parameters.AddWithValue("@Timestamp", assetHistory.Timestamp);
                    command.Parameters.AddWithValue("@Comment", assetHistory.Comment);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<AssetHistory>> GetAssetHistoryAsync(int assetId)
        {
            using (var connection = _accessDatabase.GetConnection())
            {
                List<AssetHistory> assetHistories = new List<AssetHistory>();

                string selectQuery = @"SELECT * FROM AssetHistory
                               WHERE AssetId = @AssetId
                               ORDER BY Timestamp DESC;";
                using (var command = new SqliteCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@AssetId", assetId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var assetHistory = new AssetHistory
                            {
                                Id = reader.GetInt32(0),
                                AssetId = reader.GetInt32(1),
                                UserId = reader.GetInt32(2),
                                Timestamp = reader.GetDateTime(3),
                                Comment = reader.GetString(4)
                            };

                            assetHistories.Add(assetHistory);
                        }
                    }
                }

                return assetHistories;
            }
        }

        public async Task<List<AssetHistory>> GetAssetHistoriesWithUsersAsync(int assetId)
        {
            using (var connection = _accessDatabase.GetConnection())
            {
                var assetHistories = new List<AssetHistory>();

                using (var command = new SqliteCommand("SELECT AssetHistory.Id, AssetHistory.Timestamp, AssetHistory.Comment, AssetHistory.UserId, Users.FirstName, Users.LastName FROM AssetHistory JOIN Users ON AssetHistory.UserId = Users.Id WHERE AssetHistory.AssetId = @AssetId ORDER BY AssetHistory.Timestamp DESC", connection))
                {
                    command.Parameters.AddWithValue("@AssetId", assetId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            assetHistories.Add(new AssetHistory
                            {
                                Id = reader.GetInt32(0),
                                Timestamp = reader.GetDateTime(1),
                                Comment = reader.GetString(2),
                                UserId = reader.GetInt32(3),
                                User = new User
                                {
                                    Id = reader.GetInt32(3),
                                    FirstName = reader.GetString(4),
                                    LastName = reader.GetString(5),
                                },
                                AssetId = assetId
                            });
                        }
                    }
                }

                return assetHistories;
            }
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            using (var connection = _accessDatabase.GetConnection())
            {
                using (var command = new SqliteCommand("SELECT * FROM Users WHERE Id = @UserId", connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var user = new User
                            {
                                Id = reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                Email = reader.IsDBNull(3) ? null : reader.GetString(3),
                                PasswordHash = reader.IsDBNull(4) ? null : reader.GetString(4),
                                IsAdministrator = reader.IsDBNull(5) ? false : reader.GetBoolean(5)
                            };

                            return user;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }

        public async Task<User> GetOrCreateSystemUserAsync()
        {
            using (var connection = _accessDatabase.GetConnection())
            {
                User systemUser = null;

                // Check if a System user already exists
                using (var command = new SqliteCommand("SELECT * FROM Users WHERE FirstName = 'System' AND LastName = 'System'", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                systemUser = new User
                                {
                                    Id = reader.GetInt32(0),
                                    FirstName = reader.GetString(1),
                                    LastName = reader.GetString(2),
                                };
                            }
                        }
                    }
                }

                // If a System user does not exist, create one
                if (systemUser == null)
                {
                    using (var command = new SqliteCommand("INSERT INTO Users (FirstName, LastName) VALUES ('System', 'System'); SELECT last_insert_rowid();", connection))
                    {
                        var newUserId = await command.ExecuteScalarAsync();

                        systemUser = new User
                        {
                            Id = Convert.ToInt32(newUserId),
                            FirstName = "System",
                            LastName = "User",
                        };
                    }
                }

                return systemUser;
            }
        }
        public async Task<List<Asset>> GetFilteredAssetsAsync(string assetType)
        {
            List<Asset> assets = new List<Asset>();

            using (var connection = _accessDatabase.GetConnection())
            {
                string query = @"
                    SELECT Assets.*, Users.FirstName, Users.LastName
                    FROM Assets
                    LEFT JOIN Users ON Assets.UserId = Users.Id
                    ";

                if (!string.IsNullOrEmpty(assetType))
                {
                    query += " WHERE Assets.AssetType = @AssetType";
                }

                using (var command = new SqliteCommand(query, connection))
                {
                    if (!string.IsNullOrEmpty(assetType))
                    {
                        command.Parameters.AddWithValue("@AssetType", assetType);
                    }

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var asset = new Asset
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                SerialNumber = reader.GetString(reader.GetOrdinal("SerialNumber")),
                                AssetNumber = reader.GetString(reader.GetOrdinal("AssetNumber")),
                                Location = reader.GetString(reader.GetOrdinal("Location")),
                                AssetType = reader.GetString(reader.GetOrdinal("AssetType")),
                                UserId = reader.IsDBNull(reader.GetOrdinal("UserId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("UserId"))
                            };

                            if (!reader.IsDBNull(reader.GetOrdinal("FirstName")) && !reader.IsDBNull(reader.GetOrdinal("LastName")))
                            {
                                asset.User = new User
                                {
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName"))
                                };
                            }

                            assets.Add(asset);
                        }
                    }
                }
            }

            return assets;
        }



    }
}
