using System.Collections.ObjectModel;
using PTFGym.Models;
using PTFGym.Services;

namespace PTFGym.ViewModels
{
    public class NapredakClanViewModel : BindableObject
    {
        private readonly LocalDbService _dbService;
        private readonly AuthenticationService _authService;

        private float _currentWeight;
        private float _weightChange;
        private ObservableCollection<Napredak> _progressHistory;

        public float CurrentWeight
        {
            get => _currentWeight;
            set
            {
                _currentWeight = value;
                OnPropertyChanged();
            }
        }

        public float WeightChange
        {
            get => _weightChange;
            set
            {
                _weightChange = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Napredak> ProgressHistory
        {
            get => _progressHistory;
            set
            {
                _progressHistory = value;
                OnPropertyChanged();
            }
        }

        public NapredakClanViewModel(LocalDbService dbService, AuthenticationService authService)
        {
            _dbService = dbService;
            _authService = authService;
            _progressHistory = new ObservableCollection<Napredak>();

            LoadProgressDataAsync();
        }

        private async void LoadProgressDataAsync()
        {
            try
            {
                var session = await _authService.GetUserSession();
                if (session.UserId == 0)
                {
                    await Shell.Current.GoToAsync("//LoginPage");
                    return;
                }

                var allProgress = (await _dbService.GetItemsAsync<Napredak>())
                    .Where(n => n.ClanId == session.UserId)
                    .OrderByDescending(n => n.DatumUnosa)
                    .ToList();

                if (allProgress.Any())
                {
                    CurrentWeight = allProgress.First().Tezina;

                    if (allProgress.Count > 1)
                    {
                        WeightChange = CurrentWeight - allProgress[1].Tezina;
                    }

                    ProgressHistory = new ObservableCollection<Napredak>(allProgress);
                }
                else
                {
                    CurrentWeight = 0;
                    WeightChange = 0;
                    ProgressHistory.Clear();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading progress data: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to load progress data", "OK");
            }
        }
    }
}