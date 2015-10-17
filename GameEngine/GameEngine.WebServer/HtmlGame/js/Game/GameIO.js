// ====================================================
//  =======  Handle all Game IO with the server =======
// ====================================================

//  constants

//  The following defines the endpoint of the game server. Switch comment/uncomment if you want to switch between CSharp and FSharp WS. 
var gameEndpoint = "/Services/GameService.CSharp.svc/Json/";
//var gameEndpoint = "/Services/GameService.FSharp.svc/Json/";


//  ====== Game web calls ======

function GetGameId(SuccessCallback, ErrorCallback) {
    CallWebservice("GetUniqueIdentifier", {}, SuccessCallback, ErrorCallback);
}

function GetPlayerId(SuccessCallback, ErrorCallback) {
    CallWebservice("GetUniqueIdentifier", {}, SuccessCallback, ErrorCallback);
}

function StartNewGame(gameId, SuccessCallback, ErrorCallback) {
    CallWebservice("StartNewGame", JSON.stringify({ configuration: { GameId: gameId, numberOfAI: 2} }), SuccessCallback, ErrorCallback);
}

function JoinGame(gameId, playerId, SuccessCallback, ErrorCallback) {
    CallWebservice("JoinGame", JSON.stringify({ gameId: gameId, playerId: playerId }), SuccessCallback, ErrorCallback);
}

function GetCurrentTurn(gameId, SuccessCallback, ErrorCallback) {
    CallWebservice("GetCurrentTurn", JSON.stringify({gameId: gameId}), SuccessCallback, ErrorCallback);
}

function GetPossibleMoves(gameId, playerId, SuccessCallback, ErrorCallback) {
    CallWebservice("GetPossibleMoves", JSON.stringify({gameId: gameId, playerId: playerId}), SuccessCallback, ErrorCallback);
}

function ChooseTurn(gameId, playerId, choice, SuccessCallback, ErrorCallback) {
    CallWebservice("ChooseTurn", JSON.stringify({gameId: gameId, playerId: playerId, id: choice}), SuccessCallback, ErrorCallback);
}

function ChooseFortressedTurn(gameId, playerId, choice, SuccessCallback, ErrorCallback) {
    CallWebservice("ChooseFortressedTurn", JSON.stringify({gameId: gameId, playerId: playerId, id: choice}), SuccessCallback, ErrorCallback);
}

function RetrieveBoardData(gameId, SuccessCallback, ErrorCallback) {
    CallWebservice("RetrieveBoardData", JSON.stringify({gameId: gameId}), SuccessCallback, ErrorCallback);
}

function GetBoardState(gameId, SuccessCallback, ErrorCallback) {
    CallWebservice("GetBoardState", JSON.stringify({gameId: gameId}), SuccessCallback, ErrorCallback);
}

function GetGameStats(gameId, SuccessCallback, ErrorCallback) {
    CallWebservice("GetGameStats", JSON.stringify({ gameId: gameId }), SuccessCallback, ErrorCallback);
}

function WhatIsMyColor(gameId, playerId, SuccessCallback, ErrorCallback) {
    CallWebservice("WhatIsMyColor", JSON.stringify({gameId: gameId, playerId: playerId}), SuccessCallback, ErrorCallback);
}

function WhatIsMyStatus(gameId, playerId, SuccessCallback, ErrorCallback) {
    CallWebservice("WhatIsMyStatus", JSON.stringify({gameId: gameId, playerId: playerId}), SuccessCallback, ErrorCallback);
}

//  ====== Util functions ======

//  The callback functions should be defined as follows:
//  function SuccessCallback(obj)       -- obj:         deserialized object data from the response
//  function ErrorCallback(errorText)   -- errorText:   text indicating the error

function CallWebservice(webMethod, inputData, SuccessCallback, ErrorCallback) {
    $.ajax({
        url: gameEndpoint + webMethod,
        data: inputData,
        success: function (response, status, xhr) {
            SuccessCallback(response);
        },
        error: function (xhr, status, error) {
            ErrorCallback(status);
        }
    });
}


//  ====== AJAX setup ======
$.ajaxSetup({
    type        : "POST",
    contentType : "application/json; charset=utf-8",
    dataType    : "json",
    processData : true,
    success     : gioSuccessResponse,
    error       : gioErrorResponse,
    complete    : gioComplete
});

function gioSuccessResponse(response, status, xhr) {
    alert("uncaught success response");
}

function gioErrorResponse(xhr, status, error) {
    alert("uncaught error response");
}

function gioComplete(jqXHR, status) {
}
