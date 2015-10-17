using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using NLog;
using GameEngine.CSharp.Classes;
using GameEngine.CSharp.Game.Board;

namespace GameEngine.CSharp.Game.Engine
{
    [DataContract]
    public class Game
    {
        private static Logger logger = LogManager.GetLogger("debug"); 

        private const int xMarge = 43;
        private const int yMarge = 26;
        private const int yFull = 52;
        private const int xFull = 60;

        [DataMember]
        public GameConfiguration Configuration { get; set; }

        [DataMember]
        public List<HexagonTile> hexagonList = new List<HexagonTile>();

        [DataMember]
        public Dictionary<TileType, Queue<PlayerStatus>> PlayerMessages = new Dictionary<TileType, Queue<PlayerStatus>>();

        [DataMember]
        public List<Guid> playerGuids = new List<Guid>();

        [DataMember]
        public List<TileType> turnOrder = new List<TileType>();

        [DataMember]
        public int currentTurn = 0;

        //  maps which color which player has
        [DataMember]
        public TwoWayMapper<Guid, TileType> playerColorMapping = new TwoWayMapper<Guid, TileType>();

        [DataMember]
        public Dictionary<TileType, PlayerProperties> playerProperties = new Dictionary<TileType, PlayerProperties>();

        [DataMember]
        public TileType firstCapture = TileType.none;

        [DataMember]
        public int Fortresses;

        [DataMember]
        public int[] PlayerFortress = new int[3];

        [DataMember]
        public bool GameOver = false;

        [DataMember]
        public Queue<Guid> AITurn = new Queue<Guid>();


        public Game(GameConfiguration configuration)
        {
            this.firstCapture = TileType.none;
            this.turnOrder.Add(TileType.yellow);
            this.currentTurn = 0;
            this.Configuration = configuration;
        }


        #region Start of the game
        public static void StartNewGame()
        {
        }


        public void JoinGame(Guid playerId)
        {
            if (this.playerGuids.Count != 3)
            {
                this.RegisterPlayer(playerId);
            }
            else
            {
                logger.Error("Cannot join game {0}, because it's already running", playerId);
                throw new Exception("Unable to join");
            }
        }


        private void FillWithAIPlayers()
        {
            logger.Debug(string.Format("FillWithAIPlayers: {0}", this.Configuration.numberOfAI));

            for (int i = 0; i < this.Configuration.numberOfAI; i++)
            {
                Guid guid = Guid.NewGuid();
                RegisterPlayer(guid);
            }
        }


        private void RegisterPlayer(Guid playerId)
        {
            lock (this.playerColorMapping)
            {
                if (this.playerGuids.Contains(playerId))
                {
                    throw new Exception(String.Format("Player has already joined the game: {0}", playerId));
                }

                this.playerGuids.Add(playerId);

                if (this.playerGuids.Count == 3 - this.Configuration.numberOfAI)
                {
                    FillWithAIPlayers();

                    logger.Debug("Game starts");
                    this.playerColorMapping.Add(this.playerGuids[0], TileType.yellow);
                    this.playerColorMapping.Add(this.playerGuids[1], TileType.blue);
                    this.playerColorMapping.Add(this.playerGuids[2], TileType.red);

                    this.PlayerMessages.Add(TileType.yellow, new Queue<PlayerStatus>());
                    this.PlayerMessages.Add(TileType.blue, new Queue<PlayerStatus>());
                    this.PlayerMessages.Add(TileType.red, new Queue<PlayerStatus>());

                    this.playerProperties.Add(TileType.yellow, new PlayerProperties() { PlayerType = PlayerType.human });
                    this.playerProperties.Add(TileType.blue, new PlayerProperties() { PlayerType = PlayerType.computer });
                    this.playerProperties.Add(TileType.red, new PlayerProperties() { PlayerType = PlayerType.computer});

                    this.SetTurn();
                    logger.Debug("this.currentTurn: {0}, this.turnOrder[this.currentTurn]", this.currentTurn, this.turnOrder[this.currentTurn]);
                }
                else
                {
                    logger.Debug("Waiting for new players, joined: {0}", this.playerGuids.Count);
                }
            }
        }
        #endregion


