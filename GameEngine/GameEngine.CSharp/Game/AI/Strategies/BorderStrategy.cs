using System;

namespace GameEngine.CSharp.Game.AI.Strategies
{
    public class BorderStrategy : DefaultStrategy
    {
        public BorderStrategy(Game.Engine.Game gameData, Guid playerId)
            : base(gameData, playerId)
        {
        }

        protected override MoveValuation EvaluateMove(int choice)
        {
            return new MoveValuation()
            {
                Choice = choice,
                Weight = 6 - (this.gameData.NeighbourCount(choice))
            };
        }
    }
}
