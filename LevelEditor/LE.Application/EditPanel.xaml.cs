using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using LE.Application.Classes;
using LE.GameEngine.board;
using LE.Visuals.Board;
using LE.GameEngine.TIC_Webservice;
using HexagonTileSerializable = LE.GameEngine.board.HexagonTileSerializable;
using BoardSerializable = LE.GameEngine.board.BoardSerializable;

namespace LE.Application
{
    /// <summary>
    /// Interaction logic for EditPanel.xaml
    /// </summary>
    public partial class EditPanel : UserControl
    {
        //  position consts
        private const int xMarge = 43;
        private const int yMarge = 26;
        private const int yFull = 52;
        private const int xFull = 60;

        //  default consts
        private const int defaultTileValue = 1;


        //  private properties
        private HexagonTile startTile;

        private TwoWayMapper<HexagonTile, BoardHexagon> hexagonList = new TwoWayMapper<HexagonTile, BoardHexagon>();


        //  Display options
        private bool displayBoneStructure = true;

        public EditPanel()
        {
            InitializeComponent();

            InitNewLevel();
        }


        public void InitNewLevel()
        {
            Board.Children.Clear();
            CrossSections.Children.Clear();

            hexagonList.Clear();

            startTile = new HexagonTile()
            {
                TileType = TileType.none,
                TileValue = 0,
            };
            hexagonList.Add(startTile, GetEditTile(0,0));
        }


        private void AddEditSpot(HexagonTile tile, double x, double y)
        {
            tile.X = (int)x;
            tile.Y = (int)y;

            hexagonList.Add(tile, GetEditTile(x,y));
        }


        private BoardHexagon GetEditTile(double x, double y)
        {
            BoardHexagon editTile = new BoardHexagon();
            editTile.MouseLeftButtonDown += AddHexagonTile;
            Board.Children.Add(editTile);

            Canvas.SetLeft(editTile, x);
            Canvas.SetTop(editTile, y);

            editTile.SetTileType(TileType.none);

            SetDisplayProperties(editTile);

            return editTile;
        }


        private BoardHexagon GetBoardTile(double x, double y)
        {
            BoardHexagon boardTile = new BoardHexagon();
            boardTile.MouseLeftButtonDown += AddNewColour;
            boardTile.MouseRightButtonDown += RemoveHexagonTile;
            Board.Children.Add(boardTile);

            boardTile.SetTileType(TileType.board);

            Canvas.SetLeft(boardTile, x);
            Canvas.SetTop(boardTile, y);

            SetDisplayProperties(boardTile);

            return boardTile;
        }


        private BoardHexagon GetBoardTile(double x, double y, TileType tileType)
        {
            BoardHexagon boardTile = new BoardHexagon();

            boardTile.SetTileType(tileType);

            boardTile.MouseLeftButtonDown += AddPropertyToColour;
            boardTile.MouseRightButtonDown += RemoveColouredTile;
            Board.Children.Add(boardTile);

            Canvas.SetLeft(boardTile, x);
            Canvas.SetTop(boardTile, y);

            SetDisplayProperties(boardTile);

            return boardTile;
        }


        private void SetDisplayProperties(BoardHexagon boardTile)
        {
            if (this.displayBoneStructure)
            {
                boardTile.ShowLinks(true);
                boardTile.ShowTileValues(false);
            }
            else
            {
                boardTile.ShowLinks(false);
                boardTile.ShowTileValues(true);
            }
        }

        private void ReplaceEditSpot(HexagonTile tile)
        {
            BoardHexagon control = hexagonList[tile];

            control.MouseLeftButtonDown -= AddHexagonTile;
            control.MouseLeftButtonDown += AddNewColour;
            control.MouseRightButtonDown += RemoveHexagonTile;

            tile.TileType = TileType.board;
            tile.TileValue = defaultTileValue;
            control.SetTileType(TileType.board);
        }


