using System;
using System.Collections.Generic;
using System.Linq;
using SOS_API.BusinessLogic;
using SOS_API.Models;
using SOS_API.Models.GameStates;

namespace SOS_API.Services
{
    public class GameService : IGameService
    {
        private static Dictionary<string, IGameState> _games = new Dictionary<string, IGameState>(); // if this was a bigger project id use db rather than a dictionary, but this should suffice for now

        public IGameState CreateGame(int boardSize, string gameMode)
        {
            try
            {
                // Let the business logic and models handle their own validation
                var game = Game.CreateGameState(gameMode, boardSize);
                
                // Store in our dictionary
                _games[game.GameId] = game;
                return game;
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Convert ArgumentOutOfRangeException from SOS_Board to ArgumentException for API consistency
                throw new ArgumentException(e.Message, e);
            }
            // ArgumentException from Game.CreateGameState (invalid game mode) bubbles up as-is
        }

        public (IGameState game, List<SOSSequence> newSequences) MakeMove(string gameId, int row, int col, char letter)
        {
            // Service layer: handle game retrieval
            if (!_games.TryGetValue(gameId, out IGameState? game))
                throw new ArgumentException("Game not found");

            try
            {
                // Business logic layer: process the move - let domain exceptions bubble up
                var (updatedGame, newSequences) = Game.ProcessMove(game, row, col, letter);
                
                // Service layer: update storage
                _games[gameId] = updatedGame;
                
                return (updatedGame, newSequences);
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Convert position validation errors to ArgumentException for API consistency
                throw new ArgumentException(e.Message, e);
            }
            // InvalidOperationException and other domain exceptions bubble up as-is
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