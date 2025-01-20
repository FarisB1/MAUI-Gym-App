using System.Collections.ObjectModel;
using System.Windows.Input;
using PTFGym.Models;
using PTFGym.Services;
using System;
using PTFGym.Views.Termin;

namespace PTFGym.ViewModels
{
    public class TerminViewModel : BindableObject
    {
        private readonly LocalDbService _dbService;
        public ObservableCollection<Termin> Termini { get; } = new();
        private readonly AuthenticationService _authenticationService;

        public ICommand AddClanTerminCommand { get; }
        public ICommand AddTerminCommand { get; }
        public ICommand EditTerminCommand { get; }
        public ICommand DeleteTerminCommand { get; }
        public ICommand ShowDetailsCommand { get; private set; }

        private DateTime _datumVrijeme;
        public DateTime DatumVrijeme
        {
            get => _datumVrijeme = DateTime.Today;
            set
            {
                _datumVrijeme = value;
                OnPropertyChanged();
            }
        }

        private string _vrstaTreninga;
        public string VrstaTreninga
        {
            get => _vrstaTreninga;
            set
            {
                _vrstaTreninga = value;
                OnPropertyChanged();
            }
        }

        private int _maksimalniBrojClanova;
        public int MaksimalniBrojClanova
        {
            get => _maksimalniBrojClanova;
            set
            {
                _maksimalniBrojClanova = value;
                OnPropertyChanged();
            }
        }

        public TerminViewModel(LocalDbService dbService)
        {
            _dbService = dbService;
            _authenticationService = Application.Current.MainPage.Handler.MauiContext.Services.GetRequiredService<AuthenticationService>();

            AddTerminCommand = new Command(async () => await AddTerminAsync());
            EditTerminCommand = new Command<Termin>(async (termin) => await EditTerminAsync(termin));
            DeleteTerminCommand = new Command<Termin>(async (termin) => await DeleteTerminAsync(termin));
            AddClanTerminCommand = new Command<int>(async (terminId) => await AddClanTerminAsync(terminId));
            ShowDetailsCommand = new Command<Termin>(async (termin) => await ShowDetailsAsync(termin));

        }

        public TerminViewModel() { }


        public async Task LoadTerminiForTrainerAsync(int trenerId)
        {
            try
            {
                await _dbService.InitAsync();

                var terminList = (await _dbService.GetItemsAsync<Termin>())
                    .Where(t => t.TrenerId == trenerId)
                    .ToList();

                var clanTerminList = await _dbService.GetItemsAsync<ClanTermin>();

                var clanCounts = clanTerminList
                    .GroupBy(ct => ct.TerminId)
                    .ToDictionary(g => g.Key, g => g.Count());

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Termini.Clear();
                    foreach (var termin in terminList)
                    {
                        termin.TrenutnoClanova = clanCounts.ContainsKey(termin.Id) ? clanCounts[termin.Id] : 0;
                        Termini.Add(termin);
                    }
                });