        private void AddHexagonTile(object sender, MouseButtonEventArgs e)
        {
            BoardHexagon control = sender as BoardHexagon;

            HexagonTile tile = hexagonList[control];

            if (tile != null)
            {
                this.ReplaceEditSpot(tile);
            }


            if (tile.North == null)
            {
                tile.North = new HexagonTile()
                {
                    TileType = TileType.none,
                    South = tile,
                    TileValue = 0,
                };

                AddEditSpot(tile.North, tile.X, tile.Y - yFull);
            }

            if (tile.South == null)
            {
                tile.South = new HexagonTile()
                {
                    TileType = TileType.none,
                    North = tile,
                    TileValue = 0,
                };

                AddEditSpot(tile.South, tile.X, tile.Y + yFull);
            }

            if (tile.NorthWest == null)
            {
                tile.NorthWest = new HexagonTile()
                {
                    TileType = TileType.none,
                    SouthEast = tile,
                    TileValue = 0,
                };

                AddEditSpot(tile.NorthWest, tile.X - xMarge, tile.Y - yMarge);
            }

            if (tile.NorthEast == null)
            {
                tile.NorthEast = new HexagonTile()
                {
                    TileType = TileType.none,
                    SouthWest = tile,
                    TileValue = 0,
                };

                AddEditSpot(tile.NorthEast, tile.X + xMarge, tile.Y - yMarge);
            }

            if (tile.SouthWest == null)
            {
                tile.SouthWest = new HexagonTile()
                {
                    TileType = TileType.none,
                    NorthEast = tile,
                    TileValue = 0,
                };
                AddEditSpot(tile.SouthWest, tile.X - xMarge, tile.Y + yMarge);
            }

            if (tile.SouthEast == null)
            {
                tile.SouthEast = new HexagonTile()
                {
                    TileType = TileType.none,
                    NorthWest = tile,
                    TileValue = 0,
                };
                AddEditSpot(tile.SouthEast, tile.X + xMarge, tile.Y + yMarge);
            }

            MapUpTile(tile.North);
            MapUpTile(tile.NorthWest);
            MapUpTile(tile.NorthEast);
            MapUpTile(tile.South);
            MapUpTile(tile.SouthWest);
            MapUpTile(tile.SouthEast);

            hexagonList[tile.North].SetLinkCount(tile.North.LinkCount);
            hexagonList[tile.NorthWest].SetLinkCount(tile.NorthWest.LinkCount);
            hexagonList[tile.NorthEast].SetLinkCount(tile.NorthEast.LinkCount);
            hexagonList[tile.South].SetLinkCount(tile.South.LinkCount);
            hexagonList[tile.SouthEast].SetLinkCount(tile.SouthEast.LinkCount);
            hexagonList[tile.SouthWest].SetLinkCount(tile.SouthWest.LinkCount);
            hexagonList[tile].SetLinkCount(tile.LinkCount);

            CreateMappings();
        }


        private void RemoveHexagonTile(object sender, MouseButtonEventArgs e)
        {
            BoardHexagon tileControl = sender as BoardHexagon;

            HexagonTile tile = hexagonList[tileControl];

            HexagonTile[] neighbours =
                new[]{
                    tile.North,
                    tile.NorthEast,
                    tile.NorthWest,
                    tile.South,
                    tile.SouthEast,
                    tile.SouthWest,
                };

            ReplaceBoardTile(tile);

            foreach (HexagonTile neighbour in neighbours)
            {
                if (CanBeRemoved(neighbour))
                {
                    DetachTile(neighbour);
                    Board.Children.Remove(hexagonList[neighbour]);
                    hexagonList.Remove(neighbour);
                }
            }


            if (CanBeRemoved(tile))
            {
                DetachTile(tile);
                Board.Children.Remove(hexagonList[tile]);
                hexagonList.Remove(tile);
            }

            if (tile.North != null)
            {
                hexagonList[tile.North].SetLinkCount(tile.North.LinkCount);
            }
            if (tile.NorthWest != null)
            {
                hexagonList[tile.NorthWest].SetLinkCount(tile.NorthWest.LinkCount);
            }
            if (tile.NorthEast != null)
            {
                hexagonList[tile.NorthEast].SetLinkCount(tile.NorthEast.LinkCount);
            }
            if (tile.South != null)
            {
                hexagonList[tile.South].SetLinkCount(tile.South.LinkCount);
            }
            if (tile.SouthEast != null)
            {
                hexagonList[tile.SouthEast].SetLinkCount(tile.SouthEast.LinkCount);
            }
            if (tile.SouthWest != null)
            {
                hexagonList[tile.SouthWest].SetLinkCount(tile.SouthWest.LinkCount);
            }

            hexagonList[tile].SetLinkCount(tile.LinkCount);

            CreateMappings();
        }


