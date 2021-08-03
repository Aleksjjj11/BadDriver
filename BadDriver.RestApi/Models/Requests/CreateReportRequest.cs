using System.ComponentModel.DataAnnotations;

namespace BadDriver.RestApi.Models.Requests
{
    public class CreateReportRequest
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public Car Car { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string ImageUrl1 { get; set; }
        [Required]
        public string ImageUrl2 { get; set; }
        [Required]
        public string ImageUrl3 { get; set; }
    }
}