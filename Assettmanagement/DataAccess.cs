using Assettmanagement.Database;
using Assettmanagement.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Assettmanagement.Data
{
    public class DataAccess
    {
        private readonly mySQLDbContext _accessDatabase;

        public DataAccess(mySQLDbContext accessDatabase)
        {
            _accessDatabase = accessDatabase;
        }
        //**************************************************
        // Assets
        //**************************************************

        public async Task<List<Asset>> GetAssetsAsync()
        {
            return await _accessDatabase.Assets.ToListAsync();
        }
        public async Task<Asset> GetAssetAsync(int id)
        {
            return await _accessDatabase.Assets.Include(a => a.User).FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task AddAssetAsync(Asset asset)
        {
            _accessDatabase.Assets.Add(asset);
            await _accessDatabase.SaveChangesAsync();
        }

        public async Task DeleteAssetAsync(int id)
        {
            var asset = await _accessDatabase.Assets.FindAsync(id);
            if (asset != null)
            {
                _accessDatabase.Assets.Remove(asset);
                await _accessDatabase.SaveChangesAsync();
            }
        }
        public async Task<List<Asset>> GetAssignedAssetsAsync(string assetType)
        {
            IQueryable<Asset> assignedAssets = _accessDatabase.Assets
                .Include(a => a.User)
                .Where(a => a.UserId != null);

            if (!string.IsNullOrEmpty(assetType))
            {
                assignedAssets = assignedAssets.Where(a => a.AssetType == assetType);
            }

            return await assignedAssets.ToListAsync();
        }

        public async Task AssignAssetToUserAsync(int assetId, int userId)
        {
            var asset = await _accessDatabase.Assets.FindAsync(assetId);
            if (asset != null)
            {
                asset.UserId = userId;
                await _accessDatabase.SaveChangesAsync();
            }
        }

        public async Task ReturnAssetAsync(int assetId)
        {
            var asset = await _accessDatabase.Assets.FindAsync(assetId);
            if (asset != null)
            {
                asset.UserId = null;
                await _accessDatabase.SaveChangesAsync();
            }
        }

        public async Task<List<Asset>> GetAssignedAssetsByUserAsync(int userId)
        {
            return await _accessDatabase.Assets
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }


        public async Task AddAssetHistoryAsync(AssetHistory assetHistory)
        {
            await _accessDatabase.AssetHistories.AddAsync(assetHistory);
            await _accessDatabase.SaveChangesAsync();
        }


        public async Task<List<AssetHistory>> GetAssetHistoriesWithUsersAsync(int assetId)
        {
            return await _accessDatabase.AssetHistories
                .Include(ah => ah.User)
                .Where(ah => ah.AssetId == assetId)
                .OrderByDescending(ah => ah.Timestamp)
                .ToListAsync();
        }
        public async Task<List<Asset>> GetFilteredAssetsAsync(string assetType)
        {
            var query = _accessDatabase.Assets.Include(a => a.User).AsQueryable();

            if (!string.IsNullOrEmpty(assetType))
            {
                query = query.Where(a => a.AssetType == assetType);
            }

            return await query.ToListAsync();
        }

        //**************************************************
        //USERS 
        //**************************************************
        public async Task<List<User>> GetUsersAsync()
        {
            return await _accessDatabase.Users.ToListAsync();
        }

        public async Task AddUserAsync(User user)
        {
            _accessDatabase.Users.Add(user);
            await _accessDatabase.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _accessDatabase.Users.FindAsync(id);
            if (user != null)
            {
                _accessDatabase.Users.Remove(user);
                await _accessDatabase.SaveChangesAsync();
            }
        }
                public async Task<User> GetUserAsync(int userId)
        {
            return await _accessDatabase.Users.FindAsync(userId);
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _accessDatabase.Users.FindAsync(userId);
        }

        public async Task<User> GetOrCreateSystemUserAsync()
        {
            User systemUser = await _accessDatabase.Users
                .FirstOrDefaultAsync(u => u.FirstName == "System" && u.LastName == "System");

            if (systemUser == null)
            {
                systemUser = new User
                {
                    FirstName = "System",
                    LastName = "User",
                };

                await _accessDatabase.Users.AddAsync(systemUser);
                await _accessDatabase.SaveChangesAsync();
            }

            return systemUser;
        }



        //**************************************************
        // BOOKING 
        //**************************************************
        public async Task<List<Asset>> GetAvailableAssetsAsync(string assetType)
        {
            IQueryable<Asset> availableAssets = _accessDatabase.Assets
                .Where(a => a.UserId == null);

            if (!string.IsNullOrEmpty(assetType))
            {
                availableAssets = availableAssets.Where(a => a.AssetType == assetType);
            }

            return await availableAssets.ToListAsync();
        }
    }
}
