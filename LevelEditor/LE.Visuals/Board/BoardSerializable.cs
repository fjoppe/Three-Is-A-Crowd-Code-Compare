using System;
using System.Collections.Generic;

namespace TIAC.Visuals.Board
{
    [Serializable]
    public class BoardSerializable
    {
        public List<HexagonTileSerializable> ActiveTileList=new List<HexagonTileSerializable>();
    }
}
