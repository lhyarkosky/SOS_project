using System;
using SOS_API.Models;

namespace SOS_API.BusinessLogic
{
    public static class AIMoveLogic
    {
        // Returns a random valid move (row, col, letter)
        public static (int row, int col, char letter) GetMove(SOS_Board board)
        {
            var rand = new Random();
            var emptyCells = new System.Collections.Generic.List<(int row, int col)>();
            for (int row = 0; row < board.Size; row++)
            {
                for (int col = 0; col < board.Size; col++)
                {
                    if (board.IsCellEmpty(row, col))
                        emptyCells.Add((row, col));
                }
            }
            if (emptyCells.Count == 0)
                throw new InvalidOperationException("No moves available");

            var cell = emptyCells[rand.Next(emptyCells.Count)];
            char letter = rand.Next(2) == 0 ? 'S' : 'O';
            return (cell.row, cell.col, letter);
        }
    }
}
