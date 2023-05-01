using Assettmanagement.Database;
using Assettmanagement.Models;
using Microsoft.Data.Sqlite;

namespace Assettmanagement.Data
{
    public class DataAccess
    {
        private readonly AccessDatabase _accessDatabase;

        public DataAccess(AccessDatabase accessDatabase)
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
                                UserId = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6)
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
                                UserId = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6)
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
                using (var command = new SqliteCommand("INSERT INTO Assets (Name, Description, SerialNumber, AssetNumber, Location) VALUES (@Name, @description, @serialNumber, @assetNumber, @location)", connection))
                {
                    command.Parameters.AddWithValue("@Name", asset.Name);
                    command.Parameters.AddWithValue("@description", asset.Description);
                    command.Parameters.AddWithValue("@serialNumber", asset.SerialNumber);
                    command.Parameters.AddWithValue("@assetNumber", asset.AssetNumber);
                    command.Parameters.AddWithValue("@location", asset.Location);
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
                using (var command = new SqliteCommand("INSERT INTO Users (FirstName, LastName) VALUES (@firstName, @lastName)", connection))
                {
                    command.Parameters.AddWithValue("@firstName", user.FirstName);
                    command.Parameters.AddWithValue("@lastName", user.LastName);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task UpdateUserAsync(User user)
        {
            using (var connection = _accessDatabase.GetConnection())
            {
                using (var command = new SqliteCommand(@"UPDATE Users SET FirstName = @FirstName, LastName = @LastName, WHERE Id = @Id;", connection))
                {
                    command.Parameters.AddWithValue("@firstName", user.FirstName);
                    command.Parameters.AddWithValue("@lastName", user.LastName);
                    command.Parameters.AddWithValue("@Id", user.Id);
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
                                UserId = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6)
                            });
                        }
                    }
                }

                return assets;
            }
        }

        public async Task<List<Asset>> GetAssignedAssetsAsync()
        {
            using (var connection = _accessDatabase.GetConnection())
            {
                var assets = new List<Asset>();
                using (var command = new SqliteCommand("SELECT a.*, u.* FROM Assets a INNER JOIN Users u ON a.UserId = u.Id WHERE a.UserId IS NOT NULL", connection))
                {
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
                                UserId = reader.GetInt32(6)
                            };

                            asset.User = new User
                            {
                                Id = reader.GetInt32(7),
                                FirstName = reader.GetString(8),
                                LastName = reader.GetString(9)
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
                                UserId = reader.IsDBNull(6) ? null : reader.GetInt32(6)
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
                                UserId = reader.IsDBNull(6) ? null : reader.GetInt32(6),
                                User = reader.IsDBNull(7) ? null : new User
                                {
                                    Id = reader.GetInt32(7),
                                    FirstName = reader.GetString(8),
                                    LastName = reader.GetString(9),
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
                const string query = "UPDATE Assets SET Name = @Name, Description = @Description, SerialNumber = @SerialNumber, AssetNumber = @AssetNumber, Location = @Location, UserId = @UserId WHERE Id = @Id;";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", asset.Id);
                    command.Parameters.AddWithValue("@Name", asset.Name);
                    command.Parameters.AddWithValue("@Description", asset.Description);
                    command.Parameters.AddWithValue("@SerialNumber", asset.SerialNumber);
                    command.Parameters.AddWithValue("@AssetNumber", asset.AssetNumber);
                    command.Parameters.AddWithValue("@Location", asset.Location);
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
                                // Add any other User properties you have in your model
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


    }
}
