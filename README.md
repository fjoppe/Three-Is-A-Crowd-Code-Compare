# Three-Is-A-Crowd-Code-Compare
Reversi for Three players, and a comparison between C# and F# code.

This Visual Studio 2015 Solution provides the Game Reversi, but now for three players.

Contents:
1. A level editor
2. GameEngine written in C#
3. GameEngine written in F#
4. A Game player in HTML5 and Javascript (works in Edge browser, does NOT work in Chrome, did not test others)

If you want to run this solution, you need to change the file paths in the web.config and the app.config. 
The paths must point to the location of the level-data, or indicate the leveldata file you want to use.
Currently these paths start with something like this: "F:\Hobby\Games\ThreeIsACrowd Code Compare\..."

Please also check whether the WCF endpoints are correct on your local machine.

To run the level editor, make project "LE.Application" the starting project.
To run the game itself, agains two AI players, make "GameEngine.Webserver" the starting project.
