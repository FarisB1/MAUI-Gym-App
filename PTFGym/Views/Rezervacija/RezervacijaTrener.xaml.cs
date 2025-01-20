using PTFGym.Models;
using PTFGym.Services;
using PTFGym.ViewModels;

namespace PTFGym.Views.Rezervacija;

public partial class RezervacijaTrener : ContentPage
{
	public RezervacijaTrener()
	{
		InitializeComponent();
        BindingContext = new RezervacijaViewModel(Application.Current.MainPage.Handler.MauiContext.Services.GetService<LocalDbService>());
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var viewModel = BindingContext as RezervacijaViewModel;
        await viewModel.LoadTreneroveRezervacijeAsync();
    }
    
}
