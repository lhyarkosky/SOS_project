using System.Text.Json;
using System.Text.Json.Serialization;
using SOS_API.Models.GameStates;

namespace SOS_API.Models.DTOs
{

    // Utility methods for API serialization

    public static class ApiUtilities
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public static object SerializeGameForApi(IGameState game)
        {
            // Create a simple object that avoids tuple serialization issues
            var apiObject = new
            {
                gameId = game.GameId,
                board = new
                {
                    size = game.Board.Size,
                    cells = game.Board.Board.ToDictionary(
                        kvp => $"{kvp.Key.row},{kvp.Key.col}", 
                        kvp => kvp.Value
                    )
                },
                currentPlayer = game.CurrentPlayer?.Name,
                status = game.Status.ToString(),
                mode = game.Mode.ToString(),
                players = game.Players.Select(p => p.Name).ToArray(),
                completedSequences = game.CompletedSequences.Select(seq => new
                {
                    positions = seq.Positions.Select(p => new { row = p.row, col = p.col }).ToArray(),
                    direction = seq.Direction,
                    foundBy = seq.FoundBy?.Name
                }).ToArray(),
                scores = game.Scores.ToDictionary(kvp => kvp.Key.Name, kvp => kvp.Value),
                winner = game.Winner?.Name,
                createdAt = game.CreatedAt,
                lastMoveAt = game.LastMoveAt
            };

            // Serialize to JSON string and then deserialize back to object
            var jsonString = JsonSerializer.Serialize(apiObject, JsonOptions);
            return JsonSerializer.Deserialize<object>(jsonString, JsonOptions) ?? new object();
        }
    }
}