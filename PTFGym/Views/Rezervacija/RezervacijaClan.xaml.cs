using PTFGym.Services;
using PTFGym.ViewModels;

namespace PTFGym.Views.Rezervacija;

public partial class RezervacijaClan : ContentPage
{
	public RezervacijaClan()
	{
		InitializeComponent();
        BindingContext = new RezervacijaViewModel(Application.Current.MainPage.Handler.MauiContext.Services.GetService<LocalDbService>());
    }
}