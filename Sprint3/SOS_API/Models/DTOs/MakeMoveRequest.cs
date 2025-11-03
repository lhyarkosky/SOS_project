using System.ComponentModel.DataAnnotations;

namespace SOS_API.Models.DTOs
{
    public class MakeMoveRequest
    {
        [Required]
        public required string GameId { get; set; }

        [Required]
        [Range(0, 19, ErrorMessage = "Row must be between 0 and 19")]
        public int Row { get; set; }

        [Required]
        [Range(0, 19, ErrorMessage = "Column must be between 0 and 19")]
        public int Col { get; set; }

        [Required]
        [RegularExpression("^[SO]$", ErrorMessage = "Letter must be 'S' or 'O'")]
        public char Letter { get; set; }
    }
}