        private void RemoveColouredTile(object sender, MouseButtonEventArgs e)
        {
            BoardHexagon control = (BoardHexagon)sender;
            
            control.MouseRightButtonDown -= RemoveColouredTile;
            control.MouseLeftButtonDown -= AddPropertyToColour;
            control.MouseLeftButtonDown += AddNewColour;
            control.MouseRightButtonDown += RemoveHexagonTile;

            control.SetTileType(TileType.board);
            control.ShowFortress(false);

            hexagonList[control].TileType = TileType.board;

            CreateMappings();
        }


        private void ReplaceBoardTile(HexagonTile tile)
        {
            hexagonList[tile].MouseRightButtonDown -= RemoveHexagonTile;
            hexagonList[tile].MouseLeftButtonDown -= AddNewColour;
            hexagonList[tile].MouseLeftButtonDown += AddHexagonTile;

            tile.TileType = TileType.none;
            tile.TileValue = 0;
            hexagonList[tile].SetTileType(TileType.none);
        }


        #region private void DetachTile(HexagonTile removeTile)
        private void DetachTile(HexagonTile removeTile)
        {
            if (removeTile.North != null)
            {
                removeTile.North.South = null;
                removeTile.North = null;
            }

            if (removeTile.NorthEast != null)
            {
                removeTile.NorthEast.SouthWest = null;
                removeTile.NorthEast = null;
            }

            if (removeTile.NorthWest != null)
            {
                removeTile.NorthWest.SouthEast = null;
                removeTile.NorthWest = null;
            }

            if (removeTile.South != null)
            {
                removeTile.South.North = null;
                removeTile.South = null;
            }

            if (removeTile.SouthEast != null)
            {
                removeTile.SouthEast.NorthWest = null;
                removeTile.SouthEast = null;
            }

            if (removeTile.SouthWest != null)
            {
                removeTile.SouthWest.NorthEast = null;
                removeTile.SouthWest = null;
            }
        }
        #endregion


        #region private bool HasActiveNeighbour(HexagonTile tile)
        private bool CanBeRemoved(HexagonTile tile)
        {
            if (tile.TileType != TileType.none)
            {
                return false;
            }

            HexagonTile[] neighbours =
                new[]{
                    tile.North,
                    tile.NorthEast,
                    tile.NorthWest,
                    tile.South,
                    tile.SouthEast,
                    tile.SouthWest,
                };

            foreach (HexagonTile neighbour in neighbours)
            {
                if (neighbour != null && neighbour.TileType != TileType.none)
                {
                    return false;
                }
            }


            return true;
        }
        #endregion


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



        private Dictionary<HexagonTile, List<HexagonTile>> mappings = new Dictionary<HexagonTile, List<HexagonTile>>();

        private void CreateMappings()
        {
            foreach (HexagonTile t in hexagonList)
            {
                if (t.TileType != TileType.none)
                {
                    this.startTile = t;
                    break;
                }
            }


            mappings.Clear();
            CreateMappings(this.startTile);

            CrossSections.Children.Clear();

            foreach (HexagonTile key in mappings.Keys)
            {
                foreach (HexagonTile tile in mappings[key])
                {
                    CreateLine(key, tile);
                }
            }
        }


