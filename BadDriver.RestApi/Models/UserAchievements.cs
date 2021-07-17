using System.ComponentModel.DataAnnotations.Schema;

namespace BadDriver.RestApi.Models
{
    [Table("user_achievements")]
    public class UserAchievements
    {
        [Column("id")]
        public int Id { get; set; }
        [ForeignKey("user_achievements_users_id_fk")]
        [Column("user_id")]
        public int UserId { get; set; }

        [ForeignKey("user_achievements_achievements_id_fk")]
        [Column("achievement_id")]
        public int AchievementId { get; set; }
    }
}