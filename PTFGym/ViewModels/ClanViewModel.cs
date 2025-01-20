using System.Collections.ObjectModel;
using System.Windows.Input;
using PTFGym.Models;
using PTFGym.Services;

namespace PTFGym.ViewModels
{
    public class ClanViewModel : BindableObject
    {
        private readonly LocalDbService _dbService;
        public ObservableCollection<Clan> Clanovi { get; } = new();
        public ICommand AddClanCommand { get; }
        public ICommand EditClanCommand { get; }
        public ICommand DeleteClanCommand { get; }

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

        public ClanViewModel(LocalDbService dbService)
        {
            _dbService = dbService;
            AddClanCommand = new Command(async () => await AddClanAsync());
            EditClanCommand = new Command<Clan>(async (clan) => await EditClanAsync(clan));
            DeleteClanCommand = new Command<Clan>(async (clan) => await DeleteClanAsync(clan));

            LoadClanoviAsync();
        }

        public ClanViewModel()
        {

        }
        public async Task LoadClanoviAsync()
        {
            try
            {
                await _dbService.InitAsync();
                var clanList = await _dbService.GetItemsAsync<Clan>();
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    foreach (var clan in clanList)
                    {
                        if (!Clanovi.Any(c => c.Id == clan.Id))
                        {
                            Clanovi.Add(clan);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async Task AddClanAsync()
        {
            if (string.IsNullOrWhiteSpace(Ime) || string.IsNullOrWhiteSpace(Email))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please enter name and email", "OK");
                return;
            }

            var newClan = new Clan
            {
                Ime = Ime,
                Email = Email,
                DatumPocetkaClanstva = DateTime.Now,
                DatumKrajaClanstva = DateTime.Now.AddMonths(1)
            };

            Console.WriteLine($"Clanovi count before adding: {Clanovi.Count}");

            await _dbService.SaveClan(newClan);


            Clanovi.Add(newClan);

            Console.WriteLine($"Clanovi count after adding: {Clanovi.Count}");

            Ime = string.Empty;
            Email = string.Empty;
        }



        private async Task EditClanAsync(Clan clan)
        {
            var name = await Application.Current.MainPage.DisplayPromptAsync("Edit Member",
                "Enter new name:",
                initialValue: clan.Ime);

            if (name != null)
            {
                clan.Ime = name;

                var email = await Application.Current.MainPage.DisplayPromptAsync("Edit Member",
                    "Enter new email:",
                    initialValue: clan.Email);
                
                if (email != null)
                {
                    clan.Email = email;
                    await _dbService.SaveItemAsync(clan);

                    var index = Clanovi.IndexOf(clan);
                    if (index != -1)
                    {
                        Clanovi[index] = clan;
                    }
                }
            }
        }

        private async Task DeleteClanAsync(Clan clan)
        {
            bool answer = await Application.Current.MainPage.DisplayAlert(
                "Delete Member",
                $"Are you sure you want to delete {clan.Ime}?",
                "Yes",
                "No");

            if (answer)
            {
                await _dbService.DeleteItemAsync(clan);
                Clanovi.Remove(clan);
            }
        }
    }
}