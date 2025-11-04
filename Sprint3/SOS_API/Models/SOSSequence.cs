using System.Collections.Generic;
using SOS_API.Models.Players;

namespace SOS_API.Models
{
    // Domain model representing a detected SOS sequence in the game.
    // Contains the positions, direction, and player information.
    public class SOSSequence
    {
        public List<(int row, int col)> Positions { get; set; }
        public string Direction { get; set; } = string.Empty; // "horizontal", "vertical", "diagonal-right", "diagonal-left"
        public IPlayer? FoundBy { get; set; } // The player who found this sequence

        public SOSSequence()
        {
            Positions = new List<(int row, int col)>();
        }

        public SOSSequence(List<(int row, int col)> positions, string direction, IPlayer foundBy)
        {
            Positions = positions;
            Direction = direction;
            FoundBy = foundBy;
        }
    }
}