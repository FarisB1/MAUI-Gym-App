using PTFGym.Services;
using PTFGym.ViewModels;

namespace PTFGym.Views.Napredak;

public partial class NapredakTrener : ContentPage
{
	public NapredakTrener()
	{
		InitializeComponent();
        BindingContext = new NapredakTrenerViewModel(Application.Current.MainPage.Handler.MauiContext.Services.GetService<LocalDbService>(), Application.Current.MainPage.Handler.MauiContext.Services.GetService<AuthenticationService>());
    }
}