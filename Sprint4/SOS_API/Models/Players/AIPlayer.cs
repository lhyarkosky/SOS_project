using System;

namespace SOS_API.Models.Players
{
    public class AIPlayer : IPlayer
    {
        public string Name { get; set; }
        public bool IsComputer => true;

        public AIPlayer(string name = "AI Player")
        {
            Name = name;
        }

        // Delegates move selection to BusinessLogic layer
        public (int row, int col, char letter) MakeMove(SOS_Board board)
        {
            return BusinessLogic.AIMoveLogic.GetMove(board);
        }
    }
}
