using PTFGym.Authentication;
using PTFGym.Views;
using PTFGym.Views.Login;
using PTFGym.Views.Admin;
using PTFGym.Views.Register;

namespace PTFGym
{
    public partial class AppShell : Shell
    {
        private AuthenticationService _authenticationService;

        public AppShell()
        {
            InitializeComponent();

            // Register routes
            Routing.RegisterRoute("MainPage", typeof(MainPage));
            Routing.RegisterRoute("LoginPage", typeof(LoginPage));
            Routing.RegisterRoute("RegisterPage", typeof(RegisterPage));
            Routing.RegisterRoute("DashboardPage", typeof(DashboardPage));
            Routing.RegisterRoute("RestrictedPage", typeof(RestrictedPage));

            Console.WriteLine("AppShell initialized.");

            // Subscribe to the Handler changed event
            this.HandlerChanged += OnHandlerChanged;

            // Subscribe to navigation events
            Navigated += OnNavigated;
        }

        private void OnHandlerChanged(object sender, EventArgs e)
        {
            if (Handler != null)
            {
                InitializeServices();
                this.HandlerChanged -= OnHandlerChanged;
            }
        }

        private void OnNavigated(object sender, ShellNavigatedEventArgs e)
        {
            Console.WriteLine($"Navigation occurred to {e.Current?.Location}");
            if (_authenticationService != null)
            {
                _ = UpdateNavigationVisibility();
            }
        }

        private async void InitializeServices()
        {
            try
            {
                var services = Handler.MauiContext.Services;
                _authenticationService = services.GetService<AuthenticationService>();
                if (_authenticationService != null)
                {
                    await CheckUserSessionAndNavigate();
                    await UpdateNavigationVisibility();
                }
                else
                {
                    Console.WriteLine("Warning: AuthenticationService could not be resolved");
                    RestrictedItem.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing services: {ex.Message}");
                RestrictedItem.IsVisible = false;
            }
        }

        private async Task CheckUserSessionAndNavigate()
        {
            try
            {
                var role = await _authenticationService.GetUserRoleAsync();
                Console.WriteLine($"Checking user role on startup: {role}");

                if (role.Equals("Guest", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("User is guest, navigating to restricted page");
                    Current.GoToAsync("RestrictedPage");
                }
                else
                {
                    Console.WriteLine($"User is {role}, navigating to main page");
                    Current.GoToAsync("MainPage");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during session check: {ex.Message}");
                Current.GoToAsync("RestrictedPage");
            }
        }

        private async Task UpdateNavigationVisibility()
        {
            try
            {
                var role = await _authenticationService.GetUserRoleAsync();
                Console.WriteLine($"Current user role: {role}");

                // Update navigation items visibility based on role
                RestrictedItem.IsVisible = role.Equals("Guest", StringComparison.OrdinalIgnoreCase);
                LoginItem.IsVisible = role.Equals("Guest", StringComparison.OrdinalIgnoreCase);
                RegisterItem.IsVisible = role.Equals("Guest", StringComparison.OrdinalIgnoreCase);
                AddTrenerItem.IsVisible = role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
                MainItem.IsVisible = !role.Equals("Guest", StringComparison.OrdinalIgnoreCase);
                TerminClanItem.IsVisible = role.Equals("User", StringComparison.OrdinalIgnoreCase);
                TerminTrenerItem.IsVisible = role.Equals("Trener", StringComparison.OrdinalIgnoreCase);
                RezervacijaClanItem.IsVisible = role.Equals("User", StringComparison.OrdinalIgnoreCase);
                RezervacijaTrenerItem.IsVisible = role.Equals("Trener", StringComparison.OrdinalIgnoreCase);
                NapredakClanItem.IsVisible = role.Equals("User", StringComparison.OrdinalIgnoreCase);
                NapredakTrenerItem.IsVisible = role.Equals("Trener", StringComparison.OrdinalIgnoreCase);
                ClanarinaClanItem.IsVisible = role.Equals("Trener", StringComparison.OrdinalIgnoreCase) || role.Equals("User", StringComparison.OrdinalIgnoreCase);
                DashboardItem.IsVisible = role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
                ProfilItem.IsVisible = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating navigation visibility: {ex.Message}");
                RestrictedItem.IsVisible = true;
            }
        }
    }
}
/*
        private async Task UpdateNavigationVisibility()
        {
            try
            {
                var role = await _authenticationService.GetUserRoleAsync();
                Console.WriteLine($"Current user role: {role}");

                // Update navigation items visibility based on role
                RestrictedItem.IsVisible = false;
                DobrodosliItem.IsVisible = false;
                LoginItem.IsVisible = role.Equals("Guest", StringComparison.OrdinalIgnoreCase);
                RegisterItem.IsVisible = role.Equals("Guest", StringComparison.OrdinalIgnoreCase);
                AddTrenerItem.IsVisible = role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
                MainItem.IsVisible = !role.Equals("Guest", StringComparison.OrdinalIgnoreCase);
                TerminClanItem.IsVisible = role.Equals("User", StringComparison.OrdinalIgnoreCase);
                TerminTrenerItem.IsVisible = role.Equals("Trener", StringComparison.OrdinalIgnoreCase);
                RezervacijaClanItem.IsVisible = role.Equals("User", StringComparison.OrdinalIgnoreCase);
                RezervacijaTrenerItem.IsVisible = role.Equals("Trener", StringComparison.OrdinalIgnoreCase);
                NapredakClanItem.IsVisible = role.Equals("User", StringComparison.OrdinalIgnoreCase);
                NapredakTrenerItem.IsVisible = role.Equals("Trener", StringComparison.OrdinalIgnoreCase);
                ClanarinaClanItem.IsVisible = role.Equals("Trener", StringComparison.OrdinalIgnoreCase) || role.Equals("User", StringComparison.OrdinalIgnoreCase);
                DashboardItem.IsVisible = true;
                //DashboardItem.IsVisible = role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
                ProfilItem.IsVisible = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating navigation visibility: {ex.Message}");
                RestrictedItem.IsVisible = true;
            }
        }*/