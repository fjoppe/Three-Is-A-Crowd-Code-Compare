using System;
using System.Collections.Generic;
using GameEngine.CSharp.Game.Engine;

namespace GameEngine.CSharp.Game.AI.Strategies
{
    //public class DeepInpectionStrategy : DefaultStrategy
    //{
    //    private const int maxDepth = 1;

    //    public DeepInpectionStrategy(Game.Engine.Game gameData, Guid playerId)
    //        : base(gameData, playerId)
    //    {
    //    }


    //    protected override MoveValuation EvaluateMove(int choice)
    //    {
    //        Engine.Game gameCopy = this.gameData.Clone();
    //        gameCopy.ChooseTurn(choice, this.playerId);

    //        return new MoveValuation() { Choice = choice, Weight = this.PlayRounds(gameCopy, 0) };
    //    }


    //    private int PlayRounds(Engine.Game gameData, int depth)
    //    {
    //        Guid currentPlayer = gameData.WhosTurnIsItNow();

    //        if (currentPlayer == this.playerId)
    //        {
    //            depth++;
    //        }

    //        if (depth < maxDepth)
    //        {
    //            List<int> possibleMoves = gameData.GetPossibleMoves(currentPlayer);

    //            int countPoints = 0;

    //            foreach (int choice in possibleMoves)
    //            {
    //                Engine.Game gameCopy = gameData.Clone();
    //                gameCopy.ChooseTurn(choice, currentPlayer);


    //                PlayerStatus myStatus = gameCopy.WhatIsMyStatus(this.playerId);
    //                switch (myStatus)
    //                {
    //                    case PlayerStatus.gameOver: // this is only good when it's game over for everybody
    //                        if (gameCopy.GameOver)
    //                        {
    //                            countPoints += 1;
    //                        }
    //                        else
    //                        {
    //                            countPoints -= 4;
    //                        }
    //                        break;
    //                    case PlayerStatus.noMoves:
    //                        countPoints -= 2;
    //                        break;
    //                    default:    // recurse loop
    //                        countPoints += this.PlayRounds(gameCopy, depth);
    //                        break;
    //                }
    //            }

    //            return countPoints;
    //        }
    //        else
    //        {
    //           return gameData.CountPoints(this.playerId);
    //        }
    //    }
    //}
}
