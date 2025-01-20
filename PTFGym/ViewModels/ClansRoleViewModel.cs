using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;
using PTFGym.Models;
using PTFGym.Services;

namespace PTFGym.ViewModels
{
    public class ClansRoleViewModel : BindableObject
    {
        private readonly LocalDbService _dbService;

        public ObservableCollection<Clan> ClanList { get; } = new();
        public ObservableCollection<string> Roles { get; } = new ObservableCollection<string>
        {
            "User", "Admin"
        };

        private Clan _selectedClan;
        public Clan SelectedClan
        {
            get => _selectedClan;
            set
            {
                _selectedClan = value;
                OnPropertyChanged(nameof(DisplaySelectedRole));
                UpdateSelectedRole();
            }
        }

        private string _selectedRole;
        public string SelectedRole
        {
            get => _selectedRole;
            set
            {
                _selectedRole = value;
                OnPropertyChanged(nameof(DisplaySelectedRole));
                UpdateRole();
            }
        }

        public string DisplaySelectedRole =>
            SelectedClan != null
                ? $"{SelectedClan.Ime} uloga: {SelectedRole}"
                : "Nije izabran korisnik";

        public ClansRoleViewModel(LocalDbService dbService)
        {
            _dbService = dbService;
            LoadClansAsync();
        }

        private async Task LoadClansAsync()
        {
            try
            {
                await _dbService.InitAsync();
                var clans = await _dbService.GetItemsAsync<Clan>();
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ClanList.Clear();
                    foreach (var clan in clans)
                    {
                        clan.Role ??= "User";
                        ClanList.Add(clan);
                    }
                });
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private void UpdateSelectedRole()
        {
            if (SelectedClan != null)
            {
                SelectedRole = SelectedClan.Role ?? "User";
            }
        }

        private async void UpdateRole()
        {
            if (SelectedClan != null && !string.IsNullOrWhiteSpace(SelectedRole))
            {
                SelectedClan.Role = SelectedRole;

                try
                {
                    await _dbService.SaveItemAsync(SelectedClan);
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Failed to save the updated role. " + ex.Message, "OK");
                }
            }
        }

    }
}
