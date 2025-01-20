using PTFGym.ViewModels;
using PTFGym.Services;

namespace PTFGym.Views.Termin;

public partial class TerminListPage : ContentPage
{
	public TerminListPage()
	{
		InitializeComponent();
        BindingContext = new TerminViewModel(Application.Current.MainPage.Handler.MauiContext.Services.GetService<LocalDbService>());
    }
}