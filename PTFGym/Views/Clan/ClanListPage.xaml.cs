using PTFGym.ViewModels;
using PTFGym.Services;

namespace PTFGym.Views.Clan;

public partial class ClanListPage : ContentPage
{
    public ClanListPage()
    {
        InitializeComponent();
        BindingContext = new ClanViewModel(Application.Current.MainPage.Handler.MauiContext.Services.GetService<LocalDbService>());
         }

    // Override OnAppearing to reload the data whenever the page appears
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Call LoadClanoviAsync to refresh the list of Clanovi (members)
        var viewModel = (ClanViewModel)BindingContext;
        await viewModel.LoadClanoviAsync();
    }
}