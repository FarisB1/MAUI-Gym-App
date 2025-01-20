using System.Collections.ObjectModel;
using System.Windows.Input;
using PTFGym.Models;
using PTFGym.Services;

namespace PTFGym.ViewModels
{
    public class TrenerViewModel : BindableObject
    {
        private readonly LocalDbService _dbService;
        public ObservableCollection<Trener> Treneri { get; } = new();
        public ObservableCollection<Clan> ClanList { get; } = new();
        public ICommand AddTrenerCommand { get; }
        public ICommand EditTrenerCommand { get; }
        public ICommand DeleteTrenerCommand { get; }

        private string _ime;
        public string Ime
        {
            get => _ime;
            set
            {
                _ime = value;
                OnPropertyChanged();
            }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        private string _specijalnost;
        public string Specijalnost
        {
            get => _specijalnost;
            set
            {
                _specijalnost = value;
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
                if (_selectedClan != null)
                {
                    Ime = _selectedClan.Ime;
                    Email = _selectedClan.Email;
                }
            }
        }
        public TrenerViewModel(LocalDbService dbService)
        {
            _dbService = dbService;
            AddTrenerCommand = new Command(async () => await AddTrenerAsync());
            EditTrenerCommand = new Command<Trener>(async (trener) => await EditTrenerAsync(trener));
            DeleteTrenerCommand = new Command<Trener>(async (trener) => await DeleteTrenerAsync(trener));

            LoadTreneriAsync();
            LoadClansAsync();
        }

        public TrenerViewModel() { }
        private async Task LoadTreneriAsync()
        {
            try
            {
                await _dbService.InitAsync();
                var trenerList = await _dbService.GetItemsAsync<Trener>();
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Treneri.Clear();
                    foreach (var trener in trenerList)
                    {
                        Treneri.Add(trener);
                    }
                });
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
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
        private async Task AddTrenerAsync()
        {
            if (SelectedClan == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please select a member", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Specijalnost))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please fill in all fields", "OK");
                return;
            }

            var newTrener = new Trener
            {
                Id = SelectedClan.Id,
                Ime = SelectedClan.Ime,
                Email = SelectedClan.Email,
                Specijalnost = Specijalnost,
                UserId = Guid.NewGuid().ToString()
            };

            SelectedClan.Role = "Trener";

            Console.WriteLine($"Clan role: {SelectedClan.Role}");
            await _dbService.SaveClan(SelectedClan);
            Console.WriteLine($"Clan role: {SelectedClan.Role}");

            await _dbService.SaveItemAsync(newTrener);

            Treneri.Add(newTrener);

            Specijalnost = string.Empty;
        }


        private async Task EditTrenerAsync(Trener trener)
        {
            var name = await Application.Current.MainPage.DisplayPromptAsync("Edit Trainer",
                "Enter new name:",
                initialValue: trener.Ime);

            if (name != null)
            {
                trener.Ime = name;

                var email = await Application.Current.MainPage.DisplayPromptAsync("Edit Trainer",
                    "Enter new email:",
                    initialValue: trener.Email);

                if (email != null)
                {
                    trener.Email = email;

                    var speciality = await Application.Current.MainPage.DisplayPromptAsync("Edit Trainer",
                        "Enter new speciality:",
                        initialValue: trener.Specijalnost);

                    if (speciality != null)
                    {
                        trener.Specijalnost = speciality;
                        await _dbService.SaveItemAsync(trener);

                        var index = Treneri.IndexOf(trener);
                        if (index != -1)
                        {
                            Treneri[index] = trener;
                        }
                    }
                }
            }
        }

        private async Task DeleteTrenerAsync(Trener trener)
        {
            bool answer = await Application.Current.MainPage.DisplayAlert(
                "Delete Trainer",
                $"Are you sure you want to delete {trener.Ime}?",
                "Yes",
                "No");

            if (answer)
            {
                await _dbService.DeleteItemAsync(trener);
                Treneri.Remove(trener);
            }
        }
    }
}