using System.Collections.Generic;
using System.Windows.Controls;
using TIAC.Visuals.Board;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using LE.GameEngine.board;

namespace LE.Application
{
    /// <summary>
    /// Interaction logic for GamePanel.xaml
    /// </summary>
    public partial class GamePanel : UserControl
    {
        private const int xMarge = 43;
        private const int yMarge = 26;
        private const int yFull = 52;
        private const int xFull = 60;

        private List<HexagonTile> hexagonList = new List<HexagonTile>();

        TileType[] turnOrder = new TileType[3];
        int currentTurn = 0;

        public GamePanel()
        {
            InitializeComponent();

            CurrentTurn.SetTileType(TileType.board);
            CurrentTurn.ShowLinks(false);

            this.firstCapture = TileType.none;
            this.turnOrder[0] = TileType.yellow;
            this.currentTurn = 0;

        }


        #region Load data
        public void LoadSaveData(BoardSerializable boardData)
        {
            Dictionary<HexagonTileSerializable, HexagonTile> mapping = new Dictionary<HexagonTileSerializable, HexagonTile>();

            this.hexagonList.Clear();
            this.Board.Children.Clear();

            foreach (HexagonTileSerializable serializedTile in boardData.ActiveTileList)
            {
                HexagonTile tile = new HexagonTile()
                {
                    TileType = serializedTile.TileType,
                };

                if (tile.TileType != TileType.none)
                {
                    mapping.Add(serializedTile, tile);
                    this.hexagonList.Add(tile);

                    BoardHexagon board = (BoardHexagon)this.GetBoardTile(serializedTile.X, serializedTile.Y);
                    tile.Visual = board;
                    board.SetTileType(tile.TileType);
                    board.ShowLinks(false);
                }
            }

            foreach (HexagonTile tile in hexagonList)
            {
                MapUpTile(tile);
            }

            foreach (HexagonTile tile in hexagonList)
            {
                tile.Visual.SetLinkCount();
            }

            this.SetTurn();
        }

        private ITileControl GetBoardTile(double x, double y)
        {
            BoardHexagon board = new BoardHexagon();
            Board.Children.Add(board);

            board.SetTileType(TileType.board);

            Canvas.SetLeft(board, x);
            Canvas.SetTop(board, y);

            return board;
        }


        private ITileControl GetBoardTile(double x, double y, TileType tileType)
        {
            BoardHexagon element = new BoardHexagon();

            element.SetTileType(tileType);

            Board.Children.Add(element);

            Canvas.SetLeft(element, x);
            Canvas.SetTop(element, y);

            return (ITileControl)element;
        }


        private void MapUpTile(HexagonTile tile)
        {
            if (tile.North == null)
            {
                tile.North = FindTileAt((int)tile.Visual.X + 0, (int)tile.Visual.Y - yFull);
                if (tile.North != null)
                {
                    tile.North.South = tile;
                }
            }

            if (tile.South == null)
            {
                tile.South = FindTileAt((int)tile.Visual.X + 0, (int)tile.Visual.Y + yFull);
                if (tile.South != null)
                {
                    tile.South.North = tile;
                }
            }

            if (tile.NorthWest == null)
            {
                tile.NorthWest = FindTileAt((int)tile.Visual.X - xMarge, (int)tile.Visual.Y - yMarge);
                if (tile.NorthWest != null)
                {
                    tile.NorthWest.SouthEast = tile;
                }
            }

            if (tile.NorthEast == null)
            {
                tile.NorthEast = FindTileAt((int)tile.Visual.X + xMarge, (int)tile.Visual.Y - yMarge);
                if (tile.NorthEast != null)
                {
                    tile.NorthEast.SouthWest = tile;
                }
            }

            if (tile.SouthWest == null)
            {
                tile.SouthWest = FindTileAt((int)tile.Visual.X - xMarge, (int)tile.Visual.Y + yMarge);
                if (tile.SouthWest != null)
                {
                    tile.SouthWest.NorthEast = tile;
                }
            }

            if (tile.SouthEast == null)
            {
                tile.SouthEast = FindTileAt((int)tile.Visual.X + xMarge, (int)tile.Visual.Y + yMarge);
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
                if ((int)tile.Visual.X == x && (int)tile.Visual.Y == y)
                {
                    return tile;
                }
            }
            return null;
        }
        #endregion


        private void SetTurn()
        {
            SetVisualTileCounts();

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

            foreach (HexagonTile h in posibilities)
            {
                ((BoardHexagon)h.Visual).SetTileType(TileType.none);

                ((BoardHexagon)h.Visual).MouseLeftButtonDown += ChooseTurn;
            }

            CurrentTurn.SetTileType(this.turnOrder[this.currentTurn]);
        }


        int skipTurns = 0;
        private void SkipTurn()
        {
            skipTurns++;
            if (skipTurns > 3)
            {
                GameOver.Visibility = Visibility.Visible;
            }
            else
            {
                NoMoves.Visibility = Visibility.Visible;
                DispatcherTimer d = new DispatcherTimer();
                d.Interval = TimeSpan.FromSeconds(0.5);
                d.Tick += (s, e) =>
                    {
                        NoMoves.Visibility = Visibility.Collapsed;
                        d.Stop();
                        InitNextTurn();
                    };
                d.Start();
            }
        }


        void ChooseTurn(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            BoardHexagon control = (BoardHexagon)sender;

            control.CurrentTile.TileType = this.turnOrder[this.currentTurn];
            control.SetTileType(this.turnOrder[this.currentTurn]);

            TurnTilesToCurrentColor(control.CurrentTile);
            control.MouseLeftButtonDown -= ChooseTurn;

            InitNextTurn();
            e.Handled = true;
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


        private void SetVisualTileCounts()
        {
            int yellowCount = hexagonList.Count(p => p.TileType == TileType.yellow);
            int blueCount = hexagonList.Count(p => p.TileType == TileType.blue);
            int redCount = hexagonList.Count(p => p.TileType == TileType.red);

            this.YellowCount.Text = yellowCount.ToString();
            this.BlueCount.Text = blueCount.ToString();
            this.RedCount.Text = redCount.ToString();
        }


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
            if (next == null || next.TileType == search.TileType || next.TileType==TileType.board || next.TileType == TileType.none)
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

            var boardtiles =from p in hexagonList
                            where p.TileType == TileType.board
                            select p;

            foreach (HexagonTile p in boardtiles)
            {
                ((BoardHexagon)p.Visual).SetTileType(TileType.board);

                ((BoardHexagon)p.Visual).MouseLeftButtonDown -= ChooseTurn;
            }
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

            while (next!=null && next.TileType != TileType.board  && next.TileType != TileType.none && next.TileType !=search.TileType)
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
                ((BoardHexagon)next.Visual).SetTileType(search.TileType);
                next = GetNext(next);
            }
        }

        #endregion

    }
}
