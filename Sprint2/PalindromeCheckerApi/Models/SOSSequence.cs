using System.Collections.Generic;

namespace PalindromeCheckerApi.Models
{
    // Domain model representing a detected SOS sequence in the game.
    // Contains the positions, direction, and player information.
    public class SOSSequence
    {
        public List<(int row, int col)> Positions { get; set; }
        public string Direction { get; set; } = string.Empty; // "horizontal", "vertical", "diagonal-right", "diagonal-left"
        public string FoundBy { get; set; } = string.Empty; // "Player1" or "Player2"

        public SOSSequence()
        {
            Positions = new List<(int row, int col)>();
        }

        public SOSSequence(List<(int row, int col)> positions, string direction, string foundBy)
        {
            Positions = positions;
            Direction = direction;
            FoundBy = foundBy;
        }
    }
}