        #region Load data
        public void LoadSaveData(BoardSerializable boardData)
        {
            int id = 0;

            this.Fortresses = boardData.FortressesPerPlayer;

            this.PlayerFortress[0] = boardData.FortressesPerPlayer;
            this.PlayerFortress[1] = boardData.FortressesPerPlayer;
            this.PlayerFortress[2] = boardData.FortressesPerPlayer;

            //Dictionary<HexagonTileSerializable, HexagonTile> mapping = new Dictionary<HexagonTileSerializable, HexagonTile>();

            this.hexagonList.Clear();

            foreach (HexagonTileSerializable serializedTile in boardData.ActiveTileList)
            {
                HexagonTile tile = new HexagonTile()
                {
                    Id = serializedTile.Id,
                    TileType = serializedTile.TileType,
                    X = serializedTile.X,
                    Y = serializedTile.Y,
                    TileValue = serializedTile.TileValue,
                    Fortress = serializedTile.Fortress,
                };

                if (tile.TileType != TileType.none)
                {
                    //mapping.Add(serializedTile, tile);
                    this.hexagonList.Add(tile);
                }
            }

            foreach (HexagonTile tile in hexagonList)
            {
                MapUpTile(tile);
            }

            //this.SetTurn();
        }


        private void MapUpTile(HexagonTile tile)
        {
            if (tile.North == null)
            {
                tile.North = FindTileAt((int)tile.X + 0, (int)tile.Y - yFull);
                if (tile.North != null)
                {
                    tile.North.South = tile;
                }
            }

            if (tile.South == null)
            {
                tile.South = FindTileAt((int)tile.X + 0, (int)tile.Y + yFull);
                if (tile.South != null)
                {
                    tile.South.North = tile;
                }
            }

            if (tile.NorthWest == null)
            {
                tile.NorthWest = FindTileAt((int)tile.X - xMarge, (int)tile.Y - yMarge);
                if (tile.NorthWest != null)
                {
                    tile.NorthWest.SouthEast = tile;
                }
            }

            if (tile.NorthEast == null)
            {
                tile.NorthEast = FindTileAt((int)tile.X + xMarge, (int)tile.Y - yMarge);
                if (tile.NorthEast != null)
                {
                    tile.NorthEast.SouthWest = tile;
                }
            }

            if (tile.SouthWest == null)
            {
                tile.SouthWest = FindTileAt((int)tile.X - xMarge, (int)tile.Y + yMarge);
                if (tile.SouthWest != null)
                {
                    tile.SouthWest.NorthEast = tile;
                }
            }

            if (tile.SouthEast == null)
            {
                tile.SouthEast = FindTileAt((int)tile.X + xMarge, (int)tile.Y + yMarge);
                if (tile.SouthEast != null)
                {
                    tile.SouthEast.NorthWest = tile;
                }
            }
        }


        private HexagonTile FindTileAt(int x, int y)
        {
            foreach (HexagonTile tile in hexagonList)
            {
                if ((int)tile.X == x && (int)tile.Y == y)
                {
                    return tile;
                }
            }
            return null;
        }
        #endregion


        #region Turns
        public TileType GetCurrentTurn()
        {
            if (!this.GameOver)
            {
                return this.turnOrder[this.currentTurn];
            }
            else
            {
                return TileType.none;
            }
        }


        public List<int> GetPossibleMoves(Guid playerId)
        {
            if (!playerColorMapping.ContainsKey(playerId))
            {
                logger.Error("Don't know this player: {0}", playerId);
                throw new Exception("Unknown player");
            }

            if (playerColorMapping[playerId] == turnOrder[this.currentTurn])
            {
                List<HexagonTile> posibilities = new List<HexagonTile>();

                var colors = from h in hexagonList
                             where h.TileType == this.turnOrder[this.currentTurn]
                             select h;

                foreach (HexagonTile tile in colors)
                {
                    FindChoiceCandidatesForTile(tile, posibilities);
                }

                var positions = from i in posibilities
                                select i.Id;

                return new List<int>(positions);
            }
            else
            {
                throw new Exception("It is not your turn!");
            }
        }

