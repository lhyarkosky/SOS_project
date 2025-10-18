using System.ComponentModel.DataAnnotations;

namespace PalindromeCheckerApi.Models.DTOs
{
    public class CreateGameRequest
    {
        [Required]
        [Range(3, 20, ErrorMessage = "Board size must be between 3 and 20")]
        public int BoardSize { get; set; }

        [Required]
        [RegularExpression("^(Simple|General)$", ErrorMessage = "Game mode must be 'Simple' or 'General'")]
        public required string GameMode { get; set; }
    }
}