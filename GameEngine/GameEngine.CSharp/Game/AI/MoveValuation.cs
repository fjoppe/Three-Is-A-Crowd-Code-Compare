using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.CSharp.Game.AI
{
    public class MoveValuation
    {
        public int Choice { get; set; }
        public int Weight { get; set; }

        public static MoveValuation operator +(MoveValuation e1, MoveValuation e2)
        {
            if (e1.Choice != e2.Choice)
            {
                throw new Exception("Choice must be the same value when adding.");
            }
            return new MoveValuation() { Choice = e1.Choice, Weight = e1.Weight = e2.Weight };
        }
    }
}
