
namespace GameEngine.CSharp.Game.AI.Strategies
{
    interface IStrategy
    {
        MoveValuation GetChoice();
        MoveValuation EvalutateChoice(int choice);
    }
}
