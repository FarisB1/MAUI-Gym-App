using System.Collections.ObjectModel;
using System.Windows.Input;
using PTFGym.Models;
using PTFGym.Services;

namespace PTFGym.ViewModels
{
    public class ClanarinaViewModel : BindableObject
    {
        private readonly LocalDbService _dbService;
        public ObservableCollection<Clanarina> Clanarine { get; } = new(); 
        public ObservableCollection<Clan> ClanList { get; } = new();
        public ICommand AddClanarinaCommand { get; }
        public ICommand EditClanarinaCommand { get; }
        public ICommand DeleteClanarinaCommand { get; }

        private int _clanId;
        public int ClanId
        {
            get => _clanId;
            set
            {
                _clanId = value;
                OnPropertyChanged();
            }
        }

        private DateTime _datumPocetka = DateTime.Today;
        public DateTime DatumPocetka
        {
            get => _datumPocetka;
            set
            {
                _datumPocetka = value;
                OnPropertyChanged();
            }
        }

        private DateTime _datumZavrsetka = DateTime.Today;
        public DateTime DatumZavrsetka
        {
            get => _datumZavrsetka;
            set
            {
                _datumZavrsetka = value;
                OnPropertyChanged();
            }
        }

        private float _iznos;
        public float Iznos
        {
            get => _iznos;
            set
            {
                _iznos = value;
                OnPropertyChanged();
            }
        }

        private string _userName;
        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged();
            }
        }

        private Clan _selectedClan;
        public Clan SelectedClan
        {
            get => _selectedClan;
            set
            {
                _selectedClan = value;
                OnPropertyChanged();
                UpdateUserName();
            }
        }


        private void UpdateUserName()
        {
            if (SelectedClan != null)
            {
                UserName = SelectedClan.Ime; 
                Console.WriteLine($"Selected Clan Name: {SelectedClan?.Ime}");
            }
            else
            {
                UserName = "No user selected";
            }
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
        public ClanarinaViewModel(LocalDbService dbService)
        {
            _dbService = dbService;
            AddClanarinaCommand = new Command(async () => await AddClanarinaAsync());
            EditClanarinaCommand = new Command<Clanarina>(async (clanarina) => await EditClanarinaAsync(clanarina));
            DeleteClanarinaCommand = new Command<Clanarina>(async (clanarina) => await DeleteClanarinaAsync(clanarina));
            LoadClansAsync();
            LoadClanarineAsync();
        }
        public ClanarinaViewModel() { }
        private async Task LoadClanarineAsync()
        {
            try
            {
                await _dbService.InitAsync();
                var clanarinaList = await _dbService.GetItemsAsync<Clanarina>();
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Clanarine.Clear();
                    foreach (var clanarina in clanarinaList)
                    {
                        Clanarine.Add(clanarina);
                    }
                });
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }


        private async Task AddClanarinaAsync()
        {
            await Application.Current.MainPage.DisplayAlert(
        "Debug Info",
        $"SelectedClan: {SelectedClan?.Ime ?? "null"}, DatumPocetka: {DatumPocetka}, DatumZavrsetka: {DatumZavrsetka}, Iznos: {Iznos}",
        "OK");

            if (SelectedClan == null || DatumPocetka == default || DatumZavrsetka == default || Iznos <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please fill in all fields correctly", "OK");
                return;
            }

            if (DatumPocetka > DatumZavrsetka)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Start date must be before end date", "OK");
                return;
            }

            var newClanarina = new Clanarina
            {
                ClanId = SelectedClan.Id,
                DatumPocetka = DatumPocetka,
                DatumZavrsetka = DatumZavrsetka,
                Iznos = Iznos
            };

            await _dbService.SaveClanarina(newClanarina);
            Clanarine.Add(newClanarina);

            // Clear inputs
            SelectedClan = null;
            DatumPocetka = default;
            DatumZavrsetka = default;
            Iznos = 0;
        }

        private async Task EditClanarinaAsync(Clanarina clanarina)
        {
            var clanId = await Application.Current.MainPage.DisplayPromptAsync("Edit Membership",
                "Enter new member ID:",
                initialValue: clanarina.ClanId.ToString());

            if (int.TryParse(clanId, out var newClanId))
            {
                clanarina.ClanId = newClanId;

                var startDate = await Application.Current.MainPage.DisplayPromptAsync("Edit Membership",
                    "Enter new start date (yyyy-MM-dd):",
                    initialValue: clanarina.DatumPocetka.ToString("yyyy-MM-dd"));

                if (DateTime.TryParse(startDate, out var newStartDate))
                {
                    clanarina.DatumPocetka = newStartDate;

                    var endDate = await Application.Current.MainPage.DisplayPromptAsync("Edit Membership",
                        "Enter new end date (yyyy-MM-dd):",
                        initialValue: clanarina.DatumZavrsetka.ToString("yyyy-MM-dd"));

                    if (DateTime.TryParse(endDate, out var newEndDate))
                    {
                        clanarina.DatumZavrsetka = newEndDate;

                        var amount = await Application.Current.MainPage.DisplayPromptAsync("Edit Membership",
                            "Enter new amount:",
                            initialValue: clanarina.Iznos.ToString());

                        if (float.TryParse(amount, out var newAmount))
                        {
                            clanarina.Iznos = newAmount;
                            await _dbService.SaveItemAsync(clanarina);

                            var index = Clanarine.IndexOf(clanarina);
                            if (index != -1)
                            {
                                Clanarine[index] = clanarina;
                            }
                        }
                    }
                }
            }
        }

        private async Task DeleteClanarinaAsync(Clanarina clanarina)
        {
            bool answer = await Application.Current.MainPage.DisplayAlert(
                "Delete Membership",
                $"Are you sure you want to delete this membership?",
                "Yes",
                "No");

            if (answer)
            {
                await _dbService.DeleteItemAsync(clanarina);
                Clanarine.Remove(clanarina);
            }
        }
    }
}
