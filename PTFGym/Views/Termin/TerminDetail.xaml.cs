using PTFGym.Models;
using PTFGym.ViewModels;

namespace PTFGym.Views.Termin
{
    public partial class TerminDetail : ContentPage
    {
        private readonly TerminDetailViewModel _viewModel;

        public TerminDetail(Models.Termin termin)
        {
            InitializeComponent();
            _viewModel = Application.Current.MainPage.Handler.MauiContext.Services.GetService<TerminDetailViewModel>();
            BindingContext = _viewModel;
            _viewModel.Termin = termin;
        }
    }
}