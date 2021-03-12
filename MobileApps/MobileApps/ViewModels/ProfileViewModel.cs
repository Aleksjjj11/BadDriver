using MobileApps.Interfaces;
using MobileApps.Models;

namespace MobileApps.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        public IUser User { get; set; }
        public ProfileViewModel()
        {
            User = new User
            {
                Username = "Taske",
                Email = "Taske@mail.ru",
                FirstName = "Фамилия",
                LastName = "Имя"
            };
        }
    }
}