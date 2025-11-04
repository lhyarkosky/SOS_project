using System;
using System.Collections.Generic;
using System.Linq;
using SOS_API.Models;
using SOS_API.Models.Players;
using SOS_API.Models.GameStates;

namespace SOS_API.BusinessLogic
{
    public static class GameEngine
    {
        // create game state based on mode, boardSize, and players
        public static IGameState CreateGameState(string gameMode, int boardSize, List<IPlayer> players)
        {
            if (players == null || players.Count != 2)
                throw new ArgumentException("Exactly 2 players are required");

            var gameId = Guid.NewGuid().ToString();
            
            SOS_Board board;
            try
            {
                board = new SOS_Board(boardSize);
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Convert to ArgumentException for consistent API error handling
                throw new ArgumentException($"Invalid board size: {e.Message}");
            }
            
            IGameState gameState = gameMode.ToLower() switch
            {
                "simple" => new SimpleGameState(players) { GameId = gameId, Board = board },
                "general" => new GeneralGameState(players) { GameId = gameId, Board = board },
                _ => throw new ArgumentException($"Unknown game mode: {gameMode}")
            };
            gameState.Status = GameStatus.InProgress;

            return gameState;
        }

        // Process a complete move including validation, placement, and state updates
        public static (IGameState gameState, List<SOSSequence> newSequences) ProcessMove(
            IGameState gameState, int row, int col, char letter)
        {
            ValidateMove(gameState, row, col);
            gameState.Board.SetCell(row, col, letter);
            gameState.LastMoveAt = DateTime.UtcNow;

            var newSequences = SOSPatternDetector.FindNewSOSSequences(
                gameState.Board, row, col, gameState.CurrentPlayer);

            gameState.AddSequences(newSequences, gameState.CurrentPlayer);

            ProcessEndOfTurn(gameState, newSequences.Count);

            return (gameState, newSequences);
        }


        // Validate that a move is legal
        private static void ValidateMove(IGameState gameState, int row, int col)
        {
            if (gameState.Status != GameStatus.InProgress)
                throw new InvalidOperationException("Game is not in progress");

            if (!gameState.Board.IsValidPosition(row, col))
                throw new ArgumentException("Position is out of bounds");

            if (!gameState.Board.IsCellEmpty(row, col))
                throw new InvalidOperationException("Cell is already occupied");
        }

        // end-of-turn logic including game over checks and turn switching

        private static void ProcessEndOfTurn(IGameState gameState, int newSequencesCount)
        {
            // Check if game is over
            if (gameState.IsGameOver())
            {
                gameState.Status = GameStatus.Finished;
                var winnerName = gameState.DetermineWinner();
                gameState.Winner = gameState.Players.FirstOrDefault(p => p.Name == winnerName);
            }
            else
            {
                // Handle turn switching based on game mode specific rules
                if (!gameState.PlayerGetsAnotherTurn(newSequencesCount))
                {
                    gameState.CurrentPlayer = GetNextPlayer(gameState);
                }
            }
        }


        // Get the next player in turn order
        private static IPlayer GetNextPlayer(IGameState gameState)
        {
            var currentIndex = gameState.Players.IndexOf(gameState.CurrentPlayer);
            var nextIndex = (currentIndex + 1) % gameState.Players.Count;
            return gameState.Players[nextIndex];
        }
    }
}