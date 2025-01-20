using PTFGym.ViewModels;
using PTFGym.Services;

namespace PTFGym.Views.Napredak;

public partial class NapredakListPage : ContentPage
{
	public NapredakListPage()
	{
		InitializeComponent();
        BindingContext = new NapredakViewModel(Application.Current.MainPage.Handler.MauiContext.Services.GetService<LocalDbService>());
    }
}