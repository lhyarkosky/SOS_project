using Microsoft.AspNetCore.Mvc;
using SOS_API.Models.DTOs;
using SOS_API.Services;
using SOS_API.Models.GameStates;
using SOS_API.Models.Players;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SOS_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class SOSController(IGameService _gameService) : ControllerBase
    {
        /// <summary>
        /// Creates a new SOS game with specified settings
        /// </summary>
        /// <param name="request">Game configuration including board size, game mode, and player details</param>
        /// <returns>The newly created game state</returns>
        /// <response code="200">Game created successfully</response>
        /// <response code="500">An error occurred during game creation</response>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateGame([FromBody] CreateGameRequest request)
        {
            try
            {
                // Create player objects based on request data
                var player1 = CreatePlayer(request.Player1Name ?? "Player1", request.Player1Type);
                var player2 = CreatePlayer(request.Player2Name ?? "Player2", request.Player2Type);
                
                var (game, moves) = await _gameService.CreateGame(request.BoardSize, request.GameMode, player1, player2);
                
                var gameData = ApiUtilities.SerializeGameForApi(game);
                
                return Ok(new
                {
                    game = gameData,
                    moves = moves
                });
            }
            catch (Exception e)
            {
                return StatusCode(500, $"An error occurred while creating the game: {e.Message}");
            }
        }

        private IPlayer CreatePlayer(string name, PlayerType playerType)
        {
            return playerType switch
            {
                PlayerType.Human => new HumanPlayer { Name = name },
                PlayerType.Computer => new AIPlayer{ Name = name},
                _ => throw new ArgumentException($"Unknown PlayerType: {playerType}")
            };
        }

        /// <summary>
        /// Makes a move in an existing game
        /// </summary>
        /// <param name="request">Move details including game ID, position (row, col), and letter (S or O)</param>
        /// <returns>Updated game state and information about any SOS sequences formed</returns>
        /// <response code="200">Move executed successfully</response>
        /// <response code="500">An error occurred during move execution</response>
        [HttpPost("move")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> MakeMove([FromBody] MakeMoveRequest request)
        {
            try
            {
                var (game, moves) = await _gameService.MakeMove(request.GameId, request.Row, request.Col, request.Letter);
                
                var gameData = ApiUtilities.SerializeGameForApi(game);
                
                return Ok(new
                {
                    game = gameData,
                    moves = moves
                });
            }
            catch (Exception e)
            {
                return StatusCode(500, $"An error occurred while making the move: {e.Message}");
            }
        }

        /// <summary>
        /// Retrieves the current state of a specific game
        /// </summary>
        /// <param name="gameId">Unique identifier of the game</param>
        /// <returns>Current game state including board, players, and scores</returns>
        /// <response code="200">Game found and returned successfully</response>
        /// <response code="404">Game not found</response>
        /// <response code="500">An error occurred while retrieving the game</response>
        [HttpGet("{gameId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<object> GetGame(string gameId)
        {
            try
            {
                var game = _gameService.GetGame(gameId);
                
                if (game == null)
                {
                    return NotFound("Game not found");
                }

                return Ok(ApiUtilities.SerializeGameForApi(game));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"An error occurred while retrieving the game: {e.Message}");
            }
        }

        /// <summary>
        /// Deletes a game by its ID
        /// </summary>
        /// <param name="gameId">Unique identifier of the game to delete</param>
        /// <returns>Confirmation message</returns>
        /// <response code="200">Game deleted successfully</response>
        /// <response code="404">Game not found</response>
        /// <response code="500">An error occurred while deleting the game</response>
        [HttpDelete("{gameId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<object> DeleteGame(string gameId)
        {
            try
            {
                bool deleted = _gameService.DeleteGame(gameId);
                
                if (!deleted)
                {
                    return NotFound("Game not found");
                }

                return Ok("Game deleted successfully");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"An error occurred while deleting the game: {e.Message}");
            }
        }

        /// <summary>
        /// Retrieves all active games (for debugging/admin purposes)
        /// </summary>
        /// <returns>List of all game states</returns>
        /// <response code="200">Games retrieved successfully</response>
        /// <response code="500">An error occurred while retrieving games</response>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<object> GetAllGames()
        {
            try
            {
                var games = _gameService.GetAllGames();
                
                return Ok(games.Select(ApiUtilities.SerializeGameForApi).ToList());
            }
            catch (Exception e)
            {
                return StatusCode(500, $"An error occurred while retrieving games: {e.Message}");
            }
        }

        /// <summary>
        /// Cleans up games older than the specified number of hours
        /// </summary>
        /// <param name="hoursOld">Number of hours (default: 24). Games older than this will be deleted</param>
        /// <returns>Number of games cleaned up</returns>
        /// <response code="200">Cleanup completed successfully</response>
        /// <response code="500">An error occurred during cleanup</response>
        [HttpPost("cleanup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<object> CleanupOldGames([FromQuery] int hoursOld = 24)
        {
            try
            {
                int cleanedUp = _gameService.CleanupOldGames(hoursOld);
                
                return Ok($"Cleaned up {cleanedUp} old games");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"An error occurred while cleaning up games: {e.Message}");
            }
        }

        /// <summary>
        /// Returns the full move history for a game
        /// </summary>
        /// <param name="gameId">Unique identifier of the game</param>
        /// <returns>List of all moves made in the game</returns>
        /// <response code="200">Move history returned successfully</response>
        /// <response code="404">Game not found</response>
        [HttpGet("{gameId}/moveHistory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<object> GetMoveHistory(string gameId)
        {
            var game = _gameService.GetGame(gameId);
            if (game == null)
            {
                return NotFound("Game not found");
            }
            return Ok(game.MoveHistory ?? new List<object>());
        }
    }
}