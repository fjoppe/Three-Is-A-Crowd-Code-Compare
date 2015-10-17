using System.Runtime.Serialization;
using GameEngine.CSharp.Game.Board;

namespace GameEngine.CSharp.Game.Engine
{
    public enum PlayerType { human, computer };

    [DataContract]
    public class PlayerProperties
    {
        [DataMember]
        public PlayerType PlayerType { get; set; }
    }
}
