using System;

namespace GameEngine.CSharp.Game.AI.FortressStragegies
{
    public class BorderStrategy : DefaultFortressStrategy
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
                Weight = (this.gameData.NeighbourCount(choice)) // the more neighbour tiles, the better for a fortress
            };
        }
    }
}
