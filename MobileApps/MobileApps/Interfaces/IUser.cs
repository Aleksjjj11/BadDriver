using System.Collections.ObjectModel;

namespace MobileApps.Interfaces
{
    public interface IUser
    {
        string Username { get; set; }
        string Password { get; set; }
        string Email { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        ObservableCollection<IReport> Reports { get; }
        int CountDeclined { get; }
        int CountAccepted { get; }
        int CountProcessing { get; }
    }
}