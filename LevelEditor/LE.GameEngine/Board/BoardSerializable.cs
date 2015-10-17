using System;
using System.Collections.Generic;
using LE.GameEngine.TIC_Webservice;
using System.Runtime.Serialization;

namespace LE.GameEngine.board
{
    [DataContract(Namespace = "urn:TIC")]
    public class BoardSerializable
    {
        [DataMember(Name = "FortressesPerPlayer")]
        public int FortressesPerPlayer { get; set; }

        [DataMember(Name = "ActiveTileList")]
        public List<HexagonTileSerializable> ActiveTileList=new List<HexagonTileSerializable>();
    }
}
