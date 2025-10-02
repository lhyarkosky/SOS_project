using NUnit.Framework;
using PalindromeCheckerApi.BusinessLogic;

namespace PalindromeCheckerApi.Tests.BusinessLogic
{
    [TestFixture]
    public class TextValidatorTests
    {
        [Test]
        public void IsPalindrome_WithSimplePalindrome_ReturnsTrue()
        {
            // Arrange
            string input = "racecar";

            // Act
            bool result = TextValidator.IsPalindrome(input);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsPalindrome_WithMixedCase_ReturnsTrue()
        {
            // Arrange
            string input = "RaceCar";

            // Act
            bool result = TextValidator.IsPalindrome(input);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsPalindrome_WithNonPalindrome_ReturnsFalse()
        {
            // Arrange
            string input = "hello";

            // Act
            bool result = TextValidator.IsPalindrome(input);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsPalindrome_WithSpecialCharsAndNumbers_ReturnsTrue()
        {
            // Arrange
            string input = "A1b2c3:c2b1a"; 

            // Act
            bool result = TextValidator.IsPalindrome(input);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsPalindrome_WithEmptyString_ReturnsFalse()
        {
            // Arrange
            string input = "";

            // Act
            bool result = TextValidator.IsPalindrome(input);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsPalindrome_WithNull_ReturnsFalse()
        {
            // Arrange
            string input = null;

            // Act
            bool result = TextValidator.IsPalindrome(input);

            // Assert
            Assert.IsFalse(result);
        }
    }
}