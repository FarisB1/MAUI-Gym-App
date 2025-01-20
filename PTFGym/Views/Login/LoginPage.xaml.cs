using PTFGym.Authentication;
using PTFGym.Services;
using PTFGym.ViewModels;
using PTFGym.Views.Register;

namespace PTFGym.Views.Login;

[QueryProperty(nameof(Refresh), "refresh")]
public partial class LoginPage : ContentPage
{

   

    private string _refresh;
    public string Refresh
    {
        get => _refresh;
        set
        {
            _refresh = value;
            if (value == "true")
            {
                // Clear any existing login data
                var viewModel = (LoginViewModel)BindingContext;
                viewModel.Email = string.Empty;
                viewModel.Password = string.Empty;
                viewModel.ErrorMessage = string.Empty;
            }
        }
    }
    public LoginPage()
	{
		InitializeComponent();

        var authenticationService = Application.Current?.Handler?.MauiContext?.Services.GetService<AuthenticationService>();
        if (authenticationService == null)
        {
            DisplayAlert("Error", "Authentication service not initialized", "OK");
            return;
        }

        BindingContext = new LoginViewModel(authenticationService);
    } 
    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage());
    }
}