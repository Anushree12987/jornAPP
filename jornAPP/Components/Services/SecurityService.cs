using System.Security.Cryptography;
using System.Text;
using jornAPP.Components.Data;
using jornAPP.Components.Models;

namespace jornAPP.Services
{
    /// <summary>
    /// Handles user authentication, registration, password management, and current session.
    /// </summary>
    public class SecurityService
    {
        private readonly AppDatabase _db; // Reference to the app database
        private User _currentUser;        // Stores the currently logged-in user in memory
        private const string CurrentUserIdKey = "CurrentUserId"; // Key for storing current user ID in preferences

        /// <summary>
        /// Constructor to initialize the database dependency.
        /// </summary>
        public SecurityService(AppDatabase db)
        {
            _db = db;
        }

        /// <summary>
        /// Hashes a password using SHA256 and returns a Base64 string.
        /// </summary>
        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Registers a new user with a username and password.
        /// </summary>
        /// <param name="username">Username for new account.</param>
        /// <param name="password">Password for new account.</param>
        /// <returns>True if registration succeeds; false if username already exists.</returns>
        public async Task<bool> RegisterAsync(string username, string password)
        {
            // Check if username already exists
            var existing = await _db.Connection
                .Table<User>()
                .FirstOrDefaultAsync(u => u.Username == username);

            if (existing != null)
                return false;

            // Create new user with hashed password
            var user = new User
            {
                Username = username,
                PasswordHash = HashPassword(password),
                CreatedAt = DateTime.Now
            };

            await _db.Connection.InsertAsync(user);

            // Set as current user in memory and preferences
            _currentUser = user;
            Preferences.Set(CurrentUserIdKey, user.Id);

            return true;
        }

        /// <summary>
        /// Login (unlock) using username and password.
        /// </summary>
        /// <param name="username">Username of account.</param>
        /// <param name="password">Password of account.</param>
        /// <returns>True if login succeeds; otherwise false.</returns>
        public async Task<bool> LoginAsync(string username, string password)
        {
            // Find user by username
            var user = await _db.Connection
                .Table<User>()
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return false;

            // Compare password hash
            bool valid = user.PasswordHash == HashPassword(password);
            if (valid)
            {
                _currentUser = user;
                Preferences.Set(CurrentUserIdKey, user.Id);
            }

            return valid;
        }

        /// <summary>
        /// Gets the currently logged-in user.
        /// </summary>
        /// <returns>User object if logged in; otherwise null.</returns>
        public async Task<User> GetCurrentUserAsync()
        {
            // Return in-memory user if already loaded
            if (_currentUser != null)
                return _currentUser;

            // Try to load user ID from preferences
            var userId = Preferences.Get(CurrentUserIdKey, -1);
            if (userId > 0)
            {
                _currentUser = await _db.Connection
                    .Table<User>()
                    .FirstOrDefaultAsync(u => u.Id == userId);
            }

            return _currentUser;
        }

        /// <summary>
        /// Checks if any user is registered in the system.
        /// </summary>
        /// <returns>True if at least one user exists.</returns>
        public async Task<bool> IsUserRegisteredAsync()
        {
            return await _db.Connection.Table<User>().CountAsync() > 0;
        }

        /// <summary>
        /// Logout the current user and clear preferences.
        /// </summary>
        public void Logout()
        {
            _currentUser = null;
            Preferences.Remove(CurrentUserIdKey);
        }

        /// <summary>
        /// Delete all users from the database.
        /// </summary>
        public async Task DeleteAllUsersAsync()
        {
            await _db.Connection.DeleteAllAsync<User>();
            _currentUser = null;
            Preferences.Remove(CurrentUserIdKey);
        }

        /// <summary>
        /// Change the password for the currently logged-in user.
        /// </summary>
        /// <param name="newPassword">New password.</param>
        /// <returns>True if successful; false if no user logged in.</returns>
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
