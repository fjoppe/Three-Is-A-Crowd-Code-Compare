//  Game User Interface helper functions

//  ===========================================================================
//  Constants
//  ===========================================================================

//  game area constants
var zoomFactor = 0.7;
var canvasGeometry = 450;
var origin = canvasGeometry / 2;

//  tile constants
var ftXoffset = 14;
var ftYoffset = 18;
var hexagonWmid = 60 / 2;
var hexagonHmid = 52 / 2;

var txXoffset = 26;
var txYoffset = 18;
var txWidth = 9;
var txHeight = 20;
var fontSize = 11;

//  ===========================================================================
//  Classes
//  ===========================================================================


//  class: GameArea
function GameArea(containerName) {
    this.stage = new Kinetic.Stage({ container: containerName, width: canvasGeometry, height: canvasGeometry});
    this.background = new Background();
    this.board = new Board();
    this.fortress = new FortressDisplay(3, 405);
    this.statistics = new Statistics(3,3);
    this.mycolor = new MyColor(380, 12);
    this.messages = new GameMessage();

    this.stage.add(this.background.layer);
    this.stage.add(this.board.layer);
    this.stage.add(this.fortress.layer);
    this.stage.add(this.statistics.layer);
    this.stage.add(this.mycolor.layer);
    this.stage.add(this.messages.layer);
}


//  class: Background
function Background(){
    this.layer = new Kinetic.Layer();

    var rectangle = new Kinetic.Rect({ width: canvasGeometry, height: canvasGeometry, fill: "#E5F2EC", stroke: "black" });
    this.layer.add(rectangle);
}


//  class: Board
function Board() {
    this.layer = new Kinetic.Layer({ x: origin - hexagonWmid, y: origin-hexagonHmid });
    this.layer.setScale(zoomFactor, zoomFactor);
    this.board = new Array();
    this.mycolor = null;
}

Board.prototype.LoadBoardData = function (obj) {
    for (var i = 0; i < obj.d.ActiveTileList.length; i++) {
        var tile = obj.d.ActiveTileList[i];

        this.board[tile.Id] = new BoardTile();
        this.board[tile.Id].Load(tile);
        this.layer.add(this.board[tile.Id].group);
    }

    this.layer.draw();
}


Board.prototype.UpdateBoardData = function (obj) {
    for (var i = 0; i < obj.d.length; i++) {
        var tile = obj.d[i];
        if (!this.board[tile.id].selected) {
            this.board[tile.id].SelectFortress(tile.Fortress);
            this.board[tile.id].SetTileColor(tile.color);
        }
    }
    this.layer.draw();
}

Board.prototype.InitializePossibleMoves = function (obj, ClickEvent) {
    for (var i = 0; i < obj.d.length; i++) {
        var tileId = obj.d[i];
        this.board[tileId].SelectTile(true, ClickEvent);
    }
    this.layer.draw();
}

Board.prototype.DeselectPossibleMoves = function (obj) {
    for (var i = 0; i < obj.d.length; i++) {
        var tileId = obj.d[i];
        this.board[tileId].SelectTile(false, null);
    }
    this.layer.draw();
}


//  class: BoardTile
function BoardTile() {
    this.group = new Kinetic.Group({});
    this.boardImage = null;
    this.valueImage = null;
    this.fortressImage = null;
    this.type = 0;
    this.fortress = false;
    this.value = 0;
    this.selected = false;

    this.boardImage = new Kinetic.Image({});

    this.boardImage.source = this;
    this.clickEvent = null;

    this.valueImage = new Kinetic.Text({x: txXoffset, y: txYoffset, /*width: txWidth, heigth: txHeight,*/ text: "", fontSize: fontSize, fontFamily: 'Arial', textFill: 'black'});
    this.fortressImage = new Kinetic.Image({image: imageObjs["fortress"], x: ftXoffset, y: ftYoffset });

    this.group.add(this.boardImage);
    this.group.add(this.valueImage);
    this.group.add(this.fortressImage);
}

