using System;
using System.Collections.Generic;
using System.Linq;

namespace SOS_API.Models.GameStates
{
    public abstract class BaseGameState : IGameState
    {
        public required string GameId { get; set; }
        public required SOS_Board Board { get; set; }
        public string CurrentPlayer { get; set; }
        public GameStatus Status { get; set; }
        public abstract GameMode Mode { get; }
        public List<SOSSequence> CompletedSequences { get; set; }
        public Dictionary<string, int> Scores { get; set; }
        public string? Winner { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastMoveAt { get; set; }

        protected BaseGameState()
        {
            CompletedSequences = new List<SOSSequence>();
            Scores = new Dictionary<string, int>
            {
                { "Player1", 0 },
                { "Player2", 0 }
            };
            CurrentPlayer = "Player1";
            Status = GameStatus.InProgress;
            CreatedAt = DateTime.UtcNow;
            LastMoveAt = DateTime.UtcNow;
        }

        public void AddSequences(List<SOSSequence> sequences, string player)
        {
            CompletedSequences.AddRange(sequences);
            Scores[player] += sequences.Count;
        }

        public abstract bool IsGameOver();
        public abstract string DetermineWinner();
        public abstract bool PlayerGetsAnotherTurn(int newSequencesCount);

        protected bool IsBoardFull()
        {
            for (int row = 0; row < Board.Size; row++)
            {
                for (int col = 0; col < Board.Size; col++)
                {
                    if (Board.IsCellEmpty(row, col))
                        return false;
                }
            }
            return true;
        }
    }
}