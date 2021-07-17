using System.ComponentModel.DataAnnotations;

namespace BadDriver.RestApi.Models.Requests
{
    public class CreateUserRequest
    {
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Логин может быть длинной 3-100 символов")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }

        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некорректный email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string LastName { get; set; }
    }
}