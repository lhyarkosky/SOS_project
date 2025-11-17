using System.Collections.Generic;
using SOS_API.Models.Players;

namespace SOS_API.Models.GameStates
{
    public class GeneralGameState : BaseGameState
    {
        public GeneralGameState(List<IPlayer> players) : base(players)
        {
        }

        public override GameMode Mode => GameMode.General;

        public override bool IsGameOver()
        {
            // In General mode, game ends when board is full
            return IsBoardFull();
        }

        public override string DetermineWinner()
        {
            // In General mode, whoever has more SOS sequences wins
            if (Scores.Count == 0)
                return "Draw";

            var top = Scores.OrderByDescending(kvp => kvp.Value).ToList();
            if (top.Count > 1 && top[0].Value == top[1].Value)
                return "Draw";
            return top[0].Key.Name;
        }

        public override bool PlayerGetsAnotherTurn(int newSequencesCount)
        {
            // In General mode, player gets another turn if they formed SOS
            return newSequencesCount > 0;
        }
    }
}