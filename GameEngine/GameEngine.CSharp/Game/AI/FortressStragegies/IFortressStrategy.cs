using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.CSharp.Game.AI.FortressStragegies
{
    interface IFortressStrategy
    {
        MoveValuation GetChoice();
        MoveValuation EvalutateChoice(int choice);
    }
}
