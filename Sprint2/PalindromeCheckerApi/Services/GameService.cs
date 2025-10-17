using System;
using System.Collections.Generic;
using System.Linq;
using PalindromeCheckerApi.BusinessLogic;
using PalindromeCheckerApi.Models;
using PalindromeCheckerApi.Models.GameStates;

namespace PalindromeCheckerApi.Services
{
    public class GameService : IGameService
    {
        private static Dictionary<string, IGameState> _games = new Dictionary<string, IGameState>(); // if this was a bigger project id use db rather than a dictionary, but this should suffice for now

        public IGameState CreateGame(int boardSize, string gameMode)
        {
            // create the game state
            var game = Game.CreateGameState(gameMode, boardSize);
            
            // Store in our dictionary
            _games[game.GameId] = game;
            return game;
        }

        public (IGameState game, List<SOSSequence> newSequences) MakeMove(string gameId, int row, int col, char letter)
        {
            // Service layer: handle game retrieval
            if (!_games.TryGetValue(gameId, out IGameState? game))
                throw new ArgumentException("Game not found");

            // Business logic layer: process the move
            var (updatedGame, newSequences) = Game.ProcessMove(game, row, col, letter);
            
            // Service layer: update storage
            _games[gameId] = updatedGame;
            
            return (updatedGame, newSequences);
        }

        public IGameState? GetGame(string gameId)
        {
            return _games.TryGetValue(gameId, out IGameState? game) ? game : null;
        }

        public bool DeleteGame(string gameId)
        {
            return _games.Remove(gameId);
        }

        public List<IGameState> GetAllGames()
        {
            return _games.Values.ToList();
        }

        public int CleanupOldGames(int hoursOld = 24)
        {
            var cutoffTime = DateTime.UtcNow.AddHours(-hoursOld);
            var gamesToRemove = _games.Where(kvp => kvp.Value.LastMoveAt < cutoffTime).ToList();
            
            foreach (var game in gamesToRemove)
            {
                _games.Remove(game.Key);
            }

            return gamesToRemove.Count;
        }
    }
}