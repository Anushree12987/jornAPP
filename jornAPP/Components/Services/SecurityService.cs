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
        private const string CurrentUserIdKey = "CurrentUserId";

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

            // Store as current user
            _currentUser = user;
            Preferences.Set(CurrentUserIdKey, user.Id);

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
            {
                _currentUser = user;
                Preferences.Set(CurrentUserIdKey, user.Id);
            }
            return valid;
        }

        // Get currently logged-in user
        public async Task<User> GetCurrentUserAsync()
        {
            // If already loaded in memory, return it
            if (_currentUser != null)
                return _currentUser;

            // Try to load from preferences
            var userId = Preferences.Get(CurrentUserIdKey, -1);
            if (userId > 0)
            {
                _currentUser = await _db.Connection
                    .Table<User>()
                    .FirstOrDefaultAsync(u => u.Id == userId);
            }

            return _currentUser;
        }

        // Check if user exists
        public async Task<bool> IsUserRegisteredAsync()
        {
            return await _db.Connection.Table<User>().CountAsync() > 0;
        }

        // Logout
        public void Logout()
        {
            _currentUser = null;
            Preferences.Remove(CurrentUserIdKey);
        }

        // Delete all users
        public async Task DeleteAllUsersAsync()
        {
            await _db.Connection.DeleteAllAsync<User>();
            _currentUser = null;
            Preferences.Remove(CurrentUserIdKey);
        }

        // Change password
        public async Task<bool> ChangePasswordAsync(string newPassword)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return false;
            
            user.PasswordHash = HashPassword(newPassword);
            await _db.Connection.UpdateAsync(user);
            _currentUser = user; // Update in-memory copy
            return true;
        }
    }
}