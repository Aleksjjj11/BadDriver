using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using MobileApps.Interfaces;
using MobileApps.Models;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.UI.Views.Options;
using Xamarin.Forms;

namespace MobileApps.ViewModels
{
    public class AchievementsViewModel : BaseViewModel
    {
        private readonly BackgroundWorker _bwAchievementUpdater;
        private readonly Page _ownPage;
        private bool _isBusy;

        public ObservableCollection<IAchievement> Achievements { get; set; }
        
        public Command RefreshUserCommand { get; private set; }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        public AchievementsViewModel(Page page)
        {
            _ownPage = page;

            InitCommand();

            _bwAchievementUpdater = new BackgroundWorker();

            _bwAchievementUpdater.DoWork += BwAchievementUpdaterOnDoWork;
            _bwAchievementUpdater.RunWorkerCompleted += BwAchievementUpdaterOnRunWorkerCompleted;
        }

        private void InitCommand()
        {
            RefreshUserCommand = new Command(GetAllAchievements);
        }

        private void GetAllAchievements()
        {
            _bwAchievementUpdater.RunWorkerAsync();
        }

        private void BwAchievementUpdaterOnDoWork(object sender, DoWorkEventArgs e)
        {
            var allAchievements = ((User)App.CurrentUser).GetAllAchievements(App.IpAddress).Result;
            e.Result = new ObservableCollection<IAchievement>(allAchievements ?? new List<IAchievement>());
        }

        private void BwAchievementUpdaterOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Achievements = new ObservableCollection<IAchievement>(e.Result as ObservableCollection<IAchievement>);
            _ownPage.DisplayToastAsync(new ToastOptions
            {
                BackgroundColor = Color.FromHex("#c661cf"),
                Duration = TimeSpan.FromSeconds(2),
                MessageOptions = new MessageOptions
                {
                    Message = e.Error is null ? "Успех, вот ачивки!" : $"Крах! Вот ошибка\n{e.Error.Message}",
                    Foreground = Color.White
                }
            });

            OnPropertyChanged(nameof(Achievements));
            IsBusy = false;
        }
    }
}