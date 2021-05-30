using System.ComponentModel;
using System.Runtime.CompilerServices;
using MobileApps.Annotations;
using MobileApps.Interfaces;
using Newtonsoft.Json;

namespace MobileApps.Models
{
    public class Achievement : IAchievement, INotifyPropertyChanged
    {
        private string _name;

        [JsonProperty("achivment_name")]
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private string _description;

        [JsonProperty("achivment_description")]
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        private string _bigImage;

        [JsonProperty("big_image")]
        public string BigImage
        {
            get => _bigImage;
            set
            {
                _bigImage = value;
                OnPropertyChanged(nameof(BigImage));
            }
        }

        private string _smallImage;

        [JsonProperty("small_image")]
        public string SmallImage
        {
            get => _smallImage;
            set
            {
                _smallImage = value;
                OnPropertyChanged(nameof(SmallImage));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}