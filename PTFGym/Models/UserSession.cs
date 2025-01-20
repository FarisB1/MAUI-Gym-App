namespace PTFGym.Models
{
    public class UserSession
    {
        public int? UserId { get; set; } // Added UserId as an integer (or nullable int if the user might not be logged in)
        public string? UserName { get; set; }
        public string? Role { get; set; }

        public UserSession()
        {
            UserId = 0; // Default to null if no user is logged in
            UserName = "Guest";
            Role = "Guest";
        }

        public UserSession(int userId, string username, string role)
        {
            UserId = userId;
            UserName = username ?? "Guest";
            Role = role ?? "Guest";
        }
    }
}
