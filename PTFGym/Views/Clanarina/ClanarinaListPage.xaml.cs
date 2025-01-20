using PTFGym.ViewModels;
using PTFGym.Services;

namespace PTFGym.Views.Clanarina;

public partial class ClanarinaListPage : ContentPage
{
	public ClanarinaListPage()
	{
        InitializeComponent();
        BindingContext = new ClanarinaViewModel(Application.Current.MainPage.Handler.MauiContext.Services.GetService<LocalDbService>());

    }
}