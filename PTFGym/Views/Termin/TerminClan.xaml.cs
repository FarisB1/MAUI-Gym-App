using PTFGym.Services;
using PTFGym.ViewModels;

namespace PTFGym.Views.Termin;

public partial class TerminClan : ContentPage
{
    private readonly TerminViewModel _viewModel;
    private readonly AuthenticationService _authService;

    public TerminClan()
    {
        InitializeComponent();
        var services = Application.Current.MainPage.Handler.MauiContext.Services;
        _authService = services.GetRequiredService<AuthenticationService>();
        _viewModel = new TerminViewModel(services.GetRequiredService<LocalDbService>());
        BindingContext = _viewModel;

        this.Appearing += TerminClan_Appearing;
    }

    private async void TerminClan_Appearing(object sender, EventArgs e)
    {
        await LoadAllTermini();
    }

    private async Task LoadAllTermini()
    {
        try
        {
            var session = await _authService.GetUserSession();
            if (session?.UserId == null)
            {
                await Navigation.PopAsync();
                await DisplayAlert("Error", "Please log in to view training sessions.", "OK");
                return;
            }

            // Load all available termini for members
            await _viewModel.LoadTerminiAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}