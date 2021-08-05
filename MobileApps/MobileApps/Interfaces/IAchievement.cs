namespace MobileApps.Interfaces
{
    public interface IAchievement
    {
        public int Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        string BigImage { get; set; }
        string SmallImage { get; set; }
    }
}