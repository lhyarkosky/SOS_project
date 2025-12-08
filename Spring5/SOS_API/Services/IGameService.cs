using System.Collections.Generic;
using System.Threading.Tasks;
using SOS_API.Models;
using SOS_API.Models.Players;
using SOS_API.Models.GameStates;

namespace SOS_API.Services
{
    public interface IGameService
    {
        Task<(IGameState game, List<object> moves)> CreateGame(int boardSize, string gameMode, IPlayer player1, IPlayer player2);
        (IGameState game, List<SOSSequence> newSequences) ApplyMove(string gameId, int row, int col, char letter);
        Task<(IGameState game, List<object> moves)> MakeMove(string gameId, int row, int col, char letter);
        IGameState? GetGame(string gameId);
        bool DeleteGame(string gameId);
        List<IGameState> GetAllGames(); //  for testing and later on possibly displaying active games waiting for opponent
        int CleanupOldGames(int hoursOld); // for cleaning up old games
    }
}