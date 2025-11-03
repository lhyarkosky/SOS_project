using System;
using System.Collections.Generic;

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
        string CurrentPlayer { get; set; }
        GameStatus Status { get; set; }
        GameMode Mode { get; }
        List<SOSSequence> CompletedSequences { get; set; }
        Dictionary<string, int> Scores { get; set; }
        string? Winner { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime LastMoveAt { get; set; }

        bool IsGameOver();
        string DetermineWinner();
        bool PlayerGetsAnotherTurn(int newSequencesCount);
        void AddSequences(List<SOSSequence> sequences, string player);
    }
}