using System;
using GameEngine.CSharp.Game.Board;

namespace GameEngine.CSharp.Game.AI.Strategies
{
    public class PreventNeighbourStrategy: DefaultStrategy
    {
        public PreventNeighbourStrategy(Game.Engine.Game gameData, Guid playerId)
            : base(gameData, playerId)
        {
        }

        protected override MoveValuation EvaluateMove(int choice)
        {
            HexagonTile[] tileList= this.gameData.GetNeighbours(choice);

            MoveValuation total = new MoveValuation() { Choice = choice, Weight = 0 };

            foreach (HexagonTile tile in tileList)
            {
                if (tile != null)
                {
                    total += new MoveValuation() { Choice = choice, Weight = 2* ( this.gameData.NeighbourCount(tile.Id) - 6) };
                }
            }

            return total;
        }
    }
}

