using System;
using GameEngine.CSharp.Game.Board;

namespace GameEngine.CSharp.Game.AI.FortressStragegies
{
    public class FieldPositionStrategy : DefaultFortressStrategy
    {
        public FieldPositionStrategy(Game.Engine.Game gameData, Guid playerId)
            : base(gameData, playerId)
        {
        }


        protected override MoveValuation EvaluateMove(int choice)
        {
            return new MoveValuation()
            {
                Choice = choice,
                Weight = (this.gameData.NeighbourColorCount(choice, TileType.board)) // the more empty neighbour tiles, the better for a fortress
            };
        }
    }
}
