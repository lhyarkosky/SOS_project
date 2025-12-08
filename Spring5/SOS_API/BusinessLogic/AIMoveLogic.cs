using System;
using System.Collections.Generic;
using System.Linq;
using SOS_API.Models;
using SOS_API.Models.Players;

namespace SOS_API.BusinessLogic
{
    public static class AIMoveLogic
    {
        // Returns a smart move: prioritizes scoring, then safe moves, then random
        public static (int row, int col, char letter) GetMove(SOS_Board board, IPlayer aiPlayer)
        {
            var rand = new Random();
            var emptyCells = new List<(int row, int col)>();
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

            // First, check for scoring moves
            var scoringMoves = new List<(int row, int col, char letter)>();
            foreach (var (row, col) in emptyCells)
            {
                foreach (char letter in new[] { 'S', 'O' })
                {
                    board.SetCell(row, col, letter);
                    var sequences = SOSPatternDetector.FindNewSOSSequences(board, row, col, aiPlayer);
                    if (sequences.Any())
                    {
                        scoringMoves.Add((row, col, letter));
                    }
                    board.ClearCell(row, col);
                }
            }
            if (scoringMoves.Any())
            {
                return scoringMoves[rand.Next(scoringMoves.Count)];
            }

            // If no scoring moves, check for safe moves (moves that don't allow opponent to score immediately)
            var safeMoves = new List<(int row, int col, char letter)>();
            var dummyOpponent = new DummyPlayer();
            foreach (var (row, col) in emptyCells)
            {
                foreach (char letter in new[] { 'S', 'O' })
                {
                    board.SetCell(row, col, letter);
                    bool isSafe = true;
                    var remainingEmpty = emptyCells.Where(c => c.row != row || c.col != col).ToList();
                    foreach (var (oppRow, oppCol) in remainingEmpty)
                    {
                        foreach (char oppLetter in new[] { 'S', 'O' })
                        {
                            board.SetCell(oppRow, oppCol, oppLetter);
                            var oppSequences = SOSPatternDetector.FindNewSOSSequences(board, oppRow, oppCol, dummyOpponent);
                            if (oppSequences.Any())
                            {
                                isSafe = false;
                            }
                            board.ClearCell(oppRow, oppCol);
                            if (!isSafe) break;
                        }
                        if (!isSafe) break;
                    }
                    if (isSafe)
                    {
                        safeMoves.Add((row, col, letter));
                    }
                    board.ClearCell(row, col);
                }
            }
            if (safeMoves.Any())
            {
                return safeMoves[rand.Next(safeMoves.Count)];
            }

            // If no safe moves, pick random
            var cell = emptyCells[rand.Next(emptyCells.Count)];
            char randLetter = rand.Next(2) == 0 ? 'S' : 'O';
            return (cell.row, cell.col, randLetter);
        }

        private class DummyPlayer : IPlayer
        {
            public string Name { get; set; } = "Dummy";
            public bool IsComputer => false;
        }
    }
}
