using System;
using System.Linq;

namespace PalindromeCheckerApi.BusinessLogic
{
    public static class TextValidator
    {
        public static bool IsAlphabetical(string input)
        {
            return !string.IsNullOrEmpty(input) && input.All(char.IsLetter);
        }

        public static bool ContainsSpaces(string input)
        {
            return !string.IsNullOrEmpty(input) && input.Any(char.IsWhiteSpace);
        }
        
        public static bool IsPalindrome(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            var normalized = new string(input.Where(char.IsLetter).Select(char.ToLower).ToArray());
            return normalized.SequenceEqual(normalized.Reverse());
        }
    }
}