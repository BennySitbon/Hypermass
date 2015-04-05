Benny Sitbon
Team FluffySoft
HyperMass Game
3/20

/*This Github repository will only hold the scripts I have contributed to
* to see the full game please go to fluffysoft.wordpress.com
*/

HyperMass is a game created by a team of 16 programmers and artists for a 5 months Game Development Workshop.
The game is built in Unity3d 4.6 and C#.

In this game I have created most of the architectural design and implementation, and made the game ready for expansion with addition characters and items. I have made it so in order to create another character, all you need to do is extend the base class and implement some of the interface methods and thats it! Just put a some graphics on it and its ready to go!

The most interesting part in my mind is the instant replay system I have created for the game:
Using a circular buffer and a prototype pattern, my replay recorder constantly records the GameObject it is placed upon (can be extended to include more objects), the circular buffer size can be changed to create longer/shorter replays. Upon call, the instant replay can be played and the last 5 seconds of the game will be replayed for the viewer.

