using System.ComponentModel.DataAnnotations.Schema;

namespace BadDriver.RestApi.Models
{
    [Table("reports")]
    public class Report
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("description")]
        public string Description { get; set; }
        [Column("img_url1")]
        public string ImageUrl1 { get; set; }
        [Column("img_url2")]
        public string ImageUrl2 { get; set; }
        [Column("img_url3")]
        public string ImageUrl3 { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("car_id")]
        public int CarId { get; set; }
    }
}