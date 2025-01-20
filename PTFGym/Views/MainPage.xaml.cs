using PTFGym.Services;
using PTFGym.Views.Clan;
using PTFGym.Views.Clanarina;
using PTFGym.Views.Napredak;
using PTFGym.Views.Register;
using PTFGym.Views.Rezervacija;
using PTFGym.Views.Termin;
using PTFGym.Views.Trener;

namespace PTFGym.Views;

public partial class MainPage : ContentPage
{
    private readonly AuthenticationService authenticationService;
	public MainPage()
	{
        authenticationService = Application.Current.MainPage.Handler.MauiContext.Services.GetService<AuthenticationService>();

        InitializeComponent();
	}

    private async void OnTerminiClicked(object sender, EventArgs e)
    {
        var role = await authenticationService.GetUserRoleAsync();
        if (string.Equals(role, "User", StringComparison.OrdinalIgnoreCase))
        {
            await Navigation.PushAsync(new TerminClan());
        }
        if (string.Equals(role, "Trener", StringComparison.OrdinalIgnoreCase))
        {
            await Navigation.PushAsync(new TerminTrener());
        }
    }
    private async void OnNapredakClicked(object sender, EventArgs e)
    {
        var role = await authenticationService.GetUserRoleAsync();
        if (string.Equals(role, "User", StringComparison.OrdinalIgnoreCase))
        {
            await Navigation.PushAsync(new NapredakClan());
        }
        if (string.Equals(role, "Trener", StringComparison.OrdinalIgnoreCase))
        {
            await Navigation.PushAsync(new NapredakTrener());
        }
    }
    private async void OnRezervacijeClicked(object sender, EventArgs e)
    {
        var role = await authenticationService.GetUserRoleAsync();
        if (string.Equals(role, "User", StringComparison.OrdinalIgnoreCase))
        {
            await Navigation.PushAsync(new RezervacijaClan());
        }
        if (string.Equals(role, "Trener", StringComparison.OrdinalIgnoreCase))
        {
            await Navigation.PushAsync(new RezervacijaTrener());
        }
    }
    private async void OnClanarinaClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ClanarinaClan(Application.Current.MainPage.Handler.MauiContext.Services.GetService<LocalDbService>(), Application.Current.MainPage.Handler.MauiContext.Services.GetService<AuthenticationService>()));
    }




}