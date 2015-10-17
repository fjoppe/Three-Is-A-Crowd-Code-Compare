using System.Windows.Controls;
using LE.Application.Classes;
using LE.GameEngine.board;
using LE.Visuals.Board;
using LE.GameEngine.GameEngine;
using System.Collections.Generic;
using System;

namespace LE.Application
{
    /// <summary>
    /// Interaction logic for GamePanel2.xaml
    /// </summary>
    public partial class GamePanel2 : UserControl
    {
        private Game game = Game.JoinGame(Guid.NewGuid());

        private TwoWayMapper<int, BoardHexagon> board = new TwoWayMapper<int, BoardHexagon>();

        TileType currentColor;
        List<int> choiceList;

        
        public GamePanel2()
        {
            InitializeComponent();

            CurrentTurn.SetTileType(TileType.board);
            CurrentTurn.ShowLinks(false);
        }


        public void StartGame()
        {
            this.InitializeBoard();
            this.InitializeTurn();
        }

        private void InitializeBoard()
        {
            HexagonTileSerializable[] boardData = game.RetrieveBoardData();

            foreach (HexagonTileSerializable tile in boardData)
            {
                BoardHexagon element = this.GetBoardTile(tile.X, tile.Y, tile.TileType);
                this.board.Add(tile.Id, element);
            }
        }


        private void InitializeTurn()
        {
            UpdateBoard();
            UpdateStats();

            this.currentColor = game.GetCurrentTurn();
            this.CurrentTurn.SetTileType(this.currentColor);

            this.choiceList = game.GetPossibleMoves();

            foreach (int i in this.choiceList)
            {
                this.board[i].SetTileType(TileType.none);
                this.board[i].MouseLeftButtonDown += BoardChoice;
            }
        }


        private void BoardChoice(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            BoardHexagon control = (BoardHexagon)sender;
            int choice = this.board[control];

            game.ChooseTurn(choice, Guid.NewGuid());

            foreach (int i in this.choiceList)
            {
                this.board[i].SetTileType(TileType.board);
                this.board[i].MouseLeftButtonDown -= BoardChoice;
            }

            control.SetTileType(this.currentColor);

            InitializeTurn();
        }


        private BoardHexagon GetBoardTile(double x, double y, TileType tileType)
        {
            BoardHexagon element = new BoardHexagon();

            element.SetTileType(tileType);

            Board.Children.Add(element);

            Canvas.SetLeft(element, x);
            Canvas.SetTop(element, y);
            element.ShowLinks(false);
            return element;
        }


        private void UpdateBoard()
        {
            TileColor[] colors = game.GetBoardState();

            foreach (TileColor t in colors)
            {
                this.board[t.id].SetTileType(t.color);
            }
        }

        private void UpdateStats()
        {
            GameStats stats = game.GetGameStats();

            this.RedCount.Text = stats.RedCount.ToString();
            this.BlueCount.Text = stats.BlueCount.ToString();
            this.YellowCount.Text = stats.YellowCount.ToString();
        }

        #region To be moved later

        public void LoadSaveData(BoardSerializable boardData)
        {
            game.LoadSaveData(boardData);


        }

        #endregion
    }
}
