using System.Collections.Generic;
using SOS_API.Models;

namespace SOS_API.BusinessLogic
{
    public static class SOSPatternDetector
    {
        // Find all SOS sequences that include the last placed letter, track the sequences found, the direction (for drawing line through seq's), and the player who found them (for color of line).
        public static List<SOSSequence> FindNewSOSSequences(SOS_Board board, int lastRow, int lastCol, string currentPlayer)
        {
            var newSequences = new List<SOSSequence>();
            char? lastLetter = board.GetCell(lastRow, lastCol);

            if (!lastLetter.HasValue)
                return newSequences;

            // Check all possible SOS patterns that could include this position
            newSequences.AddRange(FindHorizontalSOS(board, lastRow, lastCol, currentPlayer));
            newSequences.AddRange(FindVerticalSOS(board, lastRow, lastCol, currentPlayer));
            newSequences.AddRange(FindDiagonalSOS(board, lastRow, lastCol, currentPlayer));

            return newSequences;
        }

        private static List<SOSSequence> FindHorizontalSOS(SOS_Board board, int row, int col, string player)
        {
            var sequences = new List<SOSSequence>();

            // Check if this position is the middle 'O' of SOS
            if (board.GetCell(row, col) == 'O' && col >= 1 && col < board.Size - 1)
            {
                if (board.GetCell(row, col - 1) == 'S' && board.GetCell(row, col + 1) == 'S')
                {
                    sequences.Add(new SOSSequence(
                        new List<(int, int)> { (row, col - 1), (row, col), (row, col + 1) },
                        "horizontal",
                        player));
                }
            }

            // Check if this position is the left 'S' of SOS
            if (board.GetCell(row, col) == 'S' && col <= board.Size - 3)
            {
                if (board.GetCell(row, col + 1) == 'O' && board.GetCell(row, col + 2) == 'S')
                {
                    sequences.Add(new SOSSequence(
                        new List<(int, int)> { (row, col), (row, col + 1), (row, col + 2) },
                        "horizontal",
                        player));
                }
            }

            // Check if this position is the right 'S' of SOS
            if (board.GetCell(row, col) == 'S' && col >= 2)
            {
                if (board.GetCell(row, col - 1) == 'O' && board.GetCell(row, col - 2) == 'S')
                {
                    sequences.Add(new SOSSequence(
                        new List<(int, int)> { (row, col - 2), (row, col - 1), (row, col) },
                        "horizontal",
                        player));
                }
            }

            return sequences;
        }

        private static List<SOSSequence> FindVerticalSOS(SOS_Board board, int row, int col, string player)
        {
            var sequences = new List<SOSSequence>();

            // Check if this position is the middle 'O' of SOS
            if (board.GetCell(row, col) == 'O' && row >= 1 && row < board.Size - 1)
            {
                if (board.GetCell(row - 1, col) == 'S' && board.GetCell(row + 1, col) == 'S')
                {
                    sequences.Add(new SOSSequence(
                        new List<(int, int)> { (row - 1, col), (row, col), (row + 1, col) },
                        "vertical",
                        player));
                }
            }

            // Check if this position is the top 'S' of SOS
            if (board.GetCell(row, col) == 'S' && row <= board.Size - 3)
            {
                if (board.GetCell(row + 1, col) == 'O' && board.GetCell(row + 2, col) == 'S')
                {
                    sequences.Add(new SOSSequence(
                        new List<(int, int)> { (row, col), (row + 1, col), (row + 2, col) },
                        "vertical",
                        player));
                }
            }

            // Check if this position is the bottom 'S' of SOS
            if (board.GetCell(row, col) == 'S' && row >= 2)
            {
                if (board.GetCell(row - 1, col) == 'O' && board.GetCell(row - 2, col) == 'S')
                {
                    sequences.Add(new SOSSequence(
                        new List<(int, int)> { (row - 2, col), (row - 1, col), (row, col) },
                        "vertical",
                        player));
                }
            }

            return sequences;
        }

        private static List<SOSSequence> FindDiagonalSOS(SOS_Board board, int row, int col, string player)
        {
            var sequences = new List<SOSSequence>();

            // Check diagonal (top-left to bottom-right)
            sequences.AddRange(FindDiagonalSOSDirection(board, row, col, player, -1, -1, "diagonal-right"));
            
            // Check diagonal (top-right to bottom-left)
            sequences.AddRange(FindDiagonalSOSDirection(board, row, col, player, -1, 1, "diagonal-left"));

            return sequences;
        }

        private static List<SOSSequence> FindDiagonalSOSDirection(SOS_Board board, int row, int col, string player, int rowDir, int colDir, string direction)
        {
            var sequences = new List<SOSSequence>();

            // Check if this position is the middle 'O' of SOS
            if (board.GetCell(row, col) == 'O')
            {
                int prevRow = row - rowDir, prevCol = col - colDir;
                int nextRow = row + rowDir, nextCol = col + colDir;

                if (board.IsValidPosition(prevRow, prevCol) && board.IsValidPosition(nextRow, nextCol))
                {
                    if (board.GetCell(prevRow, prevCol) == 'S' && board.GetCell(nextRow, nextCol) == 'S')
                    {
                        sequences.Add(new SOSSequence(
                            new List<(int, int)> { (prevRow, prevCol), (row, col), (nextRow, nextCol) },
                            direction,
                            player));
                    }
                }
            }

            // Check if this position is the first 'S' of SOS
            if (board.GetCell(row, col) == 'S')
            {
                int midRow = row + rowDir, midCol = col + colDir;
                int endRow = row + 2 * rowDir, endCol = col + 2 * colDir;

                if (board.IsValidPosition(midRow, midCol) && board.IsValidPosition(endRow, endCol))
                {
                    if (board.GetCell(midRow, midCol) == 'O' && board.GetCell(endRow, endCol) == 'S')
                    {
                        sequences.Add(new SOSSequence(
                            new List<(int, int)> { (row, col), (midRow, midCol), (endRow, endCol) },
                            direction,
                            player));
                    }
                }
            }

            // Check if this position is the last 'S' of SOS
            if (board.GetCell(row, col) == 'S')
            {
                int midRow = row - rowDir, midCol = col - colDir;
                int startRow = row - 2 * rowDir, startCol = col - 2 * colDir;

                if (board.IsValidPosition(midRow, midCol) && board.IsValidPosition(startRow, startCol))
                {
                    if (board.GetCell(midRow, midCol) == 'O' && board.GetCell(startRow, startCol) == 'S')
                    {
                        sequences.Add(new SOSSequence(
                            new List<(int, int)> { (startRow, startCol), (midRow, midCol), (row, col) },
                            direction,
                            player));
                    }
                }
            }

            return sequences;
        }
    }
}