//  constants
var gameLoopTimer = 500;

//  Game Logic functions

var playerId;
var gameId;
var gameLoop = null;
var board = null;
var fortressTurn = false;


function GenericIOError(status) {
    alert(status);
}


window.onload = function () {
    PrepareImages();
    RetrieveGameId();
};


// retrieve gameId
function RetrieveGameId() {
    GetGameId(function (obj) {
        this.gameId = obj.d;
        GetPlayerIds();
    });
}


//  retrieve all player id's and start game
function GetPlayerIds() {
    GetPlayerId(function (obj) {
        this.playerId = obj.d;
        StartNewGame(this.gameId, GameStarted, GenericIOError);
    }, GenericIOError);
}


//  game is started, join all players to the game and retrieve board data
function GameStarted(obj) {
    JoinGame(this.gameId, this.playerId, function (obj) {
       GetBoardData();
    }, GenericIOError);
}


//  retrieve board data
function GetBoardData() {
    RetrieveBoardData(this.gameId, CreateBoard, GenericIOError);
}


//  if board data retrieved, initialize user interface
function CreateBoard(obj) {
    board = new GameArea("GameArea");
    board.board.LoadBoardData(obj);

    WhatIsMyColor(this.gameId, this.playerId, function (obj) {
        board.mycolor.color = obj.d;
        InitGameLogic();
    }, GenericIOError);
}


function InitGameLogic() {
    board.fortress.clickCallback = function (obj) {
        fortressTurn = obj;
    }

    board.fortress.Disable();
    GameLoop();
}


var gameEnded = false;
function GameLoop() {

    if (!gameEnded) {
        UpdateStats();
        UpdateBoard();

        WhatIsMyStatus(this.gameId, this.playerId, function (obj) {
            IsItMyTurn(obj);
        }, function (obj) {
            // ignore IO error !
        });
    } else {
        clearTimeout(this.gameLoop);
    }
}


function UpdateStats() {
    GetGameStats(this.gameId, function (obj) {
        board.statistics.LoadStatistics(obj);
    }, GenericIOError);
}


function UpdateBoard() {
    GetBoardState(this.gameId, function (obj) {
        board.board.UpdateBoardData(obj);
    }, GenericIOError);
}


function IsItMyTurn(obj){
    switch (obj.d) {
        case 1:
            UpdateStats();
            UpdateBoard();
            InitializeTurn();
            break;
        case 3:
            board.messages.ShowMessage(3);

            //  rerun game loop
            gameLoop = setTimeout(GameLoop, this.gameLoopTimer);
            break;
        case 4:
            board.messages.ShowMessage(4);
            break;
        default:
            board.mycolor.Activate(false);
            //  rerun game loop
            gameLoop = setTimeout(GameLoop, this.gameLoopTimer);
            break;
    }
}


var initializedTurn = null;
function InitializeTurn() {
    board.mycolor.Activate(true);
    board.fortress.Enable();

    GetPossibleMoves(this.gameId, this.playerId, function (obj) {
        initializedTurn = obj;
        board.board.InitializePossibleMoves(obj, ChooseTurnUI);
    }, GenericIOError);
}


function ChooseTurnUI(id) {
    board.board.DeselectPossibleMoves(initializedTurn);

    if (fortressTurn == true) {
        ChooseFortressedTurn(gameId, playerId, id, function (obj) {
            UpdateBoard();
            gameLoop = setTimeout(GameLoop, this.gameLoopTimer);
        }, GenericIOError);
    } else {
        ChooseTurn(gameId, playerId, id, function (obj) {
            UpdateBoard();
            gameLoop = setTimeout(GameLoop, this.gameLoopTimer);
        }, GenericIOError);
    }
    board.fortress.Disable();
}