                Console.WriteLine($"Loaded {terminList.Count} termini for trainer {trenerId}");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
                Console.WriteLine($"Error loading termini: {ex}");
            }
        }
        public async Task LoadTerminiAsync()
        {
            try
            {
                await _dbService.InitAsync();

                var terminList = await _dbService.GetItemsAsync<Termin>();
                var clanTerminList = await _dbService.GetItemsAsync<ClanTermin>();

                var clanCounts = clanTerminList
                    .GroupBy(ct => ct.TerminId)
                    .ToDictionary(g => g.Key, g => g.Count());

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Termini.Clear();
                    foreach (var termin in terminList)
                    {
                        termin.TrenutnoClanova = clanCounts.ContainsKey(termin.Id) ? clanCounts[termin.Id] : 0;
                        Termini.Add(termin);
                    }
                });
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async Task ShowDetailsAsync(Termin termin)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new TerminDetail(termin));
        }

        private async Task AddClanTerminAsync(int terminId)
        {
            try
            {
                var _userSession = await _authenticationService.GetUserSession();
                if (_userSession?.UserId == null || _userSession.UserId == 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Please log in to book a training session.", "OK");
                    return;
                }

                var existingBooking = await _dbService.GetItemsAsync<ClanTermin>();
                var alreadyBooked = existingBooking.Any(ct => ct.ClanId == _userSession.UserId && ct.TerminId == terminId);

                if (alreadyBooked)
                {
                    await Application.Current.MainPage.DisplayAlert("Greška", "Već ste u terminu", "OK");
                    return;
                }

                var termin = Termini.FirstOrDefault(t => t.Id == terminId);
                if (termin != null && termin.TrenutnoClanova >= termin.MaksimalniBrojClanova)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "This training session is full.", "OK");
                    return;
                }

                var clanTermin = new ClanTermin
                {
                    ClanId = (int)_userSession.UserId,
                    TerminId = terminId
                };

                await _dbService.SaveClanTermin(clanTermin);

                if (termin != null)
                {
                    termin.TrenutnoClanova++;
                    var index = Termini.IndexOf(termin);
                    if (index != -1)
                    {
                        Termini[index] = termin;
                    }
                }

                await Application.Current.MainPage.DisplayAlert("Success", "You have been successfully added to the training session.", "OK");
                await LoadTerminiAsync();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to register for training: {ex.Message}", "OK");
            }
        }

        private async Task AddTerminAsync()
        {
            try
            {
                var userSession = await _authenticationService.GetUserSession();
                if (userSession?.UserId == null || userSession.Role != "Trener")
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Only trainers can create sessions", "OK");
                    return;
                }

                if (string.IsNullOrWhiteSpace(VrstaTreninga) || MaksimalniBrojClanova <= 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Please fill in all fields correctly", "OK");
                    return;
                }

                var newTermin = new Termin
                {
                    DatumVrijeme = DatumVrijeme,
                    VrstaTreninga = VrstaTreninga,
                    MaksimalniBrojClanova = MaksimalniBrojClanova,
                    TrenerId = (int)userSession.UserId
                };

                await _dbService.SaveTermin(newTermin);
                await LoadTerminiForTrainerAsync((int)userSession.UserId);

                DatumVrijeme = DateTime.Now;
                VrstaTreninga = string.Empty;
                MaksimalniBrojClanova = 0;

                await Application.Current.MainPage.DisplayAlert("Success", "Training session created successfully", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to create training session: {ex.Message}", "OK");
            }
        }

        private async Task EditTerminAsync(Termin termin)
        {
            var treningType = await Application.Current.MainPage.DisplayPromptAsync("Edit Schedule",
                "Enter new training type:",
                initialValue: termin.VrstaTreninga);

            if (treningType != null)
            {
                termin.VrstaTreninga = treningType;

                var maxMembers = await Application.Current.MainPage.DisplayPromptAsync("Edit Schedule",
                    "Enter new maximum members:",
                    initialValue: termin.MaksimalniBrojClanova.ToString());

                if (int.TryParse(maxMembers, out var newMaxMembers))
                {
                    termin.MaksimalniBrojClanova = newMaxMembers;

                    var date = await Application.Current.MainPage.DisplayPromptAsync("Edit Schedule",
                        "Enter new date and time (yyyy-MM-dd HH:mm):",
                        initialValue: termin.DatumVrijeme.ToString("yyyy-MM-dd HH:mm"));

                    if (DateTime.TryParse(date, out var newDateTime))
                    {
                        termin.DatumVrijeme = newDateTime;
                        await _dbService.SaveItemAsync(termin);

                        var index = Termini.IndexOf(termin);
                        if (index != -1)
                        {
                            Termini[index] = termin;
                        }
                    }
                }
            }
        }

        private async Task DeleteTerminAsync(Termin termin)
        {
            bool answer = await Application.Current.MainPage.DisplayAlert(
                "Delete Schedule",
                $"Are you sure you want to delete this schedule for {termin.VrstaTreninga} on {termin.DatumVrijeme}?",
                "Yes",
                "No");

            if (answer)
            {
                await _dbService.DeleteItemAsync(termin);
                Termini.Remove(termin);
            }
        }
    }
}
