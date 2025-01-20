using PTFGym.Views.Login;
using PTFGym.Views.Register;

namespace PTFGym.Views;

public partial class RestrictedPage : ContentPage
{
    public RestrictedPage()
    {
        InitializeComponent();
        BindingContext = new RestrictedPageViewModel();
    }
}

public class RestrictedPageViewModel
{
    public Command NavigateToLoginCommand { get; }
    public Command NavigateToRegisterCommand { get; }

    public RestrictedPageViewModel()
    {
        NavigateToLoginCommand = new Command(async () => await NavigateToLogin());
        NavigateToRegisterCommand = new Command(async () => await NavigateToRegister());
    }

    private async Task NavigateToLogin()
    {
        await Shell.Current.GoToAsync(nameof(LoginPage));
    }

    private async Task NavigateToRegister()
    {
        await Shell.Current.GoToAsync(nameof(RegisterPage));
    }
}