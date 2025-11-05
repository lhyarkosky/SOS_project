using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SOS_API.Models.Players;

namespace SOS_API.Models.DTOs
{
    public class CreateGameRequest
    {
        [Required]
        [Range(3, 20, ErrorMessage = "Board size must be between 3 and 20")]
        public int BoardSize { get; set; }

        [Required]
        [RegularExpression("^(Simple|General)$", ErrorMessage = "Game mode must be 'Simple' or 'General'")]
        public required string GameMode { get; set; }

        [StringLength(15, ErrorMessage = "Player name cannot exceed 15 characters")]
        public string? Player1Name { get; set; }

        [StringLength(15, ErrorMessage = "Player name cannot exceed 15 characters")]
        public string? Player2Name { get; set; }

        [Required]
        public PlayerType Player1Type { get; set; }

        [Required]
        public PlayerType Player2Type { get; set; }
    }
}