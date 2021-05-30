using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using MobileApps.Interfaces;
using MobileApps.Models;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace MobileApps.ViewModels
{
    public class AchievementsViewModel : BaseViewModel
    {
        private readonly BackgroundWorker _bwAchievementUpdater;
        private readonly Page _ownPage;
        private bool _isBusy;

        public ObservableCollection<IAchievement> Achievements => App.CurrentUser.Achievements;
        public IAchievement[] ArrayAchievements => Achievements.ToArray();
        
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

            _bwAchievementUpdater = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };

            _bwAchievementUpdater.DoWork += BwAchievementUpdaterOnDoWork;
            _bwAchievementUpdater.ProgressChanged += BwAchievementUpdaterOnProgressChanged;
            _bwAchievementUpdater.RunWorkerCompleted += BwAchievementUpdaterOnRunWorkerCompleted;
        }

        private void InitCommand()
        {
            RefreshUserCommand = new Command(() =>
            {
                _bwAchievementUpdater.RunWorkerAsync();
            });
        }

        private void BwAchievementUpdaterOnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }

        private void BwAchievementUpdaterOnDoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                (App.CurrentUser as User)?.GetAllAchievements();
            }
            catch (Exception ex)
            {
                Log.Warning("Error Achievement Update", ex.Message);
            }
        }

        private void BwAchievementUpdaterOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsBusy = false;

            _ownPage.DisplayToastAsync(e.Error is null ? "Успех, вот ачивки!" : $"Крах! Вот ошибка\n{e.Error.Message}");

            OnPropertyChanged(nameof(Achievements));
            OnPropertyChanged(nameof(ArrayAchievements));
        }
    }
}