        private void CreateMappings(HexagonTile tile)
        {
            if (tile.TileType != TileType.none)
            {
                if (!mappings.ContainsKey(tile))
                {
                    mappings.Add(tile, new List<HexagonTile>());
                }

                if (tile.North != null)
                {

                    if (tile.North.South != tile)
                    {
                        throw new Exception("mapping incomplete");
                    }

                    if (!MappingExists(tile.North, tile))
                    {
                        mappings[tile].Add(tile.North);

                        CreateMappings(tile.North);
                    }
                }

                if (tile.South != null)
                {
                    if (tile.South.North != tile)
                    {
                        throw new Exception("mapping incomplete");
                    }

                    if (!MappingExists(tile.South, tile))
                    {
                        mappings[tile].Add(tile.South);

                        CreateMappings(tile.South);
                    }
                }

                if (tile.NorthEast != null)
                {
                    if (tile.NorthEast.SouthWest != tile)
                    {
                        throw new Exception("mapping incomplete");
                    }

                    if (!MappingExists(tile.NorthEast, tile))
                    {
                        mappings[tile].Add(tile.NorthEast);

                        CreateMappings(tile.NorthEast);
                    }
                }

                if (tile.NorthWest != null)
                {
                    if (tile.NorthWest.SouthEast != tile)
                    {
                        throw new Exception("mapping incomplete");
                    }

                    if (!MappingExists(tile.NorthWest, tile))
                    {
                        mappings[tile].Add(tile.NorthWest);

                        CreateMappings(tile.NorthWest);
                    }
                }

                if (tile.SouthEast != null)
                {
                    if (tile.SouthEast.NorthWest != tile)
                    {
                        throw new Exception("mapping incomplete");
                    }

                    if (!MappingExists(tile.SouthEast, tile))
                    {
                        mappings[tile].Add(tile.SouthEast);

                        CreateMappings(tile.SouthEast);
                    }
                }

                if (tile.SouthWest != null)
                {
                    if (tile.SouthWest.NorthEast != tile)
                    {
                        throw new Exception("mapping incomplete");
                    }

                    if (!MappingExists(tile.SouthWest, tile))
                    {
                        mappings[tile].Add(tile.SouthWest);

                        CreateMappings(tile.SouthWest);
                    }
                }
            }
        }


        private bool MappingExists(HexagonTile a, HexagonTile b)
        {
            if (mappings.ContainsKey(a) && mappings[a].Contains(b))
            {
                return true;
            }

            if (mappings.ContainsKey(b) && mappings[b].Contains(a))
            {
                return true;
            }

            if (a.TileType == TileType.none || b.TileType == TileType.none)
            {
                return true;
            }

            return false;
        }


        private void CreateLine(HexagonTile a, HexagonTile b)
        {
            double dx1 = a.X + 30.0;
            double dy1 = a.Y + 26;

            double dx2 = b.X + 30.0;
            double dy2 = b.Y + 26.0;

            Line l1 = new Line()
            {
                X1 = dx1,
                Y1 = dy1,
                X2 = dx2,
                Y2 = dy2,
                Stroke = new SolidColorBrush(Colors.Purple)
            };

            l1.Opacity = 0.9;
            l1.StrokeDashArray = new DoubleCollection() { 1, 2 };
            this.CrossSections.Children.Add(l1);
        }


        public BoardSerializable GetSaveData()
        {
            int id = 0;

            BoardSerializable boardData = new BoardSerializable();

            boardData.FortressesPerPlayer = int.Parse(this.Fortresses.Text);

            foreach (HexagonTile tile in hexagonList)
            {
                HexagonTileSerializable serializableTile = new HexagonTileSerializable(tile);

                // temp code to fill TileValue
                //tile.Fortress = false;

                serializableTile.Id = id++;
                boardData.ActiveTileList.Add(serializableTile);
            }

            return boardData;
        }


