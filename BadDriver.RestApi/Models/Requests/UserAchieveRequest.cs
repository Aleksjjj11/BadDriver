using System.ComponentModel.DataAnnotations;

namespace BadDriver.RestApi.Models.Requests
{
    public class UserAchieveRequest
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int AchievementId { get; set; }
    }
}