
using System;
namespace GameEngine.CSharp.Game.AI.Strategies
{
    public class PointCountStrategy : DefaultStrategy
    {
        public PointCountStrategy(Game.Engine.Game gameData, Guid playerId)
            : base(gameData, playerId)
        {
        }

        protected override MoveValuation EvaluateMove(int choice)
        {
            int currentPoints = this.gameData.CountPoints(this.playerId);

            Game.Engine.Game afterTurn = this.gameData.Clone();
            afterTurn.ChooseTurn(choice, this.playerId);

            int afterPoints = afterTurn.CountPoints(this.playerId);

            return new MoveValuation()
            {
                Choice = choice,
                Weight = afterPoints - currentPoints,
            };
        }
    }
}
