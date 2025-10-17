using System;
using System.Collections.Generic;
using PalindromeCheckerApi.Models;
using PalindromeCheckerApi.Models.GameStates;

namespace PalindromeCheckerApi.BusinessLogic
{
    public static class Game
    {
        // create game state based on mode and boardSize
        public static IGameState CreateGameState(string gameMode, int boardSize)
        {
            var gameId = Guid.NewGuid().ToString();
            var board = new SOS_Board(boardSize);
            
            IGameState gameState = gameMode.ToLower() switch
            {
                "simple" => new SimpleGameState { GameId = gameId, Board = board },
                "general" => new GeneralGameState { GameId = gameId, Board = board },
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
                gameState.Winner = gameState.DetermineWinner();
            }
            else
            {
                // Handle turn switching based on game mode specific rules
                if (!gameState.PlayerGetsAnotherTurn(newSequencesCount))
                {
                    gameState.CurrentPlayer = GetNextPlayer(gameState.CurrentPlayer);
                }
            }
        }


        // Get the next player in turn order
        private static string GetNextPlayer(string currentPlayer)
        {
            return currentPlayer == "Player1" ? "Player2" : "Player1";
        }
    }
}