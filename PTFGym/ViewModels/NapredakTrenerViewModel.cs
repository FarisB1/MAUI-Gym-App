using PTFGym.Models;
using PTFGym.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PTFGym.ViewModels
{
    public class NapredakTrenerViewModel : BindableObject
    {
        private readonly LocalDbService _dbService;
        private readonly AuthenticationService _authService;

        public ObservableCollection<Clan> ClanList { get; } = new();
        public ICommand AddNapredakCommand { get; }

        private Clan _selectedClan;
        private float _tezina;
        private string _biljeske;

        public Clan SelectedClan
        {
            get => _selectedClan;
            set
            {
                _selectedClan = value;
                OnPropertyChanged();
            }
        }

        public float Tezina
        {
            get => _tezina;
            set
            {
                _tezina = value;
                OnPropertyChanged();
            }
        }

        public string Biljeske
        {
            get => _biljeske;
            set
            {
                _biljeske = value;
                OnPropertyChanged();
            }
        }

        public NapredakTrenerViewModel(LocalDbService dbService, AuthenticationService authService)
        {
            _dbService = dbService;
            _authService = authService;
            AddNapredakCommand = new Command(async () => await AddNapredakAsync());

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
                        ClanList.Add(clan);
                    }
                });
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async Task AddNapredakAsync()
        {
            if (SelectedClan == null || Tezina <= 0 || string.IsNullOrWhiteSpace(Biljeske))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please fill in all fields correctly", "OK");
                return;
            }

            try
            {
                var session = await _authService.GetUserSession();
                var newNapredak = new Napredak
                {
                    ClanId = SelectedClan.Id,
                    TrenerId = (int)session.UserId,
                    DatumUnosa = DateTime.Now,
                    Tezina = Tezina,
                    Biljeske = Biljeske
                };

                await _dbService.SaveNapredak(newNapredak);

                SelectedClan = null;
                Tezina = 0;
                Biljeske = string.Empty;

                await Application.Current.MainPage.DisplayAlert("Success", "Progress saved successfully", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to save progress", "OK");
                Console.WriteLine($"Error saving progress: {ex.Message}");
            }
        }
    }
}