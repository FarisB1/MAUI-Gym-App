using PTFGym.Services;
using PTFGym.ViewModels;
using PTFGym.Views.Login;

namespace PTFGym.Views.Register;

public partial class RegisterPage : ContentPage
{
    public Command NavigateToLoginCommand => new Command(async () =>
    await Shell.Current.GoToAsync("//LoginPage"));
    public RegisterPage()
    {
        InitializeComponent();

        var dbService = Application.Current?.Handler?.MauiContext?.Services.GetService<LocalDbService>();
        if (dbService == null)
        {
            DisplayAlert("Error", "Database service not initialized", "OK");
            return;
        }

        BindingContext = new RegisterViewModel(dbService);
    }


    private async void OnLoginClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LoginPage());
    }
}