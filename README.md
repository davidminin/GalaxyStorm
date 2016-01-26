# GalaxyStorm
A project I made by myself in my last year of highschool. I utilized C# and the XNA (3.2) framework to create a Space Neon Rhythm Shooter. Everything you see was created by me, however, some of the images contain textures that were found online. Since XNA has been discontinued I have attached image links to give a visual for how the game looks.

##Screenshots and Gifs of Gameplay:

Start Screen: https://gyazo.com/b47ba7244fcdf174c063000129f844c0

Song Select: https://gyazo.com/6e54c2cd72aa976d8336819f7f87caa1

Instructions: https://gyazo.com/fb4ab664ddbbc4f42e9b1622dce44265

Gameplay1: https://gyazo.com/e8f8f49f7f09b112cc82f1a673564d4f

Gameplay2: https://gyazo.com/ea0d4d8cb3a53eb4690613d5d2b8d39b

End Screen: https://gyazo.com/6ce4271d8a16d532d116f40a6170cb1e

##Event Manager:

An event manager was created to read and manage text files containing spawn queues. A simple C# windows application program was made to take in user input to a timer which would be synced to a song. This allows for the game to play to any song and allow the user to determine its difficulty. The event manager then takes this file and translates it into in game events.

##Ai and Swarms:

Intillectual Ai properties allow for easy to make algorithms and behavior. Algorithms are shared but individual properties can be customized to make a certain entity unique. An example would be the kind of movement the ai uses. Ai also are able to group and using smart positioning will join into swarms. These swarms then act as a collective entity and will use their combined mass and positioning to determine their movement. These swarms can take on any amount and allow for more fluid gameplay.

##QuadTree:

A quadtree was created to improve performance. Much like a binary tree, the quad tree has four different child nodes but this application has each node specifying a rectangular area. The screen gets continiously divided into smaller nodes up to 5 levels and only rectangles containing bullets will be checked for collision. The quad tree is dynamic and constantly gets updated. As entities are removed, added and moved, the amount of levels and sub trees constantly changes. The positioning of the areas always stays constant but the entities they can hold (player, bullet, ai) all can change. The quadtree is very important for performance tests. During stress test of the game on my home PC, it would lag around 200 ai without the tree but could handle about 1200 before lag with the tree.

##Particle System:

A particle system was created for dynamic creation and managment of particles. The particle limit never exceeds a constant max integer and particles are constantly being recycled to save memory. All particle animations are generated programatically (including their color) and the only thing they share is an image. When a particle dies it's index in the particle array gets saved. The next time a particle needs to be created it will access the particle at that index and manipulate the property values to "bring it back to life." Obviously only particles that are "alive" are being rendered on the screen.

##Entity Classes:

A general entity class was made for use by any entity in the game (Player, bullets, ai). The use of this class is to demonstrate polymorphism as well as to make the creation of unique entities simpler. Child classes all will have different implemntation of movement, drawing, etc. but will carry similar properties like position, collision detection, and bounding rectangles.

##Bloom / Neon effect:

A neon effect was desired for the game and so it was implemented. I used premade libraries from Microsoft which perform post process rendering on the screen. All the images you normally see are originally without any glow but these libraries give it that arcade/retro feel that the game has going for it.
