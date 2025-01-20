using System.Collections.ObjectModel;
using PTFGym.Models;
using PTFGym.Services;

namespace PTFGym.ViewModels
{
    public class TerminDetailViewModel : BindableObject
    {
        private readonly LocalDbService _dbService;
        public ObservableCollection<ClanTerminDetails> EnrolledUsers { get; } = new();

        private Termin _termin;
        public Termin Termin
        {
            get => _termin;
            set
            {
                _termin = value;
                OnPropertyChanged();
                LoadEnrolledUsersAsync();
            }
        }

        public TerminDetailViewModel(LocalDbService dbService)
        {
            _dbService = dbService;
        }

        private async Task LoadEnrolledUsersAsync()
        {
            try
            {
                if (Termin == null) return;

                var clanTermins = await _dbService.GetItemsAsync<ClanTermin>();
                var clans = await _dbService.GetItemsAsync<Clan>();

                var enrolledUsers = from ct in clanTermins
                                    where ct.TerminId == Termin.Id
                                    join clan in clans on ct.ClanId equals clan.Id
                                    select new ClanTerminDetails
                                    {
                                        ClanId = clan.Id,
                                        Ime = clan.Ime,
                                        Email = clan.Email
                                    };

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    EnrolledUsers.Clear();
                    foreach (var user in enrolledUsers)
                    {
                        EnrolledUsers.Add(user);
                    }
                });
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }

    public class ClanTerminDetails
    {
        public int ClanId { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Email { get; set; }
    }
}