        //  Start of the game
        private void SetTurn()
        {
            List<HexagonTile> posibilities = new List<HexagonTile>();

            var colors = from h in hexagonList
                         where h.TileType == this.turnOrder[this.currentTurn]
                         select h;

            foreach (HexagonTile tile in colors)
            {
                FindChoiceCandidatesForTile(tile, posibilities);
            }

            if (posibilities.Count == 0)
            {
                SkipTurn();
                return;
            }

            skipTurns = 0;

            ItsMyTurnEvent();
        }


        int skipTurns = 0;
        private void SkipTurn()
        {
            skipTurns++;
            if (skipTurns > 3)
            {
                for (this.currentTurn = 0; this.currentTurn < 3; this.currentTurn++)
                {
                    this.GameOverEvent();
                    this.GameOver = true;
                }
            }
            else
            {
                if (hexagonList.Count(f => f.TileType == this.turnOrder[this.currentTurn]) == 0)
                {
                    this.GameOverEvent();
                }
                else
                {
                    this.SkipTurnEvent();
                }

                InitNextTurn();
            }
        }


        public bool HasFortresses(Guid playerId)
        {
            if (!playerGuids.Contains(playerId))
            {
                logger.Error("Don't know this player: {0}", playerId);
                throw new Exception("Unknown player");
            }
            else
            {
                return (PlayerFortress[playerGuids.IndexOf(playerId)] > 0);
            }
        }


        public void ConsumeFortress(Guid playerId, int id)
        {
            PlayerFortress[playerGuids.IndexOf(playerId)]--;
            HexagonTile choice = hexagonList.FirstOrDefault(p => p.Id == id);
            choice.Fortress = true;
        }


        public void ChooseTurn(int id, Guid playerId)
        {
            if (!playerColorMapping.ContainsKey(playerId))
            {
                logger.Error("Don't know this player: {0}", playerId);
                throw new Exception("Unknown player");
            }

            if (playerColorMapping[playerId] != turnOrder[this.currentTurn])
            {
                logger.Error("It isn't player's {0} turn", turnOrder[this.currentTurn]);
                throw new Exception("It is not your turn!");
            }

            if (!GetPossibleMoves(playerId).Contains(id))
            {
                logger.Error("Illegal move: {0}", id);
                throw new Exception("Tried illegal move");
            }

            HexagonTile choice = hexagonList.FirstOrDefault(p => p.Id == id);
            choice.TileType = this.turnOrder[this.currentTurn];
            TurnTilesToCurrentColor(choice);
            InitNextTurn();
        }
        

        private void InitNextTurn()
        {
            this.currentTurn++;
            if (this.currentTurn > 2)
            {
                currentTurn = 0;
            }

            this.SetTurn();
        }
        #endregion


        #region Find possible positions
        private List<HexagonTile> FindChoiceCandidatesForTile(HexagonTile search, List<HexagonTile> posibilities)
        {
            AddPosibility(posibilities, FindChoiceCandidateForDirection(search, f => f.North));
            AddPosibility(posibilities, FindChoiceCandidateForDirection(search, f => f.NorthWest));
            AddPosibility(posibilities, FindChoiceCandidateForDirection(search, f => f.NorthEast));
            AddPosibility(posibilities, FindChoiceCandidateForDirection(search, f => f.South));
            AddPosibility(posibilities, FindChoiceCandidateForDirection(search, f => f.SouthEast));
            AddPosibility(posibilities, FindChoiceCandidateForDirection(search, f => f.SouthWest));

            return posibilities;
        }

