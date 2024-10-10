# Snake

This is a Unity Snake game which uses the A\* pathfinding algorithm to make it's way to the food.

The window size dictates the dimensions of the game as the border is set to the camera's size. This also means that the camera's size changes the size of the game grid!

Press `Space` to pause and resume the game or to start again after you lost.

To play manually, uncheck `pathfind` from the [GameMaster](Assets/Scripts/GameMaster.cs) script (attached to the camera).
You may also want to reduce the speed of the snake by increasing the `Fixed Timestep` to `0.1` or `0.2` in the project settings.

## Known bugs:

- The food spawns in the playable area regardless of the snake's position.
- When the pathfinding algorithm is not able to find a path to the food it just keeps going forward. (not a bug but a very dumb bot)

## To do:

- Fix the food spawning issue
- Add a grid system only currently used by the pathfinding algorithm
- Add a way to change the grid size by changing the camera's size
- When 
- Add a score system
- Add a game over screen
- Add a main menu
- Add other pathfinding algorithms
