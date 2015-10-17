using System;
using GameEngine.CSharp.Game.Board;
using GameEngine.CSharp.Game.Engine;

namespace GameEngine.CSharp.Game.AI.Strategies
{
    public class GameOverStrategy: DefaultStrategy
    {
        public GameOverStrategy(Game.Engine.Game gameData, Guid playerId)
            : base(gameData, playerId)
        {
        }

        protected override MoveValuation EvaluateMove(int choice)
        {
            Game.Engine.Game afterMove = this.gameData.Clone();
            afterMove.ChooseTurn(choice, this.playerId);

            if (afterMove.GameOver && IWin(afterMove))
            {
                return new MoveValuation() { Choice = choice, Weight = 100 };
            }

            return new MoveValuation() { Choice = choice, Weight = 0 };
        }

        private bool IWin(Game.Engine.Game game)
        {
            GameStats stats= this.gameData.GetGameStats();
            switch (this.gameData.GetCurrentTurn()) // my color
            {
                case TileType.yellow:
                    return (stats.YellowCount > stats.BlueCount && stats.YellowCount > stats.RedCount);
                case TileType.blue:
                    return (stats.BlueCount > stats.YellowCount && stats.BlueCount > stats.RedCount);
                case TileType.red:
                    return (stats.RedCount > stats.YellowCount && stats.RedCount > stats.BlueCount);
            }
            return false;
        }
    }
}
