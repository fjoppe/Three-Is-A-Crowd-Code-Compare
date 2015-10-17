using System;
using System.Collections.Generic;
using System.Linq;
using LE.GameEngine.board;
using LE.GameEngine.TIC_Webservice;

namespace LE.GameEngine.GameEngine
{
    public class Game
    {
        private const int xMarge = 43;
        private const int yMarge = 26;
        private const int yFull = 52;
        private const int xFull = 60;

        private List<HexagonTile> hexagonList = new List<HexagonTile>();

        Dictionary<TileType, Queue<PlayerStatus>> playerMessages = new Dictionary<TileType, Queue<PlayerStatus>>();
        List<Guid> playerGuids = new List<Guid>();
        TileType[] turnOrder = new TileType[3];
        int currentTurn = 0;

        //  maps which color which player has
        Dictionary<Guid, TileType> playerColorMapping = new Dictionary<Guid, TileType>();

        private static Game game = null;

        public static void StartNewGame()
        {
            Game.game = null;
        }


        public static Game JoinGame(Guid playerId)
        {
            if (Game.game == null)
            {
                Game.game = new Game();
            }

            Game.game.RegisterPlayer(playerId);

            return Game.game;
        }


        private Game()
        {
            this.firstCapture = TileType.none;
            this.turnOrder[0] = TileType.yellow;
            this.currentTurn = 0;
        }


        private void RegisterPlayer(Guid playerId)
        {
            lock (this.playerColorMapping)
            {
                this.playerGuids.Add(playerId);

                if (this.playerGuids.Count == 3)
                {
                    this.playerColorMapping.Add(this.playerGuids[0], TileType.yellow);
                    this.playerColorMapping.Add(this.playerGuids[1], TileType.blue);
                    this.playerColorMapping.Add(this.playerGuids[2], TileType.red);

                    this.playerMessages.Add(TileType.yellow, new Queue<PlayerStatus>());
                    this.playerMessages.Add(TileType.blue, new Queue<PlayerStatus>());
                    this.playerMessages.Add(TileType.red, new Queue<PlayerStatus>());
                }
            }
        }


        #region Load data
        public void LoadSaveData(BoardSerializable boardData)
        {
            int id = 0;

            Dictionary<HexagonTileSerializable, HexagonTile> mapping = new Dictionary<HexagonTileSerializable, HexagonTile>();

            this.hexagonList.Clear();

            foreach (HexagonTileSerializable serializedTile in boardData.ActiveTileList)
            {
                HexagonTile tile = new HexagonTile()
                {
                    Id = serializedTile.Id,
                    TileType = serializedTile.TileType,
                    X = serializedTile.X,
                    Y = serializedTile.Y,
                };

                if (tile.TileType != TileType.none)
                {
                    mapping.Add(serializedTile, tile);
                    this.hexagonList.Add(tile);
                }
            }

            foreach (HexagonTile tile in hexagonList)
            {
                MapUpTile(tile);
            }

            this.SetTurn();
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
            return this.turnOrder[this.currentTurn];
        }


        public List<int> GetPossibleMoves()
        {
            List<HexagonTile> posibilities = new List<HexagonTile>();

            var colors = from h in hexagonList
                         where h.TileType == this.turnOrder[this.currentTurn]
                         select h;

            foreach (HexagonTile tile in colors)
            {
                FindPosibilities(tile, posibilities);
            }

            var positions = from i in posibilities
                            select i.Id;

            return new List<int>(positions);
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
                FindPosibilities(tile, posibilities);
            }

            if (posibilities.Count == 0)
            {
                SkipTurn();
                return;
            }

            skipTurns = 0;

            this.playerMessages[this.turnOrder[this.currentTurn]].Enqueue(PlayerStatus.itsMyTurn);
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


