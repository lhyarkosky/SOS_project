using System.Linq;

namespace PalindromeCheckerApi.Models.GameStates
{
    public class SimpleGameState : BaseGameState
    {
        public override GameMode Mode => GameMode.Simple;

        public override bool IsGameOver()
        {
            // In Simple mode, game ends when first SOS is formed, or if the board is full
            return CompletedSequences.Any() || IsBoardFull();
        }

        public override string DetermineWinner()
        {
            // In Simple mode, whoever formed the first SOS wins
            var firstSequence = CompletedSequences.FirstOrDefault();
            if (firstSequence != null)
            {
                return firstSequence.FoundBy;
            }
            
            // If no SOS sequences but board is full, it's a draw
            return "Draw";
        }

        public override bool PlayerGetsAnotherTurn(int newSequencesCount)
        {
            // In Simple mode, turns always alternate
            return false;
        }
    }
}