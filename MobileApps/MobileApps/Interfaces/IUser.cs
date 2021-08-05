using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MobileApps.Interfaces
{
    public interface IUser
    {
        public int Id { get; set; }
        [JsonProperty("username")]
        string Username { get; set; }
        [JsonIgnore]
        string Password { get; set; }
        [JsonProperty("email")]
        string Email { get; set; }
        [JsonProperty("firstName")]
        string FirstName { get; set; }
        [JsonProperty("lastName")]
        string LastName { get; set; }
        [JsonIgnore]
        ObservableCollection<IReport> Reports { get; }
        [JsonIgnore]
        int CountDeclined { get; }
        [JsonIgnore]
        int CountAccepted { get; }
        [JsonIgnore]
        int CountProcessing { get; }
        Task Update(string ipUrl);
        Task UpdateReports(string ipUrl);
        [JsonIgnore]
        ObservableCollection<IAchievement> Achievements { get; set; }
        Task<IEnumerable<IAchievement>> GetAllAchievements(string ipUrl);
        Task GetGivenAchievements(string ipUrl);
    }
}