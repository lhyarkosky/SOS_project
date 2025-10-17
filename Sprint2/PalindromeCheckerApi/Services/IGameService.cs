using System.Collections.Generic;
using PalindromeCheckerApi.Models;
using PalindromeCheckerApi.Models.GameStates;

namespace PalindromeCheckerApi.Services
{
    public interface IGameService
    {
        IGameState CreateGame(int boardSize, string gameMode);
        (IGameState game, List<SOSSequence> newSequences) MakeMove(string gameId, int row, int col, char letter);
        IGameState? GetGame(string gameId);
        bool DeleteGame(string gameId);
        List<IGameState> GetAllGames(); //  for testing and later on possibly displaying active games waiting for opponent
        int CleanupOldGames(int hoursOld); // for cleaning up old games
    }
}