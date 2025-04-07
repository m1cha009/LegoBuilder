This asset consists of 85 models and prefebs.

To use the Example scene:

I have included this example scene for you to play and learn.  Simply start the scene and click the yellow baseboard(large flat brick) to place bricks in a grid layout.  To change the color or brick, loot at the TapToPlace Gameobject in the Hierarchy.

The scene has a "Grid" and "TapToPlace" gameobjects with scripts of the same name attached.

The grid script simple adjusted the placement location of an object when it is called and is made to work with any script.  

The TapToPlace script has a gameobject and material that can be assigned. This script simply instantiates the selected gameobect at the clicked location, then calls the Grid method 'GetNearestPointOnGrid' to adjust the brick into the correct spot.

****Important**** There is a script called BrickStats on each of the prefabs.  This is important because it helps the TapToPlace class properly place each brick.  Since the bricks are not perfect square amounts, and some are taller or shorter, I needed a way to place them based on individual height.  I did this by setting the heigtht of each brick here then called it in TapToPlace. Please see the code of details.

Also there is a singleton class included, this is just there so we dont have more than one grid at any time.  You can learn more about singletons in the unity manual online.
