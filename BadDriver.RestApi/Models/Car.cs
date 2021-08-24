using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BadDriver.RestApi.Models
{
    [Table("cars")]
    public class Car
    {
        [Column("id")] 
        public int Id { get; set; }
        [Column("number")]
        public string Number { get; set; }
        [Column("region_code")]
        public string RegionCode { get; set; }
        [Column("country_code")] 
        public string CountryCode { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("last_inspection_date")]
        public DateTime LastInspectionDate { get; set; }
    }
}