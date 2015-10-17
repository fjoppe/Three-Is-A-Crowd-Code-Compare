using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.CSharp.Game.AI.FortressStragegies
{
    public class DefaultFortressStrategy : IFortressStrategy
    {
        protected Random random = new Random();

        protected Game.Engine.Game gameData;
        protected Guid playerId;

        public DefaultFortressStrategy(Game.Engine.Game gameData, Guid playerId)
        {
            this.gameData = gameData;
            this.playerId = playerId;
        }

        protected virtual void Initialize()
        {
        }

        public MoveValuation GetChoice()
        {
            this.Initialize();
            return DetermineMove();
        }

        protected virtual MoveValuation DetermineMove()
        {
            List<int> possibleMoves = gameData.GetPossibleMoves(this.playerId);

            int choice = random.Next(possibleMoves.Count);

            return new MoveValuation() { Choice = possibleMoves[choice], Weight = 1 };
        }


        public MoveValuation EvalutateChoice(int choice)
        {
            this.Initialize();
            return EvaluateMove(choice);
        }

        protected virtual MoveValuation EvaluateMove(int choice)
        {
            return new MoveValuation() { Choice = choice, Weight = 0 };
        }
    }
}
