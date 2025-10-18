namespace SOS_API.Models.GameStates
{
    public class GeneralGameState : BaseGameState
    {
        public override GameMode Mode => GameMode.General;

        public override bool IsGameOver()
        {
            // In General mode, game ends when board is full
            return IsBoardFull();
        }

        public override string DetermineWinner()
        {
            // In General mode, whoever has more SOS sequences wins
            int player1Score = Scores["Player1"];
            int player2Score = Scores["Player2"];

            if (player1Score > player2Score)
                return "Player1";
            else if (player2Score > player1Score)
                return "Player2";
            else
                return "Draw";
        }

        public override bool PlayerGetsAnotherTurn(int newSequencesCount)
        {
            // In General mode, player gets another turn if they formed SOS
            return newSequencesCount > 0;
        }
    }
}