        public void LoadSaveData(BoardSerializable boardData)
        {
            Dictionary<HexagonTileSerializable, HexagonTile> mapping = new Dictionary<HexagonTileSerializable, HexagonTile>();

            this.hexagonList.Clear();
            this.Board.Children.Clear();

            this.Fortresses.Text = boardData.FortressesPerPlayer.ToString();

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

                mapping.Add(serializedTile, tile);

                BoardHexagon board = null;

                switch (tile.TileType)
                {
                    case TileType.none:
                        board = (BoardHexagon)this.GetEditTile(serializedTile.X, serializedTile.Y);
                        board.SetTileType(tile.TileType);
                        board.SetTileValue(tile.TileValue);
                        board.ShowFortress(tile.Fortress);
                        this.hexagonList.Add(tile, board);
                        break;
                    case TileType.board:
                        board = (BoardHexagon)this.GetBoardTile(serializedTile.X, serializedTile.Y);
                        board.SetTileType(tile.TileType);
                        this.hexagonList.Add(tile, board);
                        board.SetTileValue(tile.TileValue);
                        board.ShowFortress(tile.Fortress);
                        break;
                    case TileType.blue:
                    case TileType.red:
                    case TileType.yellow:
                        board = (BoardHexagon)this.GetBoardTile(serializedTile.X, serializedTile.Y, tile.TileType);
                        board.SetTileType(tile.TileType);
                        board.SetTileValue(tile.TileValue);
                        board.ShowFortress(tile.Fortress);
                        this.hexagonList.Add(tile, board);
                        break;
                }
            }

            foreach (HexagonTile tile in hexagonList)
            {
                MapUpTile(tile);
            }

            foreach (HexagonTile tile in hexagonList)
            {
                hexagonList[tile].SetLinkCount(tile.LinkCount);
            }

            CreateMappings();
        }


        ITileControl currentSelection = null;
        private void OnSelectCurrent(object sender, MouseButtonEventArgs e)
        {
            Type type = sender.GetType();
            ConstructorInfo constructorInfo = type.GetConstructor(new Type[] { });

            ITileControl element = (ITileControl)constructorInfo.Invoke(new object[] { });
            ColorToAdd.Children.Clear();
            ColorToAdd.Children.Add((UIElement) element);
            this.currentSelection = element;
        }

        private void OnClearStatus(object sender, MouseButtonEventArgs e)
        {
            ClearStatus();
        }

        private void ClearStatus()
        {
            ColorToAdd.Children.Clear();
            this.currentSelection = null;
        }

        private void AddNewColour(object sender, MouseButtonEventArgs e)
        {
            if (currentSelection != null && currentSelection.TileType != TileType.none)
            {
                ColorToAdd.Children.Clear();

                BoardHexagon control = (BoardHexagon)sender;

                HexagonTile tile = hexagonList[control];

                if (tile.TileType != TileType.none)
                {
                    ReplaceBoardWithColour(tile);
                    hexagonList[tile].SetLinkCount(tile.LinkCount);
                    ClearStatus();
                }
                else
                {
                    throw new Exception("This tile must be active!");
                }
            }

            CreateMappings();
        }


        private void AddPropertyToColour(object sender, MouseButtonEventArgs e)
        {
            if (currentSelection != null && currentSelection.TileType == TileType.none)
            {
                BoardHexagon control = (BoardHexagon)sender;

                if (currentSelection is Fortress)
                {
                    ColorToAdd.Children.Clear();
                    control.ShowFortress(true);
                    hexagonList[control].Fortress = true;
                }
            }
        }


        private void ReplaceBoardWithColour(HexagonTile tile)
        {
            hexagonList[tile].MouseLeftButtonDown -= AddNewColour;
            hexagonList[tile].MouseLeftButtonDown += AddPropertyToColour;
            hexagonList[tile].MouseRightButtonDown -= RemoveHexagonTile;
            hexagonList[tile].MouseRightButtonDown += RemoveColouredTile;

            tile.TileType = this.currentSelection.TileType;
            hexagonList[tile].SetTileType(this.currentSelection.TileType);
        }


        private void DispUS_Changed(object sender, RoutedEventArgs e)
        {

            if (this.DispUnderlyingStructure.IsChecked.HasValue)
            {
                this.displayBoneStructure = this.DispUnderlyingStructure.IsChecked.Value;
            }

            if (this.displayBoneStructure)
            {
                this.CrossSections.Visibility = Visibility.Visible;
            }
            else
            {
                this.CrossSections.Visibility = Visibility.Hidden;
            }

            foreach (BoardHexagon tile in this.hexagonList.GetArrayK2())
            {
                this.SetDisplayProperties(tile);
            }

            e.Handled = true;
        }
    }
}
