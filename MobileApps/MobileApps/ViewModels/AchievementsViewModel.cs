using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
//using Android.Widget;
using MobileApps.Interfaces;
using MobileApps.Models;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
//using Application = Android.App.Application;

namespace MobileApps.ViewModels
{
    public class AchievementsViewModel : BaseViewModel
    {
        public ObservableCollection<IAchievement> Achievements => App.CurrentUser.Achievements;
        public IAchievement[] ArrayAchievements => Achievements.ToArray();


        private BackgroundWorker _bwAchievementUpdater;
        private bool _isBusy;
        private Page _ownPage;

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
            _bwAchievementUpdater = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };
            _bwAchievementUpdater.DoWork += BwAchievementUpdaterOnDoWork;
            _bwAchievementUpdater.ProgressChanged += BwAchievementUpdaterOnProgressChanged;
            _bwAchievementUpdater.RunWorkerCompleted += BwAchievementUpdaterOnRunWorkerCompleted;
        }

        private void BwAchievementUpdaterOnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            
        }

        public ICommand RefreshUser => new Command(() =>
        {
            _bwAchievementUpdater.RunWorkerAsync();
        });

        private async void BwAchievementUpdaterOnDoWork(object sender, DoWorkEventArgs e)
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
            //if(e.Error is null)
            //    Toast.MakeText(Application.Context, "Успех, вот ачивки!", ToastLength.Long)?.Show();
            //else
            //    Toast.MakeText(Application.Context, $"Крах! Вот ошибка\n{e.Error.Message}", ToastLength.Long)?.Show();
            
            OnPropertyChanged(nameof(Achievements));
            OnPropertyChanged(nameof(ArrayAchievements));
        }
    }
}