using System.Security.Cryptography;
using System.Text;
using jornAPP.Components.Data;
using jornAPP.Components.Models;

namespace jornAPP.Services
{
    public class SecurityService
    {
        private readonly AppDatabase _db;

        private User _currentUser; // store logged-in user

        public SecurityService(AppDatabase db)
        {
            _db = db;
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        // Register new user
        public async Task<bool> RegisterAsync(string username, string password)
        {
            var existing = await _db.Connection
                .Table<User>()
                .FirstOrDefaultAsync(u => u.Username == username);

            if (existing != null)
                return false;

            var user = new User
            {
                Username = username,
                PasswordHash = HashPassword(password),
                CreatedAt = DateTime.Now
            };

            await _db.Connection.InsertAsync(user);

            // store as current user immediately
            _currentUser = user;

            return true;
        }

        // Login / Unlock
        public async Task<bool> LoginAsync(string username, string password)
        {
            var user = await _db.Connection
                .Table<User>()
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return false;

            bool valid = user.PasswordHash == HashPassword(password);
            if (valid)
                _currentUser = user; // store logged-in user
            return valid;
        }

        // Get currently logged-in user
        public Task<User> GetCurrentUserAsync()
        {
            return Task.FromResult(_currentUser);
        }

        // Check if user exists
        public async Task<bool> IsUserRegisteredAsync()
        {
            return await _db.Connection.Table<User>().CountAsync() > 0;
        }
        // Add this to SecurityService
        public async Task DeleteAllUsersAsync()
        {
            await _db.Connection.DeleteAllAsync<User>();
            _currentUser = null;
        }

    }
}
