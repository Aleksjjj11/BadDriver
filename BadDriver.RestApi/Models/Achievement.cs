using System.ComponentModel.DataAnnotations.Schema;

namespace BadDriver.RestApi.Models
{
    [Table("achievements")]
    public class Achievement
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("description")]
        public string Description { get; set; }
    }
}