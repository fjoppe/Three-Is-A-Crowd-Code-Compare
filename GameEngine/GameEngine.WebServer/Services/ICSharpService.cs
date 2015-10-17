using System;
using System.Collections.Generic;
using System.ServiceModel;
using GameEngine.CSharp.Game.Board;
using GameEngine.CSharp.Game.Engine;


namespace GameEngine.WebServer.Services
{
    [ServiceContract]
    public interface ICSharpService
    {
        /// <summary>
        /// This creates a new identifier which may be used by a client
        /// </summary>
        /// <returns>A new Guid</returns>
        [OperationContract]
        Guid GetUniqueIdentifier();


        /// <summary>
        /// This starts a new game from scratch.
        /// </summary>
        [OperationContract]
        void StartNewGame(GameConfiguration configuration);


        /// <summary>
        /// Join a game.
        /// </summary>
        /// <param name="playerId">Identifies the player</param>
        /// <returns>a Guid identifying a game</returns>
        [OperationContract]
        void JoinGame(Guid gameId, Guid playerId);


        /// <summary>
        /// Retrieve which color is it's turn
        /// </summary>
        /// <param name="gameId">Identifies the game</param>
        /// <returns>the color of the player who's turn it is</returns>
        [OperationContract]
        TileType GetCurrentTurn(Guid gameId);


        /// <summary>
        /// Retrieve possible moves for the specified game and player.
        /// </summary>
        /// <param name="gameId">Identifies the game</param>
        /// <param name="playerId">Identifies the player</param>
        /// <returns>List of tile-id's for which the player may possibly move</returns>
        [OperationContract]
        List<int> GetPossibleMoves(Guid gameId, Guid playerId);


        /// <summary>
        /// Send choice for turn
        /// </summary>
        /// <param name="gameId">Identifies the game</param>
        /// <param name="playerId">Identifies the player</param>
        /// <param name="id">Identifies the chosen tile</param>
        [OperationContract]
        void ChooseTurn(Guid gameId, Guid playerId, int id);


        /// <summary>
        /// Send choice for fortressed turn
        /// </summary>
        /// <param name="gameId">Identifies the game</param>
        /// <param name="playerId">Identifies the player</param>
        /// <param name="id">Identifies the chosen tile with fotress</param>
        [OperationContract]
        void ChooseFortressedTurn(Guid gameId, Guid playerId, int id);


        /// <summary>
        /// Retrieve board data for specified game. Only used once at the beginning of the game, 
        /// for the client to visualize and create references.
        /// </summary>
        /// <param name="gameId">Identifies the game</param>
        /// <returns>Board data, including board shape, colors, tile positions and tile identifiers</returns>
        [OperationContract]
        BoardSerializable RetrieveBoardData(Guid gameId);


        /// <summary>
        /// Retrieve the current board state. This function is used during the game.
        /// </summary>
        /// <param name="gameId">Identifies the game</param>
        /// <returns>Returns board data, limited to which tile has which color.</returns>
        [OperationContract]
        TileColor[] GetBoardState(Guid gameId);


        /// <summary>
        /// Retrieve current game statistics.
        /// </summary>
        /// <param name="gameId">Identifies the game</param>
        /// <returns>Game statistics, amount of tiles per color</returns>
        [OperationContract]
        GameStats GetGameStats(Guid gameId);


        /// <summary>
        /// Retrieve the color the player is playing with.
        /// </summary>
        /// <param name="gameId">Identifies the game</param>
        /// <param name="playerId">Identifies the player</param>
        /// <returns>The color the specified player is playing with, for the specified game</returns>
        [OperationContract]
        TileType WhatIsMyColor(Guid gameId, Guid playerId);


        /// <summary>
        /// Retrieve the status for the specified player, which is actually a message:
        /// * none      - the player should wait
        /// * itsMyTurn - it's the player's turn
        /// * noMoves   - no moves possible, turn is skipped
        /// * gameOver  - the game is over (at least for this player)
        /// </summary>
        /// <param name="gameId">Identifies the game</param>
        /// <param name="playerId">Identifies the player</param>
        /// <returns></returns>
        [OperationContract]
        PlayerStatus WhatIsMyStatus(Guid gameId, Guid playerId);
    }
}
