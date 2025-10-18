using System.Text.Json;
using System.Text.Json.Serialization;
using PalindromeCheckerApi.Models.GameStates;

namespace PalindromeCheckerApi.Models.DTOs
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
                currentPlayer = game.CurrentPlayer,
                status = game.Status.ToString(),
                mode = game.Mode.ToString(),
                completedSequences = game.CompletedSequences.Select(seq => new
                {
                    positions = seq.Positions.Select(p => new { row = p.row, col = p.col }).ToArray(),
                    direction = seq.Direction,
                    foundBy = seq.FoundBy
                }).ToArray(),
                scores = game.Scores,
                winner = game.Winner,
                createdAt = game.CreatedAt,
                lastMoveAt = game.LastMoveAt
            };

            // Serialize to JSON string and then deserialize back to object
            var jsonString = JsonSerializer.Serialize(apiObject, JsonOptions);
            return JsonSerializer.Deserialize<object>(jsonString, JsonOptions) ?? new object();
        }
    }
}