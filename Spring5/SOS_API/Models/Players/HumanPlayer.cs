using SOS_API.Models.Players;

namespace SOS_API.Models.Players
{
    public class HumanPlayer : IPlayer
    {
        public string Name { get; set; } = string.Empty;
        public bool IsComputer { get; } = false;
    }
}