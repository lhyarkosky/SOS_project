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

        public Task<(IGameState game, List<object> moves)> CreateGame(int boardSize, string gameMode, IPlayer player1, IPlayer player2)
        {
            try
            {
                var players = new List<IPlayer> { player1, player2 };

                // Let the business logic and models handle their own validation
                var game = GameEngine.CreateGameState(gameMode, boardSize, players);
                
                // Store in our dictionary
                _games[game.GameId] = game;

                var allMoves = new List<object>();

                // If the current player is AI, make moves until no more AI or game over
                while (game.Status == GameStatus.InProgress && game.CurrentPlayer.IsComputer)
                {
                    var movePlayer = game.CurrentPlayer;
                    var aiPlayer = movePlayer as AIPlayer;
                    if (aiPlayer == null)
                        break;
                    var move = aiPlayer.MakeMove(game.Board);
                    var (updatedGame, newSequences) = ApplyMove(game.GameId, move.row, move.col, move.letter);
                    game = updatedGame;
                    var moveObj = BuildMoveHistoryObject(movePlayer.Name, movePlayer.IsComputer, move.row, move.col, move.letter, newSequences, "AI move");
                    allMoves.Add(moveObj);
                    game.MoveHistory.Add(moveObj);
                }

                return Task.FromResult((game, allMoves));
            }
            catch (ArgumentOutOfRangeException e)
            {
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
        public Task<(IGameState game, List<object> moves)> MakeMove(string gameId, int row, int col, char letter)
        {
            var moves = new List<object>();
            // Capture the player before the move is processed
            var game = _games.TryGetValue(gameId, out IGameState? g) ? g : null;
            var movePlayer = game?.CurrentPlayer;
            var isAI = movePlayer?.IsComputer ?? false;
            var playerName = movePlayer?.Name ?? "Unknown";

            var (updatedGame, newSequences) = ApplyMove(gameId, row, col, letter);
            game = updatedGame;

            var moveObj = BuildMoveHistoryObject(playerName, isAI, row, col, letter, newSequences);
            moves.Add(moveObj);
            game.MoveHistory.Add(moveObj);

            // After human/AI move, process AI moves if needed
            while (game.Status == GameStatus.InProgress && game.CurrentPlayer.IsComputer)
            {
                var movePlayerAI = game.CurrentPlayer;
                var aiPlayer = movePlayerAI as AIPlayer;
                if (aiPlayer == null)
                    break;
                var aiMove = aiPlayer.MakeMove(game.Board);
                var (aiGame, aiSequences) = ApplyMove(game.GameId, aiMove.row, aiMove.col, aiMove.letter);
                game = aiGame;
                var aiMoveObj = BuildMoveHistoryObject(movePlayerAI.Name, movePlayerAI.IsComputer, aiMove.row, aiMove.col, aiMove.letter, aiSequences, "AI move");
                moves.Add(aiMoveObj);
                game.MoveHistory.Add(aiMoveObj);
            }
            return Task.FromResult((game, moves));
        }

        // Helper method to build move history objects
        private object BuildMoveHistoryObject(string playerName, bool isAI, int row, int col, char letter, List<SOSSequence> newSequences, string moveType = "Move")
        {
            return new
            {
                player = playerName,
                isAI = isAI,
                move = new { row, col, letter },
                sosFormed = newSequences.Count > 0,
                newSequencesCount = newSequences.Count,
                newSequences = newSequences.Select(s => new {
                    positions = s.Positions.Select(pos => new { row = pos.Item1, col = pos.Item2 }).ToList(),
                    foundBy = s.FoundBy
                }).ToList(),
                message = newSequences.Count > 0
                    ? $"{moveType} successful! {newSequences.Count} SOS sequence(s) formed!"
                    : $"{moveType} successful!"
            };
        }
    }
}