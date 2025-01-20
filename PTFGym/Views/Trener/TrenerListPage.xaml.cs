using PTFGym.ViewModels;
using PTFGym.Services;

namespace PTFGym.Views.Trener;

public partial class TrenerListPage : ContentPage
{
	public TrenerListPage()
	{
		InitializeComponent();
        BindingContext = new TrenerViewModel(Application.Current.MainPage.Handler.MauiContext.Services.GetService<LocalDbService>());
    }
}