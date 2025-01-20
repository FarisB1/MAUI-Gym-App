using PTFGym.Services;
using PTFGym.ViewModels;

namespace PTFGym.Views.Napredak;

public partial class NapredakClan : ContentPage
{
	public NapredakClan()
	{
		InitializeComponent(); 
		BindingContext = new NapredakClanViewModel(Application.Current.MainPage.Handler.MauiContext.Services.GetService<LocalDbService>(), Application.Current.MainPage.Handler.MauiContext.Services.GetService<AuthenticationService>());

    }
}