        private void AddPosibility(List<HexagonTile> posibilities, HexagonTile tile)
        {
            if (tile != null && !posibilities.Contains(tile))
            {
                posibilities.Add(tile);
            }
        }


        
        private HexagonTile FindChoiceCandidateForDirection(HexagonTile search, Func<HexagonTile, HexagonTile> GetNext)
        {
            HexagonTile next = GetNext(search);

            // skip first
            if (next == null || next.TileType == search.TileType || next.TileType == TileType.board || next.TileType == TileType.none)
            {
                return null;
            }

            while (next.TileType != TileType.board)
            {
                if (next.TileType == search.TileType)
                {
                    return null;
                }

                if (next.Fortress)  // blocks possible moves
                {
                    return null;
                }

                next = GetNext(next);

                // endboard => end search
                if (next == null || next.TileType == TileType.none)
                {
                    return null;
                }
            }

            return next;
        }
        #endregion


        #region Set turn
        private void TurnTilesToCurrentColor(HexagonTile chosenTile)
        {
            bool first = (this.firstCapture == TileType.none);

            if (FindColorInLine(chosenTile, f => f.North) != null)
            {
                TurnColorInLine(chosenTile, f => f.North);
            }

            if (FindColorInLine(chosenTile, f => f.NorthEast) != null)
            {
                TurnColorInLine(chosenTile, f => f.NorthEast);
            }

            if (FindColorInLine(chosenTile, f => f.NorthWest) != null)
            {
                TurnColorInLine(chosenTile, f => f.NorthWest);
            }

            if (FindColorInLine(chosenTile, f => f.South) != null)
            {
                TurnColorInLine(chosenTile, f => f.South);
            }

            if (FindColorInLine(chosenTile, f => f.SouthWest) != null)
            {
                TurnColorInLine(chosenTile, f => f.SouthWest);
            }

            if (FindColorInLine(chosenTile, f => f.SouthEast) != null)
            {
                TurnColorInLine(chosenTile, f => f.SouthEast);
            }

            if (first)
            {
                logger.Debug("First move");
                switch (this.firstCapture)
                {
                    case TileType.red:
                        turnOrder.Add(TileType.red);
                        turnOrder.Add(TileType.blue);
                        break;
                    case TileType.blue:
                        turnOrder.Add(TileType.blue);
                        turnOrder.Add(TileType.red);
                        break;
                }
                logger.Debug("The order of turns is: {0}, {1}, {2}", turnOrder[0], turnOrder[1], turnOrder[2]);
            }

            var boardtiles = from p in hexagonList
                             where p.TileType == TileType.board
                             select p;
        }


        private HexagonTile FindColorInLine(HexagonTile search, Func<HexagonTile, HexagonTile> GetNext)
        {
            TileType found = TileType.none;

            HexagonTile next = GetNext(search);

            if (next == null || next.TileType == search.TileType || next.TileType == TileType.board || next.TileType == TileType.none||next.Fortress)
            {
                return null;
            }

            while (next != null && next.TileType != TileType.board && next.TileType != TileType.none && next.TileType != search.TileType && !next.Fortress)
            {
                found = next.TileType;
                next = GetNext(next);

                // endboard => end search
                if (next == null || next.TileType == TileType.none)
                {
                    return null;
                }
            }

            if (next.TileType == search.TileType)
            {
                if (this.firstCapture == TileType.none)
                {
                    this.firstCapture = found;
                }
                return next;
            }
            return null;
        }

        private void TurnColorInLine(HexagonTile search, Func<HexagonTile, HexagonTile> GetNext)
        {
            HexagonTile next = GetNext(search);
            while (next.TileType != search.TileType)
            {
                next.TileType = search.TileType;
                next = GetNext(next);
            }
        }

        #endregion


        #region Event Reporting
        private void GameOverEvent()
        {
            lock (this.PlayerMessages)
            {
                logger.Debug("Game Over for {0}", this.turnOrder[this.currentTurn]);
                this.PlayerMessages[this.turnOrder[this.currentTurn]].Enqueue(PlayerStatus.gameOver);
            }
        }


        private void SkipTurnEvent()
        {
            lock (this.PlayerMessages)
            {
                logger.Debug("Turn skipped for {0}", this.turnOrder[this.currentTurn]);
                this.PlayerMessages[this.turnOrder[this.currentTurn]].Enqueue(PlayerStatus.noMoves);
            }
        }


