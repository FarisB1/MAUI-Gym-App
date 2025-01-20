using System;
using System.Windows.Input;
using PTFGym.Models;
using PTFGym.Services;
using Microsoft.Maui.Controls;
using PTFGym.Authentication;
using System.Xml.Linq;

namespace PTFGym.ViewModels
{
    public class RegisterViewModel : BindableObject
    {
        private readonly LocalDbService _dbService;
        private readonly AuthenticationService _authenticationService;
        public string _name { get; set; }
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }
        public string _email { get; set; }
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }
        public string _password { get; set; }
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand RegisterCommand { get; }

        public RegisterViewModel(LocalDbService dbService)
        {
            Console.WriteLine("RegisterViewModel constructor start");

            if (dbService == null)
            {
                Console.WriteLine("dbService is null!");
                throw new ArgumentNullException(nameof(dbService));
            }

            _dbService = dbService;
            Console.WriteLine("dbService assigned");

            _authenticationService = new AuthenticationService(dbService);
            Console.WriteLine("authenticationService created");

            RegisterCommand = new Command(async () => await RegisterAsync());
            Console.WriteLine("RegisterViewModel constructor complete");
        }

        private async Task RegisterAsync()
        {
            try
            {
                Console.WriteLine("RegisterAsync started");
                ErrorMessage = string.Empty;
                if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Please fill in all the fields.";
                    return;
                }
                if (_dbService == null)
                {
                    Console.WriteLine("_dbService is null!");
                    ErrorMessage = "Database service not initialized";
                    return;
                }
                Console.WriteLine($"Checking existing user for email: {Email}");
                var existingUser = await _dbService.GetClanByEmailAsync(Email);
                if (existingUser != null)
                {
                    ErrorMessage = "Email is already taken.";
                    return;
                }
                if (_authenticationService == null)
                {
                    Console.WriteLine("_authenticationService is null!");
                    ErrorMessage = "Authentication service not initialized";
                    return;
                }
                Console.WriteLine("Hashing password");
                var hashedPassword = _authenticationService.HashPassword(Password);
                Console.WriteLine("Creating new Clan object");
                var newClan = new Clan
                {
                    Ime = Name,
                    Email = Email,
                    PasswordHash = hashedPassword,
                    Role = "User",
                    DatumPocetkaClanstva = DateTime.Now,
                    DatumKrajaClanstva = DateTime.Now.AddMonths(1)
                };
                Console.WriteLine("About to save to database");
                if (newClan == null)
                {
                    Console.WriteLine("newClan is null!");
                    return;
                }
                try
                {
                    var result = await _dbService.SaveClan(newClan);
                    Console.WriteLine($"Save result: {result}");

                    var clanarina = new Clanarina
                    {
                        ClanId = newClan.Id,
                        DatumPocetka = DateTime.Now,
                        DatumZavrsetka = DateTime.Now.AddMonths(1),
                        Iznos = 50,
                    };

                    Console.WriteLine("Saving initial Clanarina");
                    await _dbService.SaveClanarina(clanarina);
                    Console.WriteLine("Successfully saved Clanarina");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving to database: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    throw;
                }
                Console.WriteLine("Successfully saved to database");
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    try
                    {
                        Console.WriteLine("Attempting navigation to login page");
                        await Shell.Current.GoToAsync("//LoginPage");
                        Console.WriteLine("Navigation completed successfully");
                    }
                    catch (Exception navEx)
                    {
                        Console.WriteLine($"Navigation error: {navEx.Message}");
                        Console.WriteLine($"Navigation stack trace: {navEx.StackTrace}");
                        ErrorMessage = "Failed to navigate to login page";
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RegisterAsync: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                ErrorMessage = $"Registration failed: {ex.Message}";
            }
        }
    }
}
