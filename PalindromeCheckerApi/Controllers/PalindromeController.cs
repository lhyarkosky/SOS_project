using Microsoft.AspNetCore.Mvc;
using PalindromeCheckerApi.Models;
using PalindromeCheckerApi.BusinessLogic;

namespace PalindromeCheckerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PalindromeController : ControllerBase
    {
        [HttpGet("validate")]
        public ActionResult<ValidationResponse> Validate([FromQuery] string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return BadRequest(new ValidationResponse
                {
                    IsValid = false,
                    Message = "Please enter some text."
                });
            }

            if (TextValidator.ContainsSpaces(text))
            {
                return Ok(new ValidationResponse
                {
                    IsValid = false,
                    Message = "Input cannot have spaces."
                });
            }

            if (!TextValidator.IsAlphabetical(text))
            {
                return Ok(new ValidationResponse
                {
                    IsValid = false,
                    Message = "Input must be alphabetic."
                });
            }


            bool isPalindrome = TextValidator.IsPalindrome(text);
            
            return Ok(new ValidationResponse
            {
                IsValid = true,
                Message = isPalindrome ? "This is a palindrome!" : "This is NOT a palindrome."
            });
        }
    }
}