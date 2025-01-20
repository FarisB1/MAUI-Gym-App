using PTFGym.Authentication;
using PTFGym.Models;
using PTFGym.Services;
using System.ComponentModel;
using System.Windows.Input;
using System.Runtime.CompilerServices;
namespace PTFGym.Views;

public partial class ProfilePage : ContentPage, INotifyPropertyChanged
{
    private readonly AuthenticationService _authenticationService;
    private string _userName;
    private string _email;
    private string _role;
    private DateTime _datumPocetkaClanstva;
    private DateTime _datumKrajaClanstva;
    private int _userId;
    private bool _isUserLoggedIn;

    public string UserName
    {
        get => _userName;
        set
        {
            _userName = value;
            OnPropertyChanged();
        }
    }

    public string Email
    {
        get => _email;
        set
        {
            _email = value;
            OnPropertyChanged();
        }
    }

    public string Role
    {
        get => _role;
        set
        {
            _role = value;
            OnPropertyChanged();
        }
    }

    public DateTime DatumPocetkaClanstva
    {
        get => _datumPocetkaClanstva;
        set
        {
            _datumPocetkaClanstva = value;
            OnPropertyChanged();
        }
    }

    public DateTime DatumKrajaClanstva
    {
        get => _datumKrajaClanstva;
        set
        {
            _datumKrajaClanstva = value;
            OnPropertyChanged();
        }
    }

    public int UserId
    {
        get => _userId;
        set
        {
            _userId = value;
            OnPropertyChanged();
        }
    }

    public bool IsUserLoggedIn
    {
        get => _isUserLoggedIn;
        set
        {
            _isUserLoggedIn = value;
            OnPropertyChanged();
        }
    }

    public ProfilePage()
    {
        InitializeComponent();
        _authenticationService = Application.Current?.Handler?.MauiContext?.Services.GetService<AuthenticationService>();
        
        if (_authenticationService == null)
        {
            DisplayAlert("Error", "Authentication service not initialized", "OK");
            return;
        }

        BindingContext = this;
        LoadUserData();
    }

    private async void LoadUserData()
    {
        var _dbService = Application.Current?.Handler?.MauiContext?.Services.GetService<LocalDbService>();
        if (_dbService == null)
        {
            Console.WriteLine("[LoadUserData] Database service not initialized.");
            await DisplayAlert("Error", "Database service not initialized", "OK");
            return;
        }

        try
        {
            var session = await _authenticationService.GetUserSession();
            if (session == null)
            {
                Console.WriteLine("[LoadUserData] No session found.");
                await DisplayAlert("Error", "No active session", "OK");
                return;
            }

            UserName = session.UserName;
            Role = session.Role;
            UserId = (int)session.UserId;
            IsUserLoggedIn = true;

            Console.WriteLine($"[LoadUserData] User session loaded: UserId={UserId}, Role={Role}");

            var user = await _dbService.GetItemByIdAsync<PTFGym.Models.Clan>((int)session.UserId);
            if (user == null)
            {
                Console.WriteLine($"[LoadUserData] No user found with ID {UserId}.");
                Email = "Unknown";
                DatumPocetkaClanstva = DateTime.Today;
                DatumKrajaClanstva = DateTime.Today;
                return;
            }

            Email = user.Email;
            Console.WriteLine($"[LoadUserData] User email: {Email}");

            var clanarine = await _dbService.GetItemsAsync<PTFGym.Models.Clanarina>();
            if (clanarine == null || !clanarine.Any())
            {
                Console.WriteLine("[LoadUserData] No memberships found.");
                DatumPocetkaClanstva = DateTime.Today;
                DatumKrajaClanstva = DateTime.Today;
                return;
            }

            var clanarina = clanarine.FirstOrDefault(c => c.ClanId == user.Id);
            if (clanarina != null)
            {
                DatumPocetkaClanstva = clanarina.DatumPocetka;
                DatumKrajaClanstva = clanarina.DatumZavrsetka;
                Console.WriteLine($"[LoadUserData] Membership dates: Start={DatumPocetkaClanstva}, End={DatumKrajaClanstva}");
            }
            else
            {
                Console.WriteLine("[LoadUserData] No matching membership found for this user.");
                DatumPocetkaClanstva = DateTime.Today;
                DatumKrajaClanstva = DateTime.Today;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LoadUserData] Error: {ex.Message}");
            await DisplayAlert("Error", "Failed to load user data", "OK");
        }
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        try
        {
            Console.WriteLine("Start onLogoutClicked");

            // Remove the user session
            _authenticationService.RemoveUserSession();
            Console.WriteLine("Removed session");

            // Clear displayed user info
            UserName = string.Empty;
            Role = string.Empty;
            UserId = 0;
            IsUserLoggedIn = false;

            // Refresh bindings
            OnPropertyChanged(nameof(UserName));
            OnPropertyChanged(nameof(Role));
            OnPropertyChanged(nameof(IsUserLoggedIn));


            await Shell.Current.GoToAsync("LoginPage", true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during logout: {ex.Message}");
            await DisplayAlert("Error", "An error occurred during logout", "OK");
        }
    }

    // You'll need to implement this method in your AuthenticationService
    // Add this method to your AuthenticationService
    public async Task<PTFGym.Models.Clan?> LoadUserDetails(int userId)
    {
        var _dbService = Application.Current?.Handler?.MauiContext?.Services.GetService<LocalDbService>();
        try
        {
            // Fetch the clan/user details from the database by ID
            var user = await _dbService.GetItemByIdAsync<PTFGym.Models.Clan>(userId);

            if (user == null)
            {
                Console.WriteLine($"[LoadUserDetails] No user found with ID: {userId}");
            }
            else
            {
                Console.WriteLine($"[LoadUserDetails] Successfully loaded user details for ID: {userId}");
            }

            return user;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LoadUserDetails] Error loading user details: {ex.Message}");
            return null;
        }
    }

}