        public void ChooseTurn(int id, Guid playerId)
        {
            if (playerColorMapping[playerId] == turnOrder[this.currentTurn])
            {
                HexagonTile choice = hexagonList.FirstOrDefault(p => p.Id == id);


                choice.TileType = this.turnOrder[this.currentTurn];

                TurnTilesToCurrentColor(choice);

                InitNextTurn();
            }
            else
            {
                throw new Exception("It is not your turn!");
            }
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
        private List<HexagonTile> FindPosibilities(HexagonTile search, List<HexagonTile> posibilities)
        {
            AddPosibility(posibilities, FindPotentialLine(search, f => f.North));
            AddPosibility(posibilities, FindPotentialLine(search, f => f.NorthWest));
            AddPosibility(posibilities, FindPotentialLine(search, f => f.NorthEast));
            AddPosibility(posibilities, FindPotentialLine(search, f => f.South));
            AddPosibility(posibilities, FindPotentialLine(search, f => f.SouthEast));
            AddPosibility(posibilities, FindPotentialLine(search, f => f.SouthWest));

            return posibilities;
        }

        private void AddPosibility(List<HexagonTile> posibilities, HexagonTile tile)
        {
            if (tile != null && !posibilities.Contains(tile))
            {
                posibilities.Add(tile);
            }
        }

        private HexagonTile FindPotentialLine(HexagonTile search, Func<HexagonTile, HexagonTile> GetNext)
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
                switch (this.firstCapture)
                {
                    case TileType.red:
                        turnOrder[1] = TileType.red;
                        turnOrder[2] = TileType.blue;
                        break;
                    case TileType.blue:
                        turnOrder[1] = TileType.blue;
                        turnOrder[2] = TileType.red;
                        break;
                }
            }

            var boardtiles = from p in hexagonList
                             where p.TileType == TileType.board
                             select p;
        }


        private TileType firstCapture = TileType.none;
        private HexagonTile FindColorInLine(HexagonTile search, Func<HexagonTile, HexagonTile> GetNext)
        {
            TileType found = TileType.none;

            HexagonTile next = GetNext(search);

            if (next == null || next.TileType == search.TileType || next.TileType == TileType.board || next.TileType == TileType.none)
            {
                return null;
            }

            while (next != null && next.TileType != TileType.board && next.TileType != TileType.none && next.TileType != search.TileType)
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



        private void GameOverEvent()
        {
            lock (this.playerMessages)
            {
                this.playerMessages[this.turnOrder[this.currentTurn]].Enqueue(PlayerStatus.gameOver);
            }
        }


        private void SkipTurnEvent()
        {
            lock (this.playerMessages)
            {
                this.playerMessages[this.turnOrder[this.currentTurn]].Enqueue(PlayerStatus.noMoves);
            }
        }


        #region information services
        public HexagonTileSerializable[] RetrieveBoardData()
        {
            List<HexagonTileSerializable> result = new List<HexagonTileSerializable>();

            foreach (HexagonTile t in hexagonList)
            {
                result.Add(new HexagonTileSerializable(t));
            }

            return result.ToArray();
        }


        public TileColor[] GetBoardState()
        {
            List<TileColor> tileColors = new List<TileColor>();
            foreach (HexagonTile t in hexagonList)
            {
                tileColors.Add(new TileColor() { id = t.Id, color = t.TileType });
            }
            return tileColors.ToArray();
        }


        public GameStats GetGameStats()
        {
            GameStats stats = new GameStats();

            stats.YellowCount = hexagonList.Count(p => p.TileType == TileType.yellow);
            stats.BlueCount = hexagonList.Count(p => p.TileType == TileType.blue);
            stats.RedCount = hexagonList.Count(p => p.TileType == TileType.red);

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
                else
                {
                    return playerColorMapping[playerId];
                }
            }
        }


        public PlayerStatus WhatIsMyStatus(Guid playerId)
        {
            lock (this.playerMessages)
            {
                if (this.playerMessages[this.playerColorMapping[playerId]].Count > 0)
                {
                    return this.playerMessages[this.playerColorMapping[playerId]].Dequeue();
                }
                else
                {
                    return PlayerStatus.none;
                }
            }
        }

        #endregion
    }
}
