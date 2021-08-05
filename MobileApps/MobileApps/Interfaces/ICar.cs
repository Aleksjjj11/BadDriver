namespace MobileApps.Interfaces
{
    public interface ICar
    {
        int Id { get; set; }
        string Number { get; set; }
        string Region { get; set; }
        string Country { get; set; }
    }
}