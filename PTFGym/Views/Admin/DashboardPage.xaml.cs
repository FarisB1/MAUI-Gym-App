using PTFGym.Views.Clan;
using PTFGym.Views.Trener;
using PTFGym.Views.Termin;
using PTFGym.Views.Clanarina;
using PTFGym.Views.Napredak;
using PTFGym.Views.Rezervacija;
using PTFGym.Views.Register;
using PTFGym.Authentication;
using PTFGym.Views.Login;
using PTFGym.ViewModels;
using PTFGym.Views.Admin;
using PTFGym.Services;

namespace PTFGym.Views.Admin
{
    [QueryProperty(nameof(Refresh), "refresh")]
    public partial class DashboardPage : ContentPage
    {
        private readonly AuthenticationService _authenticationService;
        private readonly ClansRoleViewModel clansRoleViewModel;
        private readonly LocalDbService _dbService;
        private string _refresh;
        private string _userName;
        private string _role;
        private int _userId;

        public string Refresh
        {
            get => _refresh;
            set
            {
                _refresh = value;
                if (value == "true")
                {
                    LoadUserSession(); // Reload session when the "refresh" query property is set
                }
            }
        }

        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged(nameof(UserName));
            }
        }
        
        public int UserId
        {
            get => _userId;
            set
            {
                _userId = value;
                OnPropertyChanged(nameof(UserId));
            }
        }

        public string Role
        {
            get => _role;
            set
            {
                _role = value;
                OnPropertyChanged(nameof(Role));
            }
        }

        public bool IsUserLoggedIn { get; set; }

        // Constructor to inject the AuthenticationService
        public DashboardPage(AuthenticationService authenticationService)
        {
            InitializeComponent();
            Console.WriteLine("DashboardPage constructor called");
            _authenticationService = authenticationService;
            BindingContext = this;
            LoadUserSession(); // Load user session when page is created
        }

        // Load the user session and update UI accordingly
        private async void LoadUserSession()
        {
            var userSession = await _authenticationService.GetUserSession();
            if (userSession != null && !string.IsNullOrEmpty(userSession.UserName))
            {
                // If session is valid, show user info
                UserName = $"{userSession.UserName}";
                Role = $"Role: {userSession.Role}";
                UserId = userSession.UserId.Value;
                IsUserLoggedIn = true;
            }
            else
            {
                // If session is invalid or missing, log out the user
                IsUserLoggedIn = false;
                await Shell.Current.GoToAsync("LoginPage"); // Redirect to login page
            }

            OnPropertyChanged(nameof(IsUserLoggedIn));
        }

        // Logout method
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

                // Navigate directly to LoginPage
                await Shell.Current.GoToAsync("RestrictedPage");
                await Shell.Current.GoToAsync("LoginPage");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during logout: {ex.Message}");
                await DisplayAlert("Error", "An error occurred during logout", "OK");
            }
        }

        // Navigate to different pages when buttons are clicked
        private async void OnMembersClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ClanListPage());
        }



        private async void OnAddTrenerClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ClansRolePage());
        }

        private async void OnTrainersClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TrenerListPage());
        }

        private async void OnSessionsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TerminListPage());
        }

        private async void OnMembershipsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ClanarinaListPage());
        }

        private async void OnProgressClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NapredakListPage());
        }

        private async void OnReservationsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RezervacijaListPage());
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegisterPage());
        }
    }
}