BoardTile.prototype.Load = function (tile) {
    this.group = new Kinetic.Group({ x: tile.X, y: tile.Y });
    this.boardImage = new Kinetic.Image({ image: imageObjs["hexagon"], name: tile.Id });
    this.boardImage.source = this;
    this.valueImage = new Kinetic.Text({ x: txXoffset, y: txYoffset, /*width: txWidth, heigth: txHeight,*/ text: tile.TileValue, fontSize: fontSize, fontFamily: 'Arial', textFill: 'black' });
    this.valueImage.source = this;
    this.fortressImage = new Kinetic.Image({ image: imageObjs["fortress"], x: ftXoffset, y: ftYoffset });
    this.id = tile.Id;
    this.type = tile.TileType;
    this.fortress = tile.Fortress;
    this.value = tile.TileValue;

    this.group.add(this.boardImage);
    this.group.add(this.valueImage);
    this.group.add(this.fortressImage);

    this.SetTileColor(tile.TileType);
}

BoardTile.prototype.SetTileColor = function (type) {
    this.boardImage.setOpacity(1.0);
    this.boardImage.setVisible(true);

    switch (type) {
        case 0: // none
            this.boardImage.setVisible(false);
            break;
        case 1: // board
            this.boardImage.setImage(imageObjs["hexagon"]);
            break;
        case 2: // blue
            this.boardImage.setImage(imageObjs["blue"]);
            break;
        case 3: // red
            this.boardImage.setImage(imageObjs["red"]);
            break;
        case 4: // yellow
            this.boardImage.setImage(imageObjs["yellow"]);
            break;
        case 5: // is selected
            this.boardImage.setImage(imageObjs["red"]);
            this.boardImage.setOpacity(0.5);
            break;
        default:
            throw "Unknow tile type";
    }

    this.fortressImage.setVisible(this.fortress);
}

BoardTile.prototype.SelectTile = function (isSelected, clickEvent) {
    this.selected = isSelected;
    if (this.selected) {
        this.SetTileColor(5);
//        this.boardImage.moveToTop();
//        this.valueImage.moveToTop();
        this.boardImage.on("click", function () { this.source.ClickEvent(); });
        this.valueImage.on("click", function () { this.source.ClickEvent(); });
        this.clickEvent = clickEvent;
    } else {
        this.SetTileColor(this.type);
    }
}


BoardTile.prototype.SelectTileSimple = function (isSelected) {
    this.selected = isSelected;
    if (this.selected) {
        this.SetTileColor(5);
    } else {
        this.SetTileColor(this.type);
    }
}

BoardTile.prototype.SelectFortress = function(isSelected) {
    this.fortress = isSelected;
    this.fortressImage.setVisible(this.fortress);
}

BoardTile.prototype.SetText = function(text) {
    this.valueImage.setText(text);
}

BoardTile.prototype.ClickEvent = function (id) {
    this.boardImage.off("click");
    this.valueImage.off("click");
    if (this.clickEvent != null) {
        this.clickEvent(this.id);
    }
    this.clickEvent = null;
}


//  class: ColorStatistic
function ColorStatistic(color, x,y) {
    this.group = new Kinetic.Group({ x: x, y: y });
    this.tile = new BoardTile();
    this.text = new Kinetic.Text({ x: 100, y: 10, text: "0", fontSize: 30, fontFamily: 'Arial', textFill: 'black' });

    this.fortress = new BoardTile();
    this.fortress.group.setAbsolutePosition({ x: 150, y: 0 });
    this.fortressText = new Kinetic.Text({ x: 250, y: 10, text: "0", fontSize: 30, fontFamily: 'Arial', textFill: 'black' });

    this.tile.SetTileColor(color);

    this.fortress.SetTileColor(1);
    this.fortress.SelectFortress(true);

    this.group.add(this.tile.group);
    this.group.add(this.text);
    this.group.add(this.fortress.group);
    this.group.add(this.fortressText);
}

ColorStatistic.prototype.SetCountValue = function (tileCount, fortressCount) {
    this.text.setText(tileCount);
    this.fortressText.setText(fortressCount);
}


//  class: Statistics
function Statistics(x,y) {
    this.layer = new Kinetic.Layer({x:x, y:y});
    this.layer.setScale(0.4, 0.4);
    this.yellow = new ColorStatistic(4, 0, 0);
    this.red = new ColorStatistic(2, 0, 60);
    this.blue = new ColorStatistic(3, 0, 120);

    this.layer.add(this.yellow.group);
    this.layer.add(this.blue.group);
    this.layer.add(this.red.group);
}

