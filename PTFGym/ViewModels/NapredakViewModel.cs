using System.Collections.ObjectModel;
using System.Windows.Input;
using PTFGym.Models;
using PTFGym.Services;

namespace PTFGym.ViewModels
{
    public class NapredakViewModel : BindableObject
    {
        private readonly LocalDbService _dbService;
        public ObservableCollection<Napredak> Napreci { get; } = new();
        public ObservableCollection<Clan> ClanList { get; } = new();
        public ICommand AddNapredakCommand { get; }
        public ICommand EditNapredakCommand { get; }
        public ICommand DeleteNapredakCommand { get; }

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

        private int _trenerId;
        public int TrenerId
        {
            get => _trenerId;
            set
            {
                _trenerId = value;
                OnPropertyChanged();
            }
        }

        private DateTime _datumUnosa = DateTime.Today;
        public DateTime DatumUnosa
        {
            get => _datumUnosa;
            set
            {
                _datumUnosa = value;
                OnPropertyChanged();
            }
        }

        private float _tezina;
        public float Tezina
        {
            get => _tezina;
            set
            {
                _tezina = value;
                OnPropertyChanged();
            }
        }

        private string _biljeske;
        public string Biljeske
        {
            get => _biljeske;
            set
            {
                _biljeske = value;
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

       

        public NapredakViewModel(LocalDbService dbService)
        {
            _dbService = dbService;
            AddNapredakCommand = new Command(async () => await AddNapredakAsync());
            EditNapredakCommand = new Command<Napredak>(async (napredak) => await EditNapredakAsync(napredak));
            DeleteNapredakCommand = new Command<Napredak>(async (napredak) => await DeleteNapredakAsync(napredak));

            LoadNapreciAsync();
            LoadClansAsync();
        }

        public NapredakViewModel() {

        }
        private async Task LoadNapreciAsync()
        {
            try
            {
                await _dbService.InitAsync();
                var napredakList = await _dbService.GetItemsAsync<Napredak>();
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Napreci.Clear();
                    foreach (var napredak in napredakList)
                    {
                        Napreci.Add(napredak);
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

            Console.WriteLine($"SelectedClan: {SelectedClan.Id}");
            Console.WriteLine($"DatumUnosa: {DatumUnosa}");
            Console.WriteLine($"Tezina: {Tezina}");
            Console.WriteLine($"Biljeske: {Biljeske}");


            if (SelectedClan == null || DatumUnosa == default || Tezina <= 0 || string.IsNullOrWhiteSpace(Biljeske))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please fill in all fields correctly", "OK");
                return;
            }

            var newNapredak = new Napredak
            {
                ClanId = SelectedClan.Id,
                DatumUnosa = DatumUnosa,
                Tezina = Tezina,
                Biljeske = Biljeske
            };

            await _dbService.SaveNapredak(newNapredak);
            Napreci.Add(newNapredak);

            SelectedClan = null;
            DatumUnosa = default;
            Tezina = 0;
            Biljeske = string.Empty;
        }

        private async Task EditNapredakAsync(Napredak napredak)
        {
            var clanId = await Application.Current.MainPage.DisplayPromptAsync("Edit Progress",
                "Enter new member ID:",
                initialValue: napredak.ClanId.ToString());

            if (int.TryParse(clanId, out var newClanId))
            {
                napredak.ClanId = newClanId;

                var trenerId = await Application.Current.MainPage.DisplayPromptAsync("Edit Progress",
                    "Enter new trainer ID:",
                    initialValue: napredak.TrenerId.ToString());

                if (int.TryParse(trenerId, out var newTrenerId))
                {
                    napredak.TrenerId = newTrenerId;

                    var weight = await Application.Current.MainPage.DisplayPromptAsync("Edit Progress",
                        "Enter new weight:",
                        initialValue: napredak.Tezina.ToString());

                    if (float.TryParse(weight, out var newWeight))
                    {
                        napredak.Tezina = newWeight;

                        var notes = await Application.Current.MainPage.DisplayPromptAsync("Edit Progress",
                            "Enter new notes:",
                            initialValue: napredak.Biljeske);

                        if (notes != null)
                        {
                            napredak.Biljeske = notes;
                            await _dbService.SaveItemAsync(napredak);

                            var index = Napreci.IndexOf(napredak);
                            if (index != -1)
                            {
                                Napreci[index] = napredak;
                            }
                        }
                    }
                }
            }
        }

        private async Task DeleteNapredakAsync(Napredak napredak)
        {
            bool answer = await Application.Current.MainPage.DisplayAlert(
                "Delete Progress",
                $"Are you sure you want to delete this entry?",
                "Yes",
                "No");

            if (answer)
            {
                await _dbService.DeleteItemAsync(napredak);
                Napreci.Remove(napredak);
            }
        }
    }
}
