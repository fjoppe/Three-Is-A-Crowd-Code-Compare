namespace GameEngine.FSharp

open System
open System.Runtime.Serialization

[<DataContract>]
type GameConfiguration = {
        [<field: DataMember(Name="numberOfAI") >]
        numberOfAI : int;

        [<field: DataMember(Name="GameId") >]
        GameId : Guid;
    }


