using System.Windows.Input;
using Microsoft.Maui.Controls;
using PTFGym.Models;
using PTFGym.Services;
using PTFGym.Authentication;

namespace PTFGym.ViewModels
{
    public class LoginViewModel : BindableObject
    {
        private readonly AuthenticationService _authenticationService;
        private string _email;
        private string _password;
        private string _errorMessage;

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel(AuthenticationService authenticationService)
        {
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            LoginCommand = new Command(async () => await LoginAsync());
        }

        private async Task LoginAsync()
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter both email and password.";
                return;
            }

            var clan = await _authenticationService.LoginAsync(Email, Password);

            if (clan != null)
            {
                var userSession = new UserSession(clan.Id, clan.Ime, clan.Role);
                await _authenticationService.SaveUserSession(userSession);

                await Shell.Current.GoToAsync($"MainPage");
                
            }
            else
            {
                ErrorMessage = "Invalid email or password.";
            }
        }
    }
}
