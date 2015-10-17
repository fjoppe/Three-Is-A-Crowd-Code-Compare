using GameEngine.CSharp.Game.Board;
using System;

namespace GameEngine.CSharp.Game.AI.Strategies
{
    public class KillPlayerTurnStrategy : DefaultStrategy
    {
        public KillPlayerTurnStrategy(Game.Engine.Game gameData, Guid playerId)
            : base(gameData, playerId)
        {
        }

        protected override MoveValuation EvaluateMove(int choice)
        {
            GameEngine.CSharp.Game.Engine.Game afterMove = this.gameData.Clone();

            TileType afterMe = this.gameData.WhoIsAfterMe(this.playerId);
            afterMove.ChooseTurn(choice, this.playerId);

            TileType afterMoveTurn = afterMove.GetCurrentTurn();

            if (afterMoveTurn != afterMe)
            {
                if (afterMoveTurn == this.gameData.GetCurrentTurn()) // myself
                {
                    return new MoveValuation() { Choice = choice, Weight = 6 };
                }
                else
                {
                    return new MoveValuation() { Choice = choice, Weight = 3 };
                }
            }
            else
            {
                return new MoveValuation() { Choice = choice, Weight = 0 };
            }
        }
    }
}
