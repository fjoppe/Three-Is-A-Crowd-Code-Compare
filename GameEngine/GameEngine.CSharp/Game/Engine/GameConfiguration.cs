using System;
using System.Runtime.Serialization;

namespace GameEngine.CSharp.Game.Engine
{
    [DataContract]
    public class GameConfiguration
    {
        [DataMember]
        public int numberOfAI { get; set; }

        [DataMember]
        public Guid GameId { get; set; }
    }
}
