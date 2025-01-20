using System.Collections.ObjectModel;
using System.Windows.Input;
using PTFGym.Models;
using PTFGym.Services;

namespace PTFGym.ViewModels
{
    public class RezervacijaViewModel : BindableObject
    {
        private readonly LocalDbService _dbService;
        private readonly UserSession _userSession;
        private readonly AuthenticationService _authenticationService;
        public ObservableCollection<Rezervacija> MojeRezervacije { get; } = new();
        public ObservableCollection<Rezervacija> TreneroveRezervacije { get; } = new();
        public ObservableCollection<Trener> TrenerList { get; } = new();

        public ICommand AddRezervacijaCommand { get; }
        public ICommand DeleteRezervacijaCommand { get; }
        public ICommand EditTrenerRezervacijaCommand { get; }

        private TimeSpan _vrijemeRezervacije = DateTime.Now.TimeOfDay;
        public TimeSpan VrijemeRezervacije
        {
            get => _vrijemeRezervacije;
            set
            {
                _vrijemeRezervacije = value;
                OnPropertyChanged();
            }
        }

        private Trener _selectedTrener;
        public Trener SelectedTrener
        {
            get => _selectedTrener;
            set
            {
                _selectedTrener = value;
                OnPropertyChanged();
            }
        }

        private DateTime _datumRezervacije = DateTime.Now;
        public DateTime DatumRezervacije
        {
            get => _datumRezervacije;
            set
            {
                _datumRezervacije = value;
                OnPropertyChanged();
            }
        }

        public RezervacijaViewModel(LocalDbService dbService)
        {
            _dbService = dbService;
            _userSession = Application.Current.MainPage.Handler.MauiContext?.Services.GetService<UserSession>();
            _authenticationService = Application.Current.MainPage.Handler.MauiContext?.Services.GetService<AuthenticationService>();
            AddRezervacijaCommand = new Command(async () => await AddRezervacijaAsync());
            DeleteRezervacijaCommand = new Command<Rezervacija>(async (rezervacija) => await DeleteRezervacijaAsync(rezervacija));
            EditTrenerRezervacijaCommand = new Command<Rezervacija>(async (rezervacija) => await EditTrenerRezervacijaAsync(rezervacija));

            LoadTreneriAsync();
            LoadMojeRezervacijeAsync();
            LoadTreneroveRezervacijeAsync();
        }

