using System.Collections.ObjectModel;
using PTFGym.Models;
using System.Windows.Input;
using PTFGym.Services;

namespace PTFGym.ViewModels
{
    public class ClanarinaClanViewModel : BindableObject
    {
        private readonly LocalDbService _dbService;
        private readonly AuthenticationService _authService;
        private bool _hasActiveMembership;
        private string _memberName;
        private string _membershipStatus;
        private Color _statusColor;
        private int _daysRemaining;
        private Clanarina _currentMembership;
        private ObservableCollection<Clanarina> _membershipHistory;
        private bool _showRenewButton;

        public bool HasActiveMembership
        {
            get => _hasActiveMembership;
            set
            {
                _hasActiveMembership = value;
                OnPropertyChanged();
            }
        }

        public string MemberName
        {
            get => _memberName;
            set
            {
                _memberName = value;
                OnPropertyChanged();
            }
        }

        public string MembershipStatus
        {
            get => _membershipStatus;
            set
            {
                _membershipStatus = value;
                OnPropertyChanged();
            }
        }

        public Color StatusColor
        {
            get => _statusColor;
            set
            {
                _statusColor = value;
                OnPropertyChanged();
            }
        }

        public int DaysRemaining
        {
            get => _daysRemaining;
            set
            {
                _daysRemaining = value;
                OnPropertyChanged();
            }
        }

        public Clanarina CurrentMembership
        {
            get => _currentMembership;
            set
            {
                _currentMembership = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Clanarina> MembershipHistory
        {
            get => _membershipHistory;
            set
            {
                _membershipHistory = value;
                OnPropertyChanged();
            }
        }

        public bool ShowRenewButton
        {
            get => _showRenewButton;
            set
            {
                _showRenewButton = value;
                OnPropertyChanged();
            }
        }

        public ICommand RenewMembershipCommand { get; }

        public ClanarinaClanViewModel(LocalDbService dbService, AuthenticationService authService)
        {
            _dbService = dbService;
            _authService = authService;
            _membershipHistory = new ObservableCollection<Clanarina>();
            RenewMembershipCommand = new Command(async () => await RenewMembershipAsync());

            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            try
            {
                var session = await _authService.GetUserSession();
                if (session.UserId == 0)
                {
                    await Shell.Current.GoToAsync("//LoginPage");
                    return;
                }

                var clan = await _dbService.GetItemByIdAsync<Clan>((int)session.UserId);
                if (clan != null)
                {
                    MemberName = clan.Ime;
                    await LoadMembershipDataAsync((int)session.UserId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in InitializeAsync: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to load membership data", "OK");
            }
        }

        private async Task LoadMembershipDataAsync(int userId)
        {
            try
            {
                var allMemberships = (await _dbService.GetItemsAsync<Clanarina>())
                    .Where(c => c.ClanId == userId)
                    .OrderByDescending(c => c.DatumPocetka)
                    .ToList();

                if (!allMemberships.Any())
                {
                    SetupNoMembershipState();
                    return;
                }

                var currentDate = DateTime.Now;
                CurrentMembership = allMemberships.FirstOrDefault(c =>
                    c.DatumPocetka <= currentDate && c.DatumZavrsetka >= currentDate);

                if (CurrentMembership != null)
                {
                    SetupActiveMembershipState(CurrentMembership);
                }
                else
                {
                    SetupExpiredMembershipState(allMemberships[0]);
                }

                MembershipHistory = new ObservableCollection<Clanarina>(
                    allMemberships.Where(m => m != CurrentMembership));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading membership data: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to load membership data", "OK");
            }
        }

        private void SetupNoMembershipState()
        {
            HasActiveMembership = false;
            MembershipStatus = "No active membership";
            StatusColor = Colors.Red;
            ShowRenewButton = true;
        }

        private void SetupActiveMembershipState(Clanarina membership)
        {
            HasActiveMembership = true;
            DaysRemaining = (membership.DatumZavrsetka - DateTime.Now).Days;
            MembershipStatus = "Active Membership";
            StatusColor = Colors.Green;
            ShowRenewButton = DaysRemaining <= 7;
        }

        private void SetupExpiredMembershipState(Clanarina lastMembership)
        {
            HasActiveMembership = false;
            MembershipStatus = "Membership Expired";
            StatusColor = Colors.Red;
            ShowRenewButton = true;
        }

        private async Task RenewMembershipAsync()
        {
            try
            {
                var session = await _authService.GetUserSession();
                var startDate = DateTime.Now;
                var endDate = startDate.AddMonths(1);

                var newClanarina = new Clanarina
                {
                    ClanId = (int)session.UserId,
                    DatumPocetka = startDate,
                    DatumZavrsetka = endDate,
                    Iznos = 30.0f
                };

                await _dbService.SaveClanarina(newClanarina);
                await LoadMembershipDataAsync((int)session.UserId);

                await Application.Current.MainPage.DisplayAlert("Success", "Membership renewed successfully", "OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error renewing membership: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to renew membership", "OK");
            }
        }
    }
}