using System;
using System.Collections.Generic;
using System.Linq;
using SOS_API.Models.Players;

namespace SOS_API.Models.GameStates
{
    public abstract class BaseGameState : IGameState
    {
        public required string GameId { get; set; }
        public required SOS_Board Board { get; set; }
        public IPlayer CurrentPlayer { get; set; }
        public GameStatus Status { get; set; }
        public abstract GameMode Mode { get; }
        public List<SOSSequence> CompletedSequences { get; set; }
        public Dictionary<IPlayer, int> Scores { get; set; }
        public IPlayer? Winner { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastMoveAt { get; set; }
        public List<IPlayer> Players { get; set; }

        protected BaseGameState(List<IPlayer> players)
        {
            if (players == null || players.Count != 2)
                throw new ArgumentException("Exactly 2 players are required");

            Players = players;
            CompletedSequences = new List<SOSSequence>();
            Scores = new Dictionary<IPlayer, int>();
            
            foreach (var player in players)
            {
                Scores[player] = 0;
            }
            
            CurrentPlayer = players[0];
            Status = GameStatus.InProgress;
            CreatedAt = DateTime.UtcNow;
            LastMoveAt = DateTime.UtcNow;
        }

        public void AddSequences(List<SOSSequence> sequences, IPlayer player)
        {
            CompletedSequences.AddRange(sequences);
            if (Scores.ContainsKey(player))
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