        public RezervacijaViewModel() { }
        public async Task LoadMojeRezervacijeAsync()
        {
            try
            {
                var userSession = await _authenticationService.GetUserSession();
                Console.WriteLine("laga loadanje");
                var rezervacije = await _dbService.GetItemsAsync<Rezervacija>();
                var treneri = await _dbService.GetItemsAsync<Trener>();
                Console.WriteLine("laga loadano");

                Console.WriteLine("laga joinanje");
                var mojeRezervacije = from r in rezervacije
                                      where r.ClanId == userSession.UserId
                                      join t in treneri on r.TrenerId equals t.Id
                                      select new RezervacijaDisplay
                                      {
                                          Id = r.Id,
                                          ClanId = r.ClanId,
                                          TrenerId = r.TrenerId,
                                          TrenerIme = t.Ime,
                                          DatumRezervacije = r.DatumRezervacije
                                      };
                Console.WriteLine("laga joinano");

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    MojeRezervacije.Clear();
                    foreach (var rezervacija in mojeRezervacije)
                    {
                        MojeRezervacije.Add(rezervacija);
                    }
                });
                
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Greška", ex.Message, "OK");
            }
        }

        public async Task LoadTreneroveRezervacijeAsync()
        {
            try
            {
                var userSession = await _authenticationService.GetUserSession();
                Console.WriteLine("laga loadanje baza");

                var rezervacije = await _dbService.GetItemsAsync<Rezervacija>();
                var clanovi = await _dbService.GetItemsAsync<Clan>();
                var treneri = await _dbService.GetItemsAsync<Trener>();

                Console.WriteLine($"Loaded {rezervacije.Count} rezervacije and {clanovi.Count} clanovi");

                foreach(var rez in rezervacije)
                {
                    Console.WriteLine($"Id: {rez.Id}");
                    Console.WriteLine($"ClanId: {rez.ClanId}");
                    Console.WriteLine($"TrenerId: {rez.TrenerId}");
                    Console.WriteLine($"DatumRezervacije: {rez.DatumRezervacije}");
                }
                
                foreach(var trener in treneri)
                {
                    Console.WriteLine($"Id: {trener.Id}");
                    Console.WriteLine($"Ime: {trener.Ime}");
                    Console.WriteLine($"Specijalnost: {trener.Specijalnost}");
                    Console.WriteLine($"UserId TRENER:: {trener.UserId}");
                }

                Console.WriteLine($"UserId: {userSession.UserId}");

                var treneroveRezervacije = from r in rezervacije
                                           where r.TrenerId == userSession.UserId
                                           join c in clanovi on r.ClanId equals c.Id
                                           select new RezervacijaDisplay
                                           {
                                               Id = r.Id,
                                               ClanId = r.ClanId,
                                               TrenerId = r.TrenerId,
                                               ClanIme = c.Ime,
                                               DatumRezervacije = r.DatumRezervacije
                                           };

                Console.WriteLine("laga joinane rezervacije");

                if (!treneroveRezervacije.Any())
                {
                    Console.WriteLine("No reservations found for the trainer");
                }

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    TreneroveRezervacije.Clear();
                    foreach (var rezervacija in treneroveRezervacije)
                    {
                        Console.WriteLine($"Id: {rezervacija.Id}");
                        Console.WriteLine($"ClanId: {rezervacija.ClanId}");
                        Console.WriteLine($"ClanIme: {rezervacija.ClanIme}");
                        Console.WriteLine($"TrenerId: {rezervacija.TrenerId}");
                        Console.WriteLine($"DatumRezervacije: {rezervacija.DatumRezervacije}");
                        Console.WriteLine("--------------------------");
                        TreneroveRezervacije.Add(rezervacija);
                    }
                });
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Greška", ex.Message, "OK");
            }
        }


        public async Task AddRezervacijaAsync()
        {
            if (SelectedTrener == null)
            {
                await Application.Current.MainPage.DisplayAlert("Greška", "Molimo odaberite trenera", "OK");
                return;
            }


            var userSession = await _authenticationService.GetUserSession();

            var datumVrijeme = DatumRezervacije.Date.Add(VrijemeRezervacije);

            Console.WriteLine($"ClanId: {(int)userSession.UserId}");
            Console.WriteLine($"TrenerId: {SelectedTrener.Id}");
            Console.WriteLine($"Vrijeme: {datumVrijeme}");

            var newRezervacija = new Rezervacija
            {
                ClanId = (int)userSession.UserId,
                TrenerId = SelectedTrener.Id,
                DatumRezervacije = datumVrijeme
            };

            Console.WriteLine("laga dodavanje");
            await _dbService.SaveRezervacija(newRezervacija);

            Console.WriteLine("laga dodano");
            Console.WriteLine("laga loadanje");
            await LoadMojeRezervacijeAsync();
            Console.WriteLine("laga loadano");

            // Reset form
            SelectedTrener = null;
            DatumRezervacije = DateTime.Now;
            VrijemeRezervacije = DateTime.Now.TimeOfDay;

            await Application.Current.MainPage.DisplayAlert("Uspjeh", "Rezervacija uspješno kreirana", "OK");
        }

        public async Task DeleteRezervacijaAsync(Rezervacija rezervacija)
        {
            try
            {
                Console.WriteLine($"Deleting reservation with ID: {rezervacija.Id}");
                Console.WriteLine($"ClanId: {rezervacija.ClanId}");
                Console.WriteLine($"TrenerId: {rezervacija.TrenerId}");
                Console.WriteLine($"DatumRezervacije: {rezervacija.DatumRezervacije}");

                bool answer = await Application.Current.MainPage.DisplayAlert(
                    "Otkaži rezervaciju",
                    "Jeste li sigurni da želite otkazati ovu rezervaciju?",
                    "Da",
                    "Ne");


                Console.WriteLine($"Confirmation answer: {answer}");

                if (answer)
                {
                    Console.WriteLine($"Attempting to delete rezervacija with ID: {rezervacija.Id}");
                    await _dbService.DeleteRezervacijaAsync(rezervacija.Id);

                    Console.WriteLine($"Successfully deleted rezervacija with ID: {rezervacija.Id}");

                    if (_userSession?.Role == "Clan")
                    {
                        Console.WriteLine("User is a 'Clan'. Reloading 'MojeRezervacije'.");
                        await LoadMojeRezervacijeAsync();
                    }
                    else if (_userSession?.Role == "Trener")
                    {
                        Console.WriteLine("User is a 'Trener'. Reloading 'TreneroveRezervacije'.");
                        await LoadTreneroveRezervacijeAsync();
                    }
                }
                else
                {
                    Console.WriteLine("User canceled the deletion.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during deletion: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Greška", ex.Message, "OK");
            }
        }


        public async Task EditTrenerRezervacijaAsync(Rezervacija rezervacija)
        {
            var time = await Application.Current.MainPage.DisplayPromptAsync(
                "Izmjena termina",
                "Unesite novo vrijeme (HH:mm):",
                initialValue: rezervacija.DatumRezervacije.ToString("HH:mm"));

            var newRezervacija = new Rezervacija
            {
                Id = rezervacija.Id,
                ClanId = rezervacija.ClanId,
                TrenerId = rezervacija.TrenerId,
                DatumRezervacije = rezervacija.DatumRezervacije
            };

            if (TimeSpan.TryParse(time, out var newTime))
            {
                newRezervacija.DatumRezervacije = newRezervacija.DatumRezervacije.Date.Add(newTime);
                await _dbService.SaveRezervacija(newRezervacija);
                await LoadTreneroveRezervacijeAsync();
            }
        }
        public async Task LoadTreneriAsync()
        {
            try
            {
                await _dbService.InitAsync();
                var treneri = await _dbService.GetItemsAsync<Trener>();
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    TrenerList.Clear();
                    foreach (var trener in treneri)
                    {
                        TrenerList.Add(trener);
                    }
                });
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

    }


}

