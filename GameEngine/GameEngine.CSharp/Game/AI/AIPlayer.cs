using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using GameEngine.CSharp.Game.AI.Strategies;
using GameEngine.CSharp.Game.Board;

namespace GameEngine.CSharp.Game.AI
{
    public class AIPlayer
    {
        protected Random random = new Random();

        private object semaphor = new object();

        private Guid playerId = Guid.NewGuid();
        private Guid gameId = Guid.Empty;

        private TileType myColor = TileType.none;
        Thread gameThread;

        GameEngine.CSharp.Game.Engine.Game gameData;

        internal IStrategy[] strategies;


        public AIPlayer(GameEngine.CSharp.Game.Engine.Game gameData, Guid gameId, Guid playerId)
        {
            this.gameData = gameData;
            this.gameId = gameId;
            this.playerId = playerId;
            this.myColor = this.gameData.playerColorMapping[this.playerId];

            InitializeAIStrategies();
        }


        private void InitializeAIStrategies()
        {
            List<IStrategy> strategyList = new List<IStrategy>();

            var strategies = from t in this.GetType().Assembly.GetTypes()
                             where t.Namespace == typeof(IStrategy).Namespace
                             && t.GetInterfaces().Contains(typeof(IStrategy))
                             select t;

            foreach (Type t in strategies)
            {
                ConstructorInfo ci = t.GetConstructor(new Type[] { typeof(Game.Engine.Game), typeof(Guid) });
                object s = ci.Invoke(new object[] { this.gameData, this.playerId });

                strategyList.Add(s as IStrategy);
            }

            this.strategies = strategyList.ToArray();
        }


        public void StartAI()
        {
            MoveValuation choice = EvaluateTurns();
            this.gameData.ChooseTurn(choice.Choice, playerId);
        }


        private void InitializeTurn()
        {
            MoveValuation move = null;
            foreach (IStrategy strategy in this.strategies)
            {
                MoveValuation currentMove = strategy.GetChoice();

                if (move == null || move.Weight < currentMove.Weight)
                {
                    move = currentMove;
                }
            }

            this.gameData.ChooseTurn(move.Choice, playerId);
        }


        private MoveValuation EvaluateTurns()
        {
            Dictionary<int, MoveValuation> moves = PrepareEvaluation();

            foreach (int turn in moves.Keys.ToArray())
            {
                foreach (IStrategy strategy in this.strategies)
                {
                    moves[turn] += strategy.EvalutateChoice(moves[turn].Choice);
                }
            }

            int maxValue = moves.Values.Max(h => h.Weight);

            var isMax = from m in moves.Values
                        where m.Weight == maxValue
                        select m;

            MoveValuation[] availableChoices = isMax.ToArray();
            int choice = random.Next(availableChoices.Length);
            return availableChoices[choice];
        }


        private Dictionary<int, MoveValuation> PrepareEvaluation()
        {
            List<int> possibleMoves = gameData.GetPossibleMoves(this.playerId);
            Dictionary<int, MoveValuation> evaluation = new Dictionary<int, MoveValuation>();
            foreach (int turn in possibleMoves)
            {
                evaluation.Add(turn, new MoveValuation() { Choice = turn, Weight = 0 });
            }
            return evaluation;
        }
    }
}
