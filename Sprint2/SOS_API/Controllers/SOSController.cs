using Microsoft.AspNetCore.Mvc;
using SOS_API.Models.DTOs;
using SOS_API.Services;
using SOS_API.Models.GameStates;
using System;

namespace SOS_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SOSController : ControllerBase
    {
        private readonly IGameService _gameService;

        public SOSController(IGameService gameService)
        {
            _gameService = gameService;
        }



        // Create a new SOS game
        [HttpPost("create")]
        public ActionResult<object> CreateGame([FromBody] CreateGameRequest request)
        {
            try
            {
                var game = _gameService.CreateGame(request.BoardSize, request.GameMode);
                
                return Ok(ApiUtilities.SerializeGameForApi(game));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"An error occurred while creating the game: {e.Message}");
            }
        }

        // Make a move in the game
        [HttpPost("move")]
        public ActionResult<object> MakeMove([FromBody] MakeMoveRequest request)
        {
            try
            {
                var (game, newSequences) = _gameService.MakeMove(request.GameId, request.Row, request.Col, request.Letter);
                
                var gameData = ApiUtilities.SerializeGameForApi(game);
                
                return Ok(new
                {
                    game = gameData,
                    sosFormed = newSequences.Count > 0,
                    newSequencesCount = newSequences.Count,
                    message = newSequences.Count > 0 
                        ? $"Move successful! {newSequences.Count} SOS sequence(s) formed!"
                        : "Move successful!"
                });
            }
            catch (Exception e)
            {
                return StatusCode(500, $"An error occurred while making the move: {e.Message}");
            }
        }

        // Get game state by ID
        [HttpGet("{gameId}")]
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

        // Delete a game
        [HttpDelete("{gameId}")]
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

        // Get all games (for debugging/admin purposes)
        [HttpGet("all")]
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

        // Clean up old games
        [HttpPost("cleanup")]
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
    }
}