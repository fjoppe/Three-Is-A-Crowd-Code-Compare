using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Controls;
using LE.Application.Classes;
using LE.GameEngine.board;
using LE.Visuals.Board;
using LE.GameEngine.GameEngine;
using System.Windows;
using LE.GameEngine.TIC.Service;

namespace LE.Application
{
    /// <summary>
    /// Interaction logic for GamePanel3.xaml
    /// </summary>
    public partial class GamePanel3 : UserControl
    {
        private object semaphor = new object();

        private Guid playerId = Guid.NewGuid();

        private Game game;

        private TwoWayMapper<int, BoardHexagon> board = new TwoWayMapper<int, BoardHexagon>();

        TileType myColor=TileType.none;
        List<int> choiceList;

        Thread gameThread;

        public GamePanel3()
        {
            game = Game.JoinGame(playerId);

            InitializeComponent();

            this.MyTurn.SetTileType(TileType.board);
            this.MyTurn.ShowLinks(false);
        }

        public void ClosePanel()
        {
            this.gameEnded = true;
            gameThread.Abort();
        }

        public void StartGame()
        {
            this.InitializeBoard();
            this.gameThread = new Thread(GameLoop);
            this.gameThread.Start();
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

        private void InitializeBoard()
        {
            HexagonTileSerializable[] boardData = game.RetrieveBoardData();

            foreach (HexagonTileSerializable tile in boardData)
            {
                BoardHexagon element = this.GetBoardTile(tile.X, tile.Y, tile.TileType);
                this.board.Add(tile.Id, element);
            }
        }

        private void GetMyColor()
        {
            while (this.myColor == TileType.none)
            {
                this.myColor = this.game.GetMyColor(this.playerId);
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
            }

            MyTurn.Dispatcher.BeginInvoke((Action)(()=>MyTurn.SetTileType(myColor)));
        }


        bool gameEnded = false;

        void GameLoop()
        {
            this.GetMyColor();

            while (!this.gameEnded)
            {
                UpdateStats();
                UpdateBoard();

                switch(game.WhatIsMyStatus(this.playerId))
                {
                    case PlayerStatus.itsMyTurn:
                        MyTurn.Dispatcher.BeginInvoke((Action)(() => MyTurn.SetTileType(this.myColor)));
                        InitializeTurn();
                        lock (semaphor)
                        {
                            Monitor.Wait(semaphor);
                        }
                        break;
                    
                    case PlayerStatus.noMoves:
                        NoMoves.Dispatcher.BeginInvoke((Action)(()=>NoMoves.Visibility = Visibility.Visible));
                        Thread.Sleep(5);
                        NoMoves.Dispatcher.BeginInvoke((Action)(()=>NoMoves.Visibility = Visibility.Collapsed));
                        break;
                    
                    case PlayerStatus.gameOver:
                        GameOver.Dispatcher.BeginInvoke((Action)(()=>GameOver.Visibility = Visibility.Visible));
                        this.gameEnded = true;
                        break;

                    default:
                        MyTurn.Dispatcher.BeginInvoke((Action)(() => MyTurn.SetTileType(TileType.board)));
                        break;
                }

                Thread.Sleep(TimeSpan.FromSeconds(0.2));
            }
        }


        private void InitializeTurn()
        {
            this.choiceList = game.GetPossibleMoves();

            foreach (int i in this.choiceList)
            {
                int p = i;
                this.board[i].Dispatcher.BeginInvoke((Action)(() =>{
                    this.board[p].SetTileType(TileType.none);
                    this.board[p].MouseLeftButtonDown += BoardChoice;
                }));
            }
        }


        private void BoardChoice(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            BoardHexagon control = (BoardHexagon)sender;
            int choice = this.board[control];

            game.ChooseTurn(choice, this.playerId);

            foreach (int i in this.choiceList)
            {
                int p = i;
                this.board[i].Dispatcher.BeginInvoke(
                    (Action)(() =>
                    {
                        this.board[p].SetTileType(TileType.board);
                        this.board[p].MouseLeftButtonDown -= BoardChoice;
                    }));
            }

            control.SetTileType(TileType.board);

            UpdateBoard();
            lock (semaphor)
            {
                Monitor.Pulse(semaphor);
            }
        }


        private void UpdateBoard()
        {
            TileColor[] colors = game.GetBoardState();

            foreach (TileColor t in colors)
            {
                TileColor tCopy = t;
                this.board[tCopy.id].Dispatcher.BeginInvoke((Action)(() => this.board[tCopy.id].SetTileType(tCopy.color)));
            }
        }


        private void UpdateStats()
        {
            GameStats stats = game.GetGameStats();

            this.RedCount.Dispatcher.BeginInvoke((Action)(()=>this.RedCount.Text = stats.RedCount.ToString()));
            this.BlueCount.Dispatcher.BeginInvoke((Action)(()=>this.BlueCount.Text = stats.BlueCount.ToString()));
            this.YellowCount.Dispatcher.BeginInvoke((Action)(() => this.YellowCount.Text = stats.YellowCount.ToString()));
        }

        #region To be moved later

        public void LoadSaveData(BoardSerializable boardData)
        {
            game.LoadSaveData(boardData);
        }

        #endregion
    }
}
