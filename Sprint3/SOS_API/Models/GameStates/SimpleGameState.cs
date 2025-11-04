using System.Linq;
using System.Collections.Generic;
using SOS_API.Models.Players;

namespace SOS_API.Models.GameStates
{
    public class SimpleGameState : BaseGameState
    {
        public SimpleGameState(List<IPlayer> players) : base(players)
        {
        }

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
            if (firstSequence != null && firstSequence.FoundBy != null)
            {
                return firstSequence.FoundBy.Name;
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