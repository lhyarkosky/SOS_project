using System;
using System.Collections.Generic;
using SOS_API.Models.Players;

namespace SOS_API.Models.GameStates
{
    public enum GameStatus
    {
        InProgress,
        Finished
    }

    public enum GameMode
    {
        Simple,
        General
    }

    public interface IGameState
    {
        string GameId { get; set; }
        SOS_Board Board { get; set; }
        IPlayer CurrentPlayer { get; set; }
        GameStatus Status { get; set; }
        GameMode Mode { get; }
        List<SOSSequence> CompletedSequences { get; set; }
        Dictionary<IPlayer, int> Scores { get; set; }
        IPlayer? Winner { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime LastMoveAt { get; set; }
        List<IPlayer> Players { get; set; }

        bool IsGameOver();
        string DetermineWinner();
        bool PlayerGetsAnotherTurn(int newSequencesCount);
        void AddSequences(List<SOSSequence> sequences, IPlayer player);
    }
}