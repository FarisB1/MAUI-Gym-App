using PTFGym.ViewModels;
using PTFGym.Services;

namespace PTFGym.Views.Rezervacija;

public partial class RezervacijaListPage : ContentPage
{
	public RezervacijaListPage()
	{
		InitializeComponent();
        BindingContext = new RezervacijaViewModel(Application.Current.MainPage.Handler.MauiContext.Services.GetService<LocalDbService>());
    }
}