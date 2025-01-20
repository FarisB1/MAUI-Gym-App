using PTFGym.Services;
using PTFGym.ViewModels;

namespace PTFGym.Views.Clanarina;

public partial class ClanarinaClan : ContentPage
{
    private readonly ClanarinaClanViewModel _viewModel;


    public ClanarinaClan(LocalDbService dbService, AuthenticationService authService)
    {
        InitializeComponent();
        _viewModel = new ClanarinaClanViewModel(dbService, authService);
        BindingContext = _viewModel;
    }
}