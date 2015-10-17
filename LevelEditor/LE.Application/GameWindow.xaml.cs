using System.Windows;
using LE.GameEngine.board;
using LE.GameEngine.GameEngine;
using LE.GameEngine.TIC_Webservice;
using System;

namespace LE.Application
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        public GameWindow()
        {
            InitializeComponent();
        }

        CSharpServiceClient webservice = new CSharpServiceClient();

        internal void LoadData(LE.GameEngine.board.BoardSerializable gameData)
        {
            //Player1.LoadSaveData(gameData);
            //Game.StartNewGame();

        }

        internal void StartGame()
        {
            Player1.gameId = Guid.NewGuid();

            GameConfiguration gameConfiguration = new GameConfiguration()
            {
                numberOfAI= 2,
                GameId = Player1.gameId,                
            };

            webservice.StartNewGame(gameConfiguration);

            Player1.StartGame();
            //Player2.StartGame();
            //Player3.StartGame();
        }

        protected override void OnClosed(System.EventArgs e)
        {
            base.OnClosed(e);
            this.Player1.ClosePanel();
            //this.Player2.ClosePanel();
            //this.Player3.ClosePanel();
        }
    }
}