        private void ItsMyTurnEvent()
        {
            lock (this.PlayerMessages)
            {
                if (this.playerProperties[this.turnOrder[this.currentTurn]].PlayerType == PlayerType.human)
                {
                    logger.Debug("Set turn for human {0}", this.turnOrder[this.currentTurn]);
                    this.PlayerMessages[this.turnOrder[this.currentTurn]].Enqueue(PlayerStatus.itsMyTurn);
                }
                else
                {
                    logger.Debug("Set turn for computer {0}", this.turnOrder[this.currentTurn]);
                    //this.PlayerMessages[this.turnOrder[this.currentTurn]].Enqueue(PlayerStatus.triggerAI);
                    this.AITurn.Enqueue(this.playerColorMapping[this.turnOrder[this.currentTurn]]); // message the guid for AI player to be next
                }
            }
        }
        #endregion


        #region information services
        public BoardSerializable RetrieveBoardData()
        {
            BoardSerializable result = new BoardSerializable();

            result.ActiveTileList = new List<HexagonTileSerializable>();

            result.FortressesPerPlayer = this.Fortresses;

            foreach (HexagonTile t in hexagonList)
            {
                result.ActiveTileList.Add(new HexagonTileSerializable(t));
            }

            return result;
        }


        public TileColor[] GetBoardState()
        {
            List<TileColor> tileColors = new List<TileColor>();
            foreach (HexagonTile t in hexagonList)
            {
                tileColors.Add(new TileColor() { id = t.Id, color = t.TileType, Fortress = t.Fortress });
            }
            return tileColors.ToArray();
        }


        public GameStats GetGameStats()
        {
            GameStats stats = new GameStats();

            stats.YellowCount = hexagonList.Count(p => p.TileType == TileType.yellow);
            stats.BlueCount = hexagonList.Count(p => p.TileType == TileType.blue);
            stats.RedCount = hexagonList.Count(p => p.TileType == TileType.red);

            stats.RedFortress = PlayerFortress[playerGuids.IndexOf(playerColorMapping[TileType.red])];
            stats.BlueFortress = PlayerFortress[playerGuids.IndexOf(playerColorMapping[TileType.blue])];
            stats.YellowFortress = PlayerFortress[playerGuids.IndexOf(playerColorMapping[TileType.yellow])];

            return stats;
        }


        public TileType GetMyColor(Guid playerId)
        {
            lock (playerColorMapping)
            {

                if (playerColorMapping.Count == 0)
                {
                    return TileType.none;
                }

                if (!playerColorMapping.ContainsKey(playerId))
                {
                    logger.Error("Don't know this player: {0}", playerId);
                    throw new Exception("Unknown player");
                }

                return playerColorMapping[playerId];
            }
        }


        public PlayerStatus WhatIsMyStatus(Guid playerId)
        {
            lock (this.PlayerMessages)
            {
                if (!playerColorMapping.ContainsKey(playerId))
                {
                    logger.Error("Don't know this player: {0}", playerId);
                    throw new Exception("Unknown player");
                }

                if (this.PlayerMessages[this.playerColorMapping[playerId]].Count > 0)
                {
                    return this.PlayerMessages[this.playerColorMapping[playerId]].Dequeue();
                }
                else
                {
                    return PlayerStatus.none;
                }
            }
        }

        #endregion


