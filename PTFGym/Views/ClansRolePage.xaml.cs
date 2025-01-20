using PTFGym.ViewModels;
using Microsoft.Maui.Controls;
using PTFGym.Services;

namespace PTFGym.Views
{
    public partial class ClansRolePage : ContentPage
    {
        public ClansRolePage()
        {
            InitializeComponent();
            BindingContext = new ClansRoleViewModel(Application.Current.MainPage.Handler.MauiContext.Services.GetService<LocalDbService>());
        }
    }
}
