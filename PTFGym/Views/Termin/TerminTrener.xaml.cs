using PTFGym.Services;
using PTFGym.ViewModels;

namespace PTFGym.Views.Termin;

public partial class TerminTrener : ContentPage
{
    private readonly TerminViewModel _viewModel;
    private readonly AuthenticationService _authService;

    public TerminTrener()
    {
        InitializeComponent();
        var services = Application.Current.MainPage.Handler.MauiContext.Services;
        _authService = services.GetRequiredService<AuthenticationService>();
        _viewModel = new TerminViewModel(services.GetRequiredService<LocalDbService>());
        BindingContext = _viewModel;

        // Remove any automatic loading that might be in the ViewModel constructor
        this.Appearing += TerminTrener_Appearing;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadTrainerTermini();
    }

    private async void TerminTrener_Appearing(object sender, EventArgs e)
    {
        await LoadTrainerTermini();
    }

    private async Task LoadTrainerTermini()
    {
        try
        {
            var session = await _authService.GetUserSession();
            if (session?.UserId == null)
            {
                await Navigation.PopAsync();
                await DisplayAlert("Error", "Please log in to access trainer features.", "OK");
                return;
            }

            if (session.Role != "Trener")
            {
                await Navigation.PopAsync();
                await DisplayAlert("Error", "Access denied. Trainer privileges required.", "OK");
                return;
            }

            // Load only this trainer's termini
            await _viewModel.LoadTerminiForTrainerAsync((int)session.UserId);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}