namespace SOS_API.Models.Players
{
    public interface IPlayer
    {
        string Name { get; set; }
        bool IsComputer { get; }
    }
}