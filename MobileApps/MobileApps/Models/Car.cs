using MobileApps.Interfaces;
using Newtonsoft.Json;

namespace MobileApps.Models
{
    public class Car : ICar
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("number")]
        public string Number { get; set; }
        [JsonProperty("region_code")]
        public string Region { get; set; }
        [JsonProperty("country_code")]
        public string Country { get; set; }

        public Car()
        {
        }

        public Car(string number, string region, string country)
        {
            Number = number;
            Region = region;
            Country = country;
        }
    }
}