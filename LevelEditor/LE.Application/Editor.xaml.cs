using System.Configuration;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using LE.GameEngine.board;
using System.Runtime.Serialization;

namespace LE.Application
{
    public partial class Editor : Window
    {
        public Editor()
        {
            InitializeComponent();
        }


        private void LoadFileList()
        {
            this.FileList.Items.Clear();

            foreach (string file in Directory.EnumerateFiles(ConfigurationManager.AppSettings["files"]))
            {
                this.FileList.Items.Add(System.IO.Path.GetFileName(file));
            }
        }


        private void SaveDataToFile(object sender, RoutedEventArgs e)
        {
            BoardSerializable boardData = EditorPanel.GetSaveData();

            using(FileStream saveFile = File.Open(System.IO.Path.Combine(ConfigurationManager.AppSettings["files"], this.Filename.Text), FileMode.Create))
            {
                DataContractSerializer formatter = new DataContractSerializer(typeof(BoardSerializable));
                formatter.WriteObject(saveFile, boardData);
            }

            LoadFileList();
        }


        private void OnLoadedFileList(object sender, RoutedEventArgs e)
        {
            LoadFileList();
        }


        private void LoadDataFromDisk(object sender, RoutedEventArgs e)
        {
            BoardSerializable boardData;
            if (this.FileList.SelectedItem != null)
            {
                using (FileStream loadFile = File.Open(System.IO.Path.Combine(ConfigurationManager.AppSettings["files"], this.FileList.SelectedItem.ToString()), FileMode.Open))
                {
                    DataContractSerializer formatter = new DataContractSerializer(typeof(BoardSerializable));

                    boardData = formatter.ReadObject(loadFile) as BoardSerializable;

                    this.EditorPanel.LoadSaveData(boardData);
                    this.Filename.Text = this.FileList.SelectedItem.ToString();
                }
            }
        }


        private void OnNewLevel(object sender, RoutedEventArgs e)
        {
            this.EditorPanel.InitNewLevel();
        }


        GameWindow game;
        private void button3_Click(object sender, RoutedEventArgs e)
        {
            game = new GameWindow();
            game.Show();

            BoardSerializable gameData =  EditorPanel.GetSaveData();
            game.LoadData(gameData);
            game.StartGame();
        }

    }
}