Statistics.prototype.LoadStatistics = function (obj) {
    this.yellow.SetCountValue(obj.d.YellowCount, obj.d.YellowFortress);
    this.red.SetCountValue(obj.d.RedCount, obj.d.RedFortress);
    this.blue.SetCountValue(obj.d.BlueCount, obj.d.BlueFortress);
    this.layer.draw();
}


//  class: FortressDisplay
function FortressDisplay(x, y) {
    this.layer = new Kinetic.Layer({ x: x, y: y });
    this.layer.setScale(0.8, 0.8);
    this.tile = new BoardTile();
    this.tile.type = 1; // board tile
    this.text = new Kinetic.Text({ x: 70, y: 15, text: "0", width: 20, heigth: 29, fontSize: 20, fontFamily: 'Arial', textFill: 'black' });
    this.canClick = true;
    this.clickCallback = null;
    this.selected = false;

    this.tile.SelectFortress(true);
    this.tile.SetTileColor(1);        //  none

    this.layer.add(this.tile.group);
    this.layer.add(this.text);

    this.clickEvent = null;

    this.tile.boardImage.source = this;
    this.tile.fortressImage.source = this;


    this.AddClick = function (click) {
        this.clickEvent = click;
    }

    this.tile.boardImage.on("click", function () {
        this.source.Select();
    });

    this.tile.fortressImage.on("click", function () {
        this.source.Select();
    });
}

FortressDisplay.prototype.Select = function (evt) {
    if (this.canClick) {
        if (this.tile.selected) {
            this.tile.SelectTileSimple(false);
            this.selected = false;
        } else {
            this.tile.SelectTileSimple(true);
            this.selected = true;
        }
    } else {
        this.tile.SelectTileSimple(false);
        this.selected = false;
    }

    if (this.clickCallback != null) {
        this.clickCallback(this.selected);
    }

    this.layer.draw();
}


FortressDisplay.prototype.Enable = function () {
    this.canClick = true;
}


FortressDisplay.prototype.Disable = function () {
    this.canClick = false;
    this.Select(false);
}


//  class: MyColor
function MyColor(x,y) {
    this.layer = new Kinetic.Layer({x: x, y: y});
    this.tile = new BoardTile();
    this.color = 0;

    this.tile.SetTileColor(1);
    this.tile.SelectFortress(false);
    this.tile.SetText("");

    this.layer.add(this.tile.group);
}

MyColor.prototype.Activate= function(isActive){
    if(isActive){
        this.tile.SetTileColor(this.color);
    }else{
        this.tile.SetTileColor(1);  // none
    }
    this.layer.draw();
}


//  class: GameMessage
function GameMessage() {
    this.layer = new Kinetic.Layer();
    this.NoMovesMessage = new Kinetic.Text({x: 173, y: 372, text: "No Moves", fontSize: 28, fontFamily: 'Arial', textFill: 'black', visible: false});
    this.GameOverMessage = new Kinetic.Text({ x: 173, y: 379, text: "Game Over", fontSize: 28, fontFamily: 'Arial', textFill: 'black', visible: false });

    this.layer.add(this.NoMovesMessage);
    this.layer.add(this.GameOverMessage);
}

GameMessage.prototype.ShowMessage = function (messageId) {
    switch (messageId) {
        case 3:
            this.NoMovesMessage.setVisible(true);
            setTimeout(this.Clear, 3000);
            break;
        case 4:
            this.GameOverMessage.setVisible(true);
            break;
    }
    this.layer.draw();
}

GameMessage.prototype.Clear = function () {
    this.board.messages.NoMovesMessage.setVisible(false);
    this.board.messages.layer.draw();
}


//  ===========================================================================
//  Preload images
//  ===========================================================================
var gameImages = new Array();
var imageObjs   = new Array();

function PrepareImages() {
    imageObjs["blue"] = new Image(); imageObjs["blue"].src = "Assets/BlueHexagon.png";
    imageObjs["fortress"] = new Image(); imageObjs["fortress"].src = "Assets/Fortress.png";
    imageObjs["red"] = new Image(); imageObjs["red"].src = "Assets/RedHexagon.png";
    imageObjs["yellow"] = new Image(); imageObjs["yellow"].src = "Assets/YellowHexagon.png";
    imageObjs["hexagon"] = new Image(); imageObjs["hexagon"].src = "Assets/Hexagon.png";
}
