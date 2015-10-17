using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using LE.Application.Classes;
using LE.Visuals.Board;
using LE.GameEngine.TIC_Webservice;

namespace LE.Application
{
    public partial class GamePanel4 : UserControl
    {
        private object semaphor = new object();

        private Guid playerId = Guid.NewGuid();

        private TwoWayMapper<int, BoardHexagon> board = new TwoWayMapper<int, BoardHexagon>();

        TileType myColor = TileType.none;
        int[] choiceList;

        Thread gameThread;

        public Guid gameId { get; set; }


        CSharpServiceClient webservice = new CSharpServiceClient();

        private int MyFortresses=0;

        private bool FortressSelected = false;


        public GamePanel4()
        {
            InitializeComponent();
        }


        public void ClosePanel()
        {
            this.gameEnded = true;
            gameThread.Abort();
        }


        public void StartGame()
        {
            webservice.JoinGame(this.gameId, this.playerId);
            this.MyTurn.SetTileType(TileType.board);
            this.MyTurn.ShowLinks(false);
            this.MyTurn.ShowTileValues(false);

            this.InitializeBoard();
            this.gameThread = new Thread(GameLoop);
            this.gameThread.Start();
        }


        private BoardHexagon GetBoardTile(double x, double y, TileType tileType, int tileValue)
        {
            BoardHexagon element = new BoardHexagon();

            element.SetTileType(tileType);

            Board.Children.Add(element);

            Canvas.SetLeft(element, x);
            Canvas.SetTop(element, y);

            element.ShowLinks(true);
            element.ShowTileValues(false);
            element.SetLinkCount(tileValue);

            return element;
        }


        private void InitializeBoard()
        {
            BoardSerializable boardData = this.webservice.RetrieveBoardData(this.gameId);

            this.MyFortresses = boardData.FortressesPerPlayer;
            this.SetFortressesDisplay();

            foreach (HexagonTileSerializable tile in boardData.ActiveTileList)
            {
                BoardHexagon element = this.GetBoardTile(tile.X, tile.Y, tile.TileType, tile.TileValue);
                element.ShowFortress(tile.Fortress);
                this.board.Add(tile.Id, element);
            }
        }


        private void SetFortressesDisplay()
        {
            this.Fortresses.Dispatcher.BeginInvoke((Action)(()=>this.Fortresses.Text = this.MyFortresses.ToString()));
        }

        private void GetMyColor()
        {
            while (this.myColor == TileType.none)
            {
                this.myColor = this.webservice.WhatIsMyColor(this.gameId, this.playerId);
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
            }

            MyTurn.Dispatcher.BeginInvoke((Action)(() => MyTurn.SetTileType(myColor)));
        }


        bool gameEnded = false;

        void GameLoop()
        {
            this.GetMyColor();

            while (!this.gameEnded)
            {
                UpdateStats();
                UpdateBoard();

                switch (this.webservice.WhatIsMyStatus(this.gameId, this.playerId))
                {
                    case PlayerStatus.itsMyTurn:
                        UpdateStats();
                        UpdateBoard();
                        MyTurn.Dispatcher.BeginInvoke((Action)(() => MyTurn.SetTileType(this.myColor)));
                        InitializeTurn();
                        lock (semaphor)
                        {
                            Monitor.Wait(semaphor);
                        }
                        break;

                    case PlayerStatus.noMoves:
                        NoMoves.Dispatcher.BeginInvoke((Action)(() => {
                            NoMoves.Visibility = Visibility.Visible;
                            Thread.Sleep(TimeSpan.FromSeconds(2));
                            NoMoves.Visibility = Visibility.Collapsed;
                        }));
                        break;

                    case PlayerStatus.gameOver:
                        GameOver.Dispatcher.BeginInvoke((Action)(() => GameOver.Visibility = Visibility.Visible));
                        this.gameEnded = true;
                        break;

                    default:
                        MyTurn.Dispatcher.BeginInvoke((Action)(() => MyTurn.SetTileType(TileType.board)));
                        break;
                }

                Thread.Sleep(TimeSpan.FromSeconds(0.5));
            }
        }


        private void InitializeTurn()
        {
            this.choiceList = this.webservice.GetPossibleMoves(this.gameId, this.playerId);

            foreach (int i in this.choiceList)
            {
                int p = i;
                this.board[i].Dispatcher.BeginInvoke((Action)(() =>
                {
                    this.board[p].SetTileType(TileType.none);
                    this.board[p].MouseLeftButtonDown += BoardChoice;
                }));
            }
        }


        private void BoardChoice(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            BoardHexagon control = (BoardHexagon)sender;
            int choice = this.board[control];

            if (!this.FortressSelected)
            {
                this.webservice.ChooseTurn(this.gameId, this.playerId, choice);
            }
            else
            {
                this.webservice.ChooseFortressedTurn(this.gameId, this.playerId, choice);
                this.MyFortresses--;
            }

            this.FortressSelected = false;
            this.DisplayFortressSelected();

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
            this.DisplayFortressSelected();

            TileColor[] colors = this.webservice.GetBoardState(this.gameId);

            foreach (TileColor t in colors)
            {
                TileColor tCopy = t;
                this.board[tCopy.id].Dispatcher.BeginInvoke((Action)(() => 
                {
                    this.board[tCopy.id].SetTileType(tCopy.color);
                    this.board[tCopy.id].ShowFortress(tCopy.Fortress);
                }));
            }
        }


        private void UpdateStats()
        {
            GameStats stats = this.webservice.GetGameStats(this.gameId);

            this.RedCount.Dispatcher.BeginInvoke((Action)(() => this.RedCount.Text = stats.RedCount.ToString()));
            this.BlueCount.Dispatcher.BeginInvoke((Action)(() => this.BlueCount.Text = stats.BlueCount.ToString()));
            this.YellowCount.Dispatcher.BeginInvoke((Action)(() => this.YellowCount.Text = stats.YellowCount.ToString()));

            this.RedFortress.Dispatcher.BeginInvoke((Action)(()=> this.RedFortress.Text = stats.RedFortress.ToString()));
            this.BlueFortress.Dispatcher.BeginInvoke((Action)(() => this.BlueFortress.Text = stats.BlueFortress.ToString()));
            this.YellowFortress.Dispatcher.BeginInvoke((Action)(() => this.YellowFortress.Text = stats.YellowFortress.ToString()));
        }


        private void OnFortressSelected(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.MyFortresses > 0)
            {
                this.FortressSelected = !this.FortressSelected;
                this.DisplayFortressSelected();
            }
        }


        private void DisplayFortressSelected()
        {
            this.FortressSelect.Dispatcher.BeginInvoke((Action)(()=>this.FortressSelect.ShowSelected(this.FortressSelected)));
            this.SetFortressesDisplay();
        }
    }
}
