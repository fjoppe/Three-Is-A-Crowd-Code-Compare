using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel.Activation;
using System.Threading;
using System.Web.Hosting;
using System.Xml.Serialization;
using NLog;
using GameEngine.CSharp.Game.AI;
using GameEngine.CSharp.Game.Board;
using GameEngine.CSharp.Game.Engine;
using TileType = GameEngine.CSharp.Game.Board.TileType;

namespace GameEngine.WebServer.Services
{

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class GameServiceCSharp : ICSharpService
    {

        private Logger logger = LogManager.GetLogger("debug");
        private Game current;


        public GameServiceCSharp()
        {
        }


        public Guid GetUniqueIdentifier()
        {
            return Guid.NewGuid();
        }


        public void StartNewGame(GameConfiguration configuration)
        {
            File.Delete(HostingEnvironment.MapPath("/App_Data/TIC.log"));
            logger.Debug("StartNewGame()");
            try
            {
                current = null;
                if (File.Exists(Filename(configuration.GameId)))
                {
                    File.Delete(Filename(configuration.GameId));
                }

                current = new Game(configuration);
                LoadLevel();

                this.SaveGame(configuration.GameId);
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw;
            }
        }


        public void JoinGame(Guid gameId, Guid playerId)
        {
            try
            {
                RetrieveLock(gameId);
                logger.Debug("JoinGame({0})", playerId);

                this.LoadGame(gameId);

                if (current != null)
                {
                    current.JoinGame(playerId);

                    this.SaveGame(gameId);
                }
                else
                {
                    logger.Error("No game data available for {0}");
                    throw new Exception("No game data available");
                }
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw;
            }
            finally
            {
                ReleaseLock();
            }
        }


        public TileType GetCurrentTurn(Guid gameId)
        {
            try
            {
                RetrieveLock(gameId);
                logger.Debug("GetCurrentTurn({0})", gameId);

                this.LoadGame(gameId);
                TileType result = TileType.none;
                if (current != null)
                {
                    result = current.GetCurrentTurn();
                }
                else
                {
                    logger.Error("No game data available for {0}", gameId);
                    throw new Exception("No game data available");
                }
                this.SaveGame(gameId);

                return result;
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw;
            }
            finally
            {
                ReleaseLock();
            }
        }


        public List<int> GetPossibleMoves(Guid gameId, Guid playerId)
        {
            try
            {
                RetrieveLock(gameId);
                this.LoadGame(gameId);
                List<int> result = null;
                if (current != null)
                {
                    logger.Debug("GetPossibleMoves({0})", current.GetMyColor(playerId));
                    result = current.GetPossibleMoves(playerId);
                }
                else
                {
                    logger.Error("No game data available for {0}", gameId);
                    throw new Exception("No game data available");
                }
                this.SaveGame(gameId);

                return result;
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw;
            }
            finally
            {
                ReleaseLock();
            }
        }


        public void ChooseTurn(Guid gameId, Guid playerId, int id)
        {
            try
            {
                RetrieveLock(gameId);
                this.LoadGame(gameId);
                if (current != null)
                {
                    logger.Debug("ChooseTurn({0}, {1})", current.GetMyColor(playerId), id);
                    current.ChooseTurn(id, playerId);
                }
                else
                {
                    logger.Error("No game data available for {0}", gameId);
                    throw new Exception("No game data available");
                }
                this.SaveGame(gameId);
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw;
            }
            finally
            {
                ReleaseLock();
            }
        }


        public void ChooseFortressedTurn(Guid gameId, Guid playerId, int id)
        {
            try
            {
                RetrieveLock(gameId);
                this.LoadGame(gameId);
                if (current != null)
                {
                    logger.Debug("ChooseTurn({0}, {1})", current.GetMyColor(playerId), id);


                    if (current.HasFortresses(playerId))
                    {
                        current.ChooseTurn(id, playerId);
                        current.ConsumeFortress(playerId, id);
                    }
                }
                else
                {
                    logger.Error("No game data available for {0}", gameId);
                    throw new Exception("No game data available");
                }
                this.SaveGame(gameId);
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw;
            }
            finally
            {
                ReleaseLock();
            }
        }


        public BoardSerializable RetrieveBoardData(Guid gameId)
        {
            try
            {
                RetrieveLock(gameId);
                this.LoadGame(gameId);
                logger.Debug("RetrieveBoardData({0})", gameId);
                BoardSerializable result = null;
                if (current != null)
                {
                    result = current.RetrieveBoardData();
                }
                else
                {
                    logger.Error("No game data available for {0}", gameId);
                    throw new Exception("No game data available");
                }
                this.SaveGame(gameId);

                return result;
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw;
            }
            finally
            {
                ReleaseLock();
            }
        }


        public TileColor[] GetBoardState(Guid gameId)
        {
            try
            {
                RetrieveLock(gameId);
                this.LoadGame(gameId);
                TileColor[] result = null;
                if (current != null)
                {
                    result = current.GetBoardState();
                }
                else
                {
                    logger.Error("No game data available for {0}", gameId);
                    throw new Exception("No game data available");
                }
                this.SaveGame(gameId);

                return result;
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw;
            }
            finally
            {
                ReleaseLock();
            }
        }


        public GameStats GetGameStats(Guid gameId)
        {
            try
            {
                RetrieveLock(gameId);
                this.LoadGame(gameId);
                GameStats result = null;
                if (current != null)
                {
                    result = current.GetGameStats();
                }
                else
                {
                    logger.Error("No game data available for {0}", gameId);
                    throw new Exception("No game data available");
                }
                this.SaveGame(gameId);

                return result;
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw;
            }
            finally
            {
                ReleaseLock();
            }
        }


        public TileType WhatIsMyColor(Guid gameId, Guid playerId)
        {
            try
            {
                RetrieveLock(gameId);
                this.LoadGame(gameId);

                TileType result = TileType.none;

                if (current != null)
                {
                    logger.Debug("WhatIsMyColor({0})", current.GetMyColor(playerId));
                    result = current.GetMyColor(playerId);
                }
                else
                {
                    logger.Error("No game data available for {0}", gameId);
                    throw new Exception("No game data available");
                }
                this.SaveGame(gameId);

                return result;
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw;
            }
            finally
            {
                ReleaseLock();
            }
        }


        public PlayerStatus WhatIsMyStatus(Guid gameId, Guid playerId)
        {
            try
            {
                RetrieveLock(gameId);
                this.LoadGame(gameId);
                PlayerStatus result = PlayerStatus.none;
                if (current != null)
                {
                    result = current.WhatIsMyStatus(playerId);

                    if (result == PlayerStatus.none && this.current.AITurn.Count>0)
                    {
                        Guid AIPlayerId = this.current.AITurn.Dequeue();
                        StartAI(gameId, AIPlayerId);
                    }
                    //if (result == PlayerStatus.triggerAI)
                    //{
                    //    StartAI(gameId, playerId);
                    //}
                }
                else
                {
                    logger.Error("No game data available for {0}", gameId);
                    throw new Exception("No game data available");
                }
                this.SaveGame(gameId);

                return result;
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw;
            }
            finally
            {
                ReleaseLock();
            }
        }


        #region Game Storage
        private void LoadGame(Guid gameId)
        {
            bool exit = false;
            while (!exit)
            {
                try
                {
                    using (FileStream loadFile = File.Open(Filename(gameId), FileMode.Open))
                    {
                        DataContractSerializer formatter = new DataContractSerializer(typeof(Game));
                        current = (Game)formatter.ReadObject(loadFile);
                    }
                    exit = true;
                }
                catch (FileNotFoundException ex)
                {
                    break;
                }
                catch (IOException ex)
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(1.0));
                }
            }
        }


        private void SaveGame(Guid gameId)
        {
            bool exit = false;

            while (!exit)
            {
                try
                {
                    using (FileStream saveFile = File.Open(Filename(gameId), FileMode.Create))
                    {
                        DataContractSerializer formatter = new DataContractSerializer(typeof(Game));
                        formatter.WriteObject(saveFile, current);
                    }
                    exit = true;
                }
                catch (IOException ex)
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(1.0));
                }
            }
        }


        private string Filename(Guid gameGuid)
        {
            return HostingEnvironment.MapPath(String.Format("/App_Data/{0}.xml", gameGuid));
        }

        #endregion


        #region Level Storage

        private void LoadLevel()
        {
            BoardSerializable boardData;

            using (FileStream loadFile = File.Open(ConfigurationManager.AppSettings["LevelData"], FileMode.Open))
            {
                DataContractSerializer formatter = new DataContractSerializer(typeof(BoardSerializable));

                boardData = formatter.ReadObject(loadFile) as BoardSerializable;

                //this.current = new Game();
                this.current.LoadSaveData(boardData);
            }
        }

        #endregion


        #region file locking
        private string LockFile(Guid gameId)
        {
            return HostingEnvironment.MapPath(string.Format("{1}/{0}.lck", gameId, ConfigurationManager.AppSettings["DataFolder"]));
        }

        FileStream lockFile;
        private void RetrieveLock(Guid gameId)
        {
            Random rnd = new Random();

            bool exit =false;
            while (!exit)
            {
                try
                {
                    string lockName = this.LockFile(gameId);
                    lockFile = File.Open(lockName, FileMode.Create);
                    exit = true;
                }
                catch
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(rnd.NextDouble()*0.2));
                }
            }
        }

        private void ReleaseLock()
        {
            lockFile.Close();
        }
        #endregion


        #region AI

        internal class AIThreadParams
        {
            public AIThreadParams(Guid gameId, Guid playerId)
            {
                this.gameId = gameId;
                this.playerId = playerId;
            }

            public Guid gameId { get; set; }
            public Guid playerId { get; set; }
        }

        private void StartAI(Guid gameId, Guid playerId)
        {
            Thread AI = new Thread(AIThread);
            AI.Start(new AIThreadParams(gameId, playerId));
        }

        private void AIThread(object threadParams)
        {
            AIThreadParams parms = threadParams as AIThreadParams;
            try
            {
                RetrieveLock(parms.gameId);
                this.LoadGame(parms.gameId);

                AIPlayer ai = new AIPlayer(this.current, parms.gameId, parms.playerId);

                ai.StartAI();

                this.SaveGame(parms.gameId);

            }
            catch (Exception e)
            {
                logger.Error(e);
                throw;
            }
            finally
            {
                ReleaseLock();
            }
        }

        #endregion
    }
}