        #region Cloning
        /// <summary>
        /// Creates a clone of this object.
        /// </summary>
        /// <returns>A new object witg cloned data</returns>
        public Game Clone()
        {
            Game target = new Game(this.Configuration);

            // first step: set basic info
            for (int i = 0; i < this.hexagonList.Count; i++)
            {
                target.hexagonList.Add(new HexagonTile()
                {
                    Id = this.hexagonList[i].Id,
                    TileType = this.hexagonList[i].TileType,
                    TileValue = this.hexagonList[i].TileValue,
                    X = this.hexagonList[i].X,
                    Y = this.hexagonList[i].Y,
                });
            }
            // second step: set relations
            for (int i = 0; i < this.hexagonList.Count; i++)
            {
                target.hexagonList[i].North = GetClone(this.hexagonList[i].North, target);
                target.hexagonList[i].NorthEast = GetClone(this.hexagonList[i].NorthEast, target);
                target.hexagonList[i].NorthWest = GetClone(this.hexagonList[i].NorthWest, target);

                target.hexagonList[i].South = GetClone(this.hexagonList[i].South, target);
                target.hexagonList[i].SouthEast = GetClone(this.hexagonList[i].SouthEast,target);
                target.hexagonList[i].SouthWest = GetClone(this.hexagonList[i].SouthWest,target);
            }

            target.Configuration = new GameConfiguration()
            {
                GameId = this.Configuration.GameId,
                numberOfAI = this.Configuration.numberOfAI,
            };


            foreach (TileType tileType in this.PlayerMessages.Keys)
            {
                target.PlayerMessages.Add(tileType, new Queue<PlayerStatus>(this.PlayerMessages[tileType].ToArray()));
            }

            target.playerGuids.AddRange(this.playerGuids);
            target.currentTurn = this.currentTurn;
            target.playerProperties = new Dictionary<TileType, PlayerProperties>(this.playerProperties);
            target.playerColorMapping = new TwoWayMapper<Guid, TileType>(this.playerColorMapping);

            target.firstCapture = this.firstCapture;

            target.turnOrder = new List<TileType>(this.turnOrder);

            return target;
        }


        private HexagonTile GetClone(HexagonTile tile, Game target)
        {
            if (this.hexagonList.IndexOf(tile) >= 0)
            {
                return target.hexagonList[this.hexagonList.IndexOf(tile)];
            }
            else
            {
                return null;
            }
        }

        #endregion


        #region AI Helpers

        public int CountPoints(Guid playerId)
        {
            TileType color = playerColorMapping[playerId];

            var myTiles = from h in this.hexagonList
                          where h.TileType == color
                          select h;
            return myTiles.Sum(h => h.TileValue);                      
        }


        public Guid WhosTurnIsItNow()
        {
            return this.playerColorMapping[this.turnOrder[this.currentTurn]];
        }


        public TileType WhoIsAfterMe(Guid playerId)
        {
            TileType color = this.playerColorMapping[playerId];
            int afterMe = (this.turnOrder.IndexOf(color) + 1) % this.turnOrder.Count;
            return this.turnOrder[afterMe];
        }


        public int NeighbourCount(int id)
        {
            HexagonTile tile = hexagonList.Find(t => t.Id == id);

            HexagonTile[] neighbours =
            new[]{
                    tile.North,
                    tile.NorthEast,
                    tile.NorthWest,
                    tile.South,
                    tile.SouthEast,
                    tile.SouthWest,
                };

            return neighbours.Count(n => n != null);
        }


        public int NeighbourColorCount(int id, TileType color)
        {
            HexagonTile tile = hexagonList.Find(t => t.Id == id);

            HexagonTile[] neighbours =
            new[]{
                    tile.North,
                    tile.NorthEast,
                    tile.NorthWest,
                    tile.South,
                    tile.SouthEast,
                    tile.SouthWest,
                };

            return neighbours.Count(n => n != null && n.TileType == color);
        }


        public int DeepNeighbourColorCount(int id, TileType color)
        {
            List<HexagonTile> resultTiles = new List<HexagonTile>();

            HexagonTile[] firstDepth = this.GetNeighbours(id);

            resultTiles.AddRange(firstDepth);

            foreach (HexagonTile tile in firstDepth)
            {
                HexagonTile[] depthNeighbours = this.GetNeighbours(tile.Id);
                if (depthNeighbours.Length > 0)
                {
                    resultTiles.AddRange(depthNeighbours);
                }
            }

            return resultTiles.Distinct().Count(n => n != null && n.TileType == color);
        }


        public HexagonTile[] GetNeighbours(int id)
        {
            HexagonTile tile = hexagonList.Find(t => t.Id == id);

            HexagonTile[] neighbours =
            new[]{
                    tile.North,
                    tile.NorthEast,
                    tile.NorthWest,
                    tile.South,
                    tile.SouthEast,
                    tile.SouthWest,
                };

            return neighbours;
        }

        #endregion
    }
}
