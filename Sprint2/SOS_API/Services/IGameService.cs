using System.Collections.Generic;
using SOS_API.Models;
using SOS_API.Models.GameStates;

namespace SOS_API.Services
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