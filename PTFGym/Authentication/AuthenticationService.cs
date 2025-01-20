using PTFGym.Models;
using PTFGym.Services;
using System.Text.Json;
using Microsoft.Maui.Storage;

public class AuthenticationService
{
    private const string USER_SESSION_KEY = "current_user_session";
    private readonly LocalDbService _dbService;

    public AuthenticationService(LocalDbService dbService)
    {
        Console.WriteLine("AuthenticationService constructor start");
        _dbService = dbService ?? throw new ArgumentNullException(nameof(dbService));
        Console.WriteLine("AuthenticationService constructor complete");
    }

    // Method to retrieve the user session
    public async Task<UserSession> GetUserSession()
    {
        Console.WriteLine("[GetUserSession] Attempting to retrieve session from preferences");

        try
        {
            string? sessionJson = Preferences.Get(USER_SESSION_KEY, null);

            if (string.IsNullOrEmpty(sessionJson))
            {
                Console.WriteLine("[GetUserSession] No session found, returning default guest session");
                var defaultSession = new UserSession(0, "Guest", "Guest");
                await SaveUserSession(defaultSession);
                return defaultSession;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var session = JsonSerializer.Deserialize<UserSession>(sessionJson, options);

            if (session == null)
            {
                Console.WriteLine("[GetUserSession] Deserialization returned null, returning default guest session");
                var defaultSession = new UserSession(0, "Guest", "Guest");
                await SaveUserSession(defaultSession);
                return defaultSession;
            }

            Console.WriteLine($"[GetUserSession] Successfully retrieved session for user: {session.UserName}");
            return session;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[GetUserSession] JSON deserialization error: {ex.Message}");
            // Return a default guest session if there's a JSON error
            var defaultSession = new UserSession(0, "Guest", "Guest");
            await SaveUserSession(defaultSession);
            return defaultSession;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GetUserSession] Unexpected error: {ex.Message}");
            // Handle other potential exceptions
            return new UserSession(0, "Guest", "Guest");
        }
    }

    // Method to save the user session
    public async Task SaveUserSession(UserSession userSession)
    {
        Console.WriteLine($"[SaveUserSession] Saving session for user: {userSession.UserName}");

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        string sessionJson = JsonSerializer.Serialize(userSession, options);
        Preferences.Set(USER_SESSION_KEY, sessionJson);
    }

    // Method to remove the user session
    public void RemoveUserSession()
    {
        try
        {
            Console.WriteLine("[RemoveUserSession] Removing user session");
            Preferences.Remove(USER_SESSION_KEY);
            Console.WriteLine("[RemoveUserSession] Session removed successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[RemoveUserSession] Error removing session: {ex.Message}");
            throw;
        }
    }

    // Method to get the role of the current user
    public async Task<string> GetUserRoleAsync()
    {
        Console.WriteLine("[GetUserRoleAsync] Getting user role");
        var userSession = await GetUserSession();
        return userSession?.Role ?? "Guest";
    }

    // Method to check if the user has a specific role
    public async Task<bool> IsUserInRoleAsync(string role)
    {
        Console.WriteLine($"[IsUserInRoleAsync] Checking if user is in role: {role}");
        var userRole = await GetUserRoleAsync();
        return userRole != null && userRole.Equals(role, StringComparison.OrdinalIgnoreCase);
    }

    // Method to load the user session
    public async Task LoadUserSession()
    {
        var userSession = await GetUserSession();
        if (userSession != null && !string.IsNullOrEmpty(userSession.UserName))
        {
            // If session is valid, show user info
            Console.WriteLine($"User session loaded: {userSession.UserName}");
        }
        else
        {
            // If session is invalid or missing, log out the user
            Console.WriteLine("User session is invalid, logging out");
        }
    }

    // Method to hash the password
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    // Method to verify the password
    public bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }

    // Method to register a new clan (user)
    public async Task RegisterClanAsync(string ime, string email, string password)
    {
        var hashedPassword = HashPassword(password);

        var newClan = new Clan
        {
            Ime = ime,
            Email = email,
            PasswordHash = hashedPassword,
            DatumPocetkaClanstva = DateTime.Now,
            DatumKrajaClanstva = DateTime.Now.AddMonths(1)
        };

        await _dbService.SaveItemAsync(newClan);
    }

    // Method to log in a clan (user)
    public async Task<Clan?> LoginAsync(string email, string password)
    {
        var clan = await _dbService.GetClanByEmailAsync(email);
        if (clan != null && VerifyPassword(password, clan.PasswordHash))
        {
            // Create and save a new session when user logs in
            var userSession = new UserSession(clan.Id, clan.Ime, clan.Role ?? "User");
            await SaveUserSession(userSession);
            return clan;
        }
        return null;
    }

    // Method to check if an email is unique for registration
    public async Task<bool> IsEmailUniqueAsync(string email)
    {
        var existingClan = await _dbService.GetClanByEmailAsync(email);
        return existingClan == null;
    }
}
