//using Assettmanagement.Database;
using Assettmanagement.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Assettmanagement.Data
{
    public class IDataAccess
    {
        private readonly AppDbContext _context;

        public IDataAccess(AppDbContext context)
        {
            _context = context;
        }

        // Assets methods
        public async Task<List<Asset>> GetAssetsAsync()
        {
            return await _context.Assets.ToListAsync();
        }

        public async Task<Asset> GetAssetAsync(int id)
        {
            return await _context.Assets.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<int?> GetHighestAssetNumberAsync()
        {
            return await _context.Assets.MaxAsync(a => (int?)a.AssetNumber);
        }

        public async Task AddAssetAsync(Asset asset)
        {
            _context.Assets.Add(asset);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAssetAsync(int id)
        {
            var asset = new Asset { Id = id };
            _context.Entry(asset).State = EntityState.Deleted;
            await _context.SaveChangesAsync();
        }

        // User methods
        public async Task AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetUserAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        // Booking methods
        public async Task<List<Asset>> GetAvailableAssetsAsync(string assetType)
        {
            var query = _context.Assets.AsQueryable();

            if (!string.IsNullOrEmpty(assetType))
            {
                query = query.Where(a => a.AssetType == assetType);
            }

            return await query.Where(a => a.UserId == null).ToListAsync();
        }

        public async Task<List<Asset>> GetAssignedAssetsAsync(string assetType)
        {
            var query = _context.Assets.Include(a => a.User).AsQueryable();

            if (!string.IsNullOrEmpty(assetType))
            {
                query = query.Where(a => a.AssetType == assetType);
            }

            return await query.Where(a => a.UserId != null).ToListAsync();
        }

        public async Task AssignAssetToUserAsync(int assetId, int userId)
        {
            var asset = await _context.Assets.FindAsync(assetId);
            if (asset != null)
            {
                asset.UserId = userId;
                await _context.SaveChangesAsync();
            }
        }

        public async Task ReturnAssetAsync(int assetId)
        {
            var asset = await _context.Assets.FindAsync(assetId);
            if (asset != null)
            {
                asset.UserId = null;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Asset>> GetAssignedAssetsByUserAsync(int userId)
        {
            return await _context.Assets
                                 .Where(a => a.UserId == userId)
                                 .ToListAsync();
        }



        ///**********************************
        // Assett history...
        public async Task AddAssetHistoryAsync(AssetHistory assetHistory)
        {
            _context.AssetHistories.Add(assetHistory);
            await _context.SaveChangesAsync();
        }

        public async Task<List<AssetHistory>> GetAssetHistoriesWithUsersAsync(int assetId)
        {
            return await _context.AssetHistories
                                 .Include(ah => ah.User)
                                 .Where(ah => ah.AssetId == assetId)
                                 .OrderByDescending(ah => ah.Timestamp)
                                 .ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        // used to also create a system user if it did not exist,  but this is crazy pratcice
        public async Task<User> GetOrCreateSystemUserAsync()
        {
            var systemUser = await _context.Users
                                           .FirstOrDefaultAsync(u => u.FirstName == "System" && u.LastName == "System");

            if (systemUser == null)
            {
                systemUser = new User { FirstName = "System", LastName = "System", Email ="system@home.com", PasswordHash="123456" };
                _context.Users.Add(systemUser);
                await _context.SaveChangesAsync();
            }

            return systemUser;
        }
        public async Task<List<Asset>> GetAssetsByUserEmailAsync(string email, string assetType)
        {
            var query = _context.Assets
                                .Include(a => a.User)
                                .Where(a => a.User.Email == email);

            if (!string.IsNullOrEmpty(assetType))
            {
                query = query.Where(a => a.AssetType == assetType);
            }

            return await query.ToListAsync();
        }

        public async Task<List<Asset>> GetFilteredAssetsAsync(string assetType)
        {
            IQueryable<Asset> query = _context.Assets.Include(a => a.User);

            if (!string.IsNullOrEmpty(assetType))
            {
                query = query.Where(a => a.AssetType == assetType);
            }

            return await query.ToListAsync();
        }

    }
}
