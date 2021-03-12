using MobileApps.Interfaces;

namespace MobileApps.Models
{
    public class Car : ICar
    {
        public string Number { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }

        public Car(string number, string region, string country)
        {
            Number = number;
            Region = region;
            Country = country;
        }
    }
}