using System;
using System.Collections.Generic;
using System.Linq;
using SOS_API.BusinessLogic;
using SOS_API.Models;
using SOS_API.Models.Players;
using SOS_API.Models.GameStates;

namespace SOS_API.Services
{
    public class GameService : IGameService
    {
        private static Dictionary<string, IGameState> _games = new Dictionary<string, IGameState>(); // if this was a bigger project id use db rather than a dictionary, but this should suffice for now

        public async Task<(IGameState game, List<object> moves)> CreateGame(int boardSize, string gameMode, IPlayer player1, IPlayer player2)
        {
            try
            {
                var players = new List<IPlayer> { player1, player2 };

                // Let the business logic and models handle their own validation
                var game = GameEngine.CreateGameState(gameMode, boardSize, players);
                
                // Store in our dictionary
                _games[game.GameId] = game;

                // If the current player is AI, make the initial move
                if (game.CurrentPlayer.IsComputer)
                {
                    var aiPlayer = game.CurrentPlayer as AIPlayer;
                    if (aiPlayer != null)
                    {
                        var move = AIMoveLogic.GetMove(game.Board);
                        var (updatedGame, moves) = await MakeMove(game.GameId, move.row, move.col, move.letter);
                        return (updatedGame, moves);
                    }
                }

                return (game, new List<object>());
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Convert ArgumentOutOfRangeException from SOS_Board to ArgumentException for API consistency
                throw new ArgumentException(e.Message, e);
            }
            // ArgumentException from GameEngine.CreateGameState (invalid game mode) bubbles up as-is
        }

        public (IGameState game, List<SOSSequence> newSequences) ApplyMove(string gameId, int row, int col, char letter)
        {
            // Service layer: handle game retrieval
            if (!_games.TryGetValue(gameId, out IGameState? game))
                throw new ArgumentException("Game not found");

            try
            {
                // Business logic layer: process the move - let domain exceptions bubble up
                var (updatedGame, newSequences) = GameEngine.ProcessMove(game, row, col, letter);
                
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

        // Handles a human move and automatically processes AI moves if needed
        public async Task<(IGameState game, List<object> moves)> MakeMove(string gameId, int row, int col, char letter)
        {
            var moves = new List<object>();
            var (game, newSequences) = ApplyMove(gameId, row, col, letter);
            moves.Add(new
            {
                player = game.CurrentPlayer.Name,
                isAI = game.CurrentPlayer.IsComputer,
                move = new { row, col, letter },
                sosFormed = newSequences.Count > 0,
                newSequencesCount = newSequences.Count,
                newSequences = newSequences.Select(s => new { positions = s.Positions, foundBy = s.FoundBy }).ToList(),
                message = newSequences.Count > 0
                    ? $"Move successful! {newSequences.Count} SOS sequence(s) formed!"
                    : "Move successful!"
            });

            // After human move, process AI moves if needed
            while (game.Status == GameStatus.InProgress && game.CurrentPlayer.IsComputer)
            {
                var aiPlayer = game.CurrentPlayer as AIPlayer;
                if (aiPlayer == null)
                    break;
                var aiMove = AIMoveLogic.GetMove(game.Board);
                var (aiGame, aiSequences) = ApplyMove(game.GameId, aiMove.row, aiMove.col, aiMove.letter);
                game = aiGame;
                moves.Add(new
                {
                    player = aiPlayer.Name,
                    isAI = true,
                    move = new { aiMove.row, aiMove.col, aiMove.letter },
                    sosFormed = aiSequences.Count > 0,
                    newSequencesCount = aiSequences.Count,
                    newSequences = aiSequences.Select(s => new { positions = s.Positions, foundBy = s.FoundBy }).ToList(),
                    message = aiSequences.Count > 0
                        ? $"AI move successful! {aiSequences.Count} SOS sequence(s) formed!"
                        : "AI move successful!"
                });
            }
            return (game, moves);
        }
    }
}