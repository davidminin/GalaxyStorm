//Author: David Minin   
//Project Name: Galaxy Storm
//File Name: Galaxy Storm
//Creation Date: January 31, 2013
//Modification Date: Febuary 16, 2013
//Description: A 2d space shooter where the goal is to achieve the highest score possible.
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;
using BloomPostprocess;

namespace Galaxy_Storm
{

    public class MainGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState keyboardState;
        MouseState mouseState;
        SpriteFont font;
        SpriteFont fontSmall;

        //This thing is pretty random.
        Random random = new Random();

        //Constant valeus
        const int PARTICLE_LIMIT = 10000;

        //Holds the ratio for the monitor and the optimal size 
        Vector2 scale; 
        int optimalWidth = 1024;
        int optimalHeight = 768;

        //Used to determine which state the game is in. 0 = Main Menu, 1 = Options, 2 = Play, 3 = Lose Screen.
        int gameState = 0;

        //Holds player properties 
        Player player;
        float playerAcc = 1.2f;
        float speedCap = 8;

        //Holds how many frames have passed since the last shot fired
        int shootPeriod = 0;

        //Holds how many frames are needed to shoot the next bullet
        int shootSpeed = 6;

        //Holds the songs used
        Microsoft.Xna.Framework.Media.Song sngHeaven;

        //Holds the stop watch
        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();

        //Holds the textures used.
        Texture2D imgBackgroundOne;
        Texture2D imgBackgroundTwo;
        Texture2D imgQuadrant;
        Texture2D imgPlayer;
        Texture2D imgPBullet;
        Texture2D imgSpecialBar;
        Texture2D imgSpecialMeter;
        Texture2D imgParticle;
        Texture2D imgECircle;
        Texture2D imgESquare;
        Texture2D imgETriangle;
        Texture2D imgEOctagon;
        Texture2D imgMenuBar;
        Texture2D imgSpawnAnim;
        Texture2D imgPlayerLife;
        Texture2D imgBomb;
        Texture2D test;

        //Holds all the particles
        Color[,] particleColors = new Color[5, 3];

        //Holds the sounds used in the games
        SoundEffect seLaser;
        SoundEffect sePop;

        //Holds the location of the backgrounds and the players initial velocity used for when they move.
        Vector2 spaceLocOne;
        Vector2 spaceLocTwo;

        //Bloom
        BloomComponent bloom;

        //Lists and Managers
        QuadTree quadTree;
        Vector2[,] spawnPoints;
        AIManager aiManager = new AIManager();
        List<Event> events = new List<Event>();
        List<Bullet> bullets = new List<Bullet>();
        List<FloatingText> floatingTexts = new List<FloatingText>();
        ParticleManager particles = new ParticleManager(PARTICLE_LIMIT);
        int spawnPointsSizeX;
        int spawnPointsSizeY;

        //Variables used to calculate score
        int score = 0;
        int multiplier = 1;

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            IsMouseVisible = true;

            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            //Gets the user's screen width and height, also creates the game fullscreen.
            scale = new Vector2((float)GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / optimalWidth, (float)GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / optimalHeight);

            spaceLocOne = new Vector2(-810 * scale.X, -810 * scale.Y);
            spaceLocTwo = new Vector2(-810 * scale.X, -810 * scale.Y);

            quadTree = new QuadTree(1, new Point(0, 0), graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            bloom = new BloomComponent(this);
            Components.Add(bloom);
            bloom.Settings = new BloomSettings(null, 0.15f, 3, 1.20f, 1, 1.5f, 1);

            //Calculates the spawn point dimensions
            spawnPointsSizeX = (graphics.PreferredBackBufferWidth / 36) - 1;
            spawnPointsSizeY = (graphics.PreferredBackBufferHeight / 36) - 4;

            //Initializes the spawn points
            spawnPoints = new Vector2[spawnPointsSizeX, spawnPointsSizeY];
            for (int i = 0; i < spawnPointsSizeX; i++)
            {
                for (int j = 0; j < spawnPointsSizeY; j++)
                {
                    spawnPoints[i, j] = new Vector2(36 + i * 36, 136 + j * 36);
                }
            }
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            //Spritebatch and fonts
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Font");
            fontSmall = Content.Load<SpriteFont>("SmallFont");

            #region Media Loading

            //Backgrounds
            imgBackgroundOne = Content.Load<Texture2D>("Backgrounds/Space3");
            imgBackgroundTwo = Content.Load<Texture2D>("Backgrounds/background1");
            imgQuadrant = Content.Load<Texture2D>("Backgrounds/Quadrant");
            imgSpecialBar = Content.Load<Texture2D>("Backgrounds/Special Bar");
            imgSpecialMeter = Content.Load<Texture2D>("Backgrounds/Special Meter");
            imgMenuBar = Content.Load<Texture2D>("Backgrounds/Menu Bar");

            //Player Images
            imgPlayer = Content.Load<Texture2D>("Entities/Player");;
            imgECircle = Content.Load<Texture2D>("Entities/Enemy Blue Circle");
            imgESquare = Content.Load<Texture2D>("Entities/Enemy Pink Square");
            imgETriangle = Content.Load<Texture2D>("Entities/Enemy Arrow2");
            imgEOctagon = Content.Load<Texture2D>("Entities/Enemy Green Emerald2");
            test = Content.Load<Texture2D>("Entities/Collision Circle");

            //Effects
            imgParticle = Content.Load<Texture2D>("Effects/Particle");
            imgPBullet = Content.Load<Texture2D>("Effects/Bullet");
            imgBomb = Content.Load<Texture2D>("Effects/Bombs");
            imgPlayerLife = Content.Load<Texture2D>("Effects/Player Life");
            seLaser = Content.Load<SoundEffect>("Effects/Laser");
            sePop = Content.Load<SoundEffect>("Effects/Pop");
            imgSpawnAnim = Content.Load<Texture2D>("Effects/Spawn Animations");

            //Songs
            sngHeaven = Content.Load<Microsoft.Xna.Framework.Media.Song>("Songs/EnV - Heaven");

            //Particle Colors
            particleColors[0, 0] = new Color(136, 5, 168); // purple
            particleColors[0, 1] = new Color(189, 99, 212);
            particleColors[0, 2] = new Color(88, 2, 109);
            particleColors[1, 0] = new Color(8, 108, 162); //blue 
            particleColors[1, 1] = new Color(60, 157, 208);
            particleColors[1, 2] = new Color(3, 69, 105);
            particleColors[2, 0] = new Color(93, 255, 0); //green
            particleColors[2, 1] = new Color(163, 240, 108);
            particleColors[2, 2] = new Color(61, 146, 0);
            particleColors[3, 0] = new Color(255, 199, 0); // yellow 
            particleColors[3, 1] = new Color(255, 213, 64);
            particleColors[3, 2] = new Color(255, 224, 115);
            particleColors[4, 0] = new Color(255, 13, 0); //red
            particleColors[4, 1] = new Color(255, 122, 115);
            particleColors[4, 2] = new Color(166, 8, 0);

            #endregion

            player = new Player(new Rectangle(Convert.ToInt32(optimalWidth * scale.X / 2 - imgPlayer.Width * scale.X / 2), Convert.ToInt32(optimalHeight * scale.Y / 2 - imgPlayer.Height * scale.Y / 2), (int)(imgPlayer.Width * scale.X), 
                (int)(imgPlayer.Height * scale.Y)), imgPlayer, 3, 4);

            MediaPlayer.Volume = 0.6f;
        }

        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //Holds the connection between the keyboard + mouse and the game.
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            //If escape is pressed then the game will exit.
            if (keyboardState.IsKeyDown(Keys.Escape))
                this.Exit();

            //Play Mode
            if (gameState == 2)
            {
                #region Player Movement

                //Slows the player down if they are moving right.
                if (player.VelocityX > 0f)
                {
                    if (player.VelocityX > 0.5f)
                        player.VelocityX -= 0.2f;
                    else if (player.VelocityX > 0.11f)
                        player.VelocityX -= 0.1f;
                    else if (player.VelocityX > 0f)
                        player.VelocityX = 0f;
                }
                //Slows the player down if they are moving left.
                else if (player.VelocityX < 0f)
                {
                    if (player.VelocityX < -0.5f)
                        player.VelocityX += 0.2f;
                    else if (player.VelocityX < -0.11f)
                        player.VelocityX += 0.1f;
                    else if (player.VelocityX < 0f)
                        player.VelocityX = 0f;
                }
                //Slows the player down if they are moving up.
                if (player.VelocityY > 0f)
                {
                    if (player.VelocityY > 0.5f)
                        player.VelocityY -= 0.2f;
                    else if (player.VelocityY > 0.11f)
                        player.VelocityY -= 0.1f;
                    else if (player.VelocityY > 0f)
                        player.VelocityY = 0f;
                }
                //Slows the player down if they are moving down.
                else if (player.VelocityY < 0f)
                {
                    if (player.VelocityY < -0.5f)
                        player.VelocityY += 0.2f;
                    else if (player.VelocityY < -0.11f)
                        player.VelocityY += 0.1f;
                    else if (player.VelocityY < 0f)
                        player.VelocityY = 0f;
                }

                //Eliminates weird situations where the player practically stops moving and then shifts into a direction.
                if (keyboardState.IsKeyUp(Keys.D) && keyboardState.IsKeyUp(Keys.A) && keyboardState.IsKeyUp(Keys.S) && keyboardState.IsKeyUp(Keys.W))
                {
                    if (player.VelocityX == 0 && player.VelocityY <= 0.45f && player.VelocityY >= -0.45f)
                        player.VelocityY = 0f;
                    else if (player.VelocityY == 0 && player.VelocityX <= 0.45f && player.VelocityX >= -0.45f)
                        player.VelocityX = 0f;
                }

                //Player Movement: W = up, S = down, A = left, D = right. 
                if (keyboardState.IsKeyDown(Keys.D) && player.BoundingRect.Center.X + 52 + player.VelocityX + playerAcc < optimalWidth * scale.X && player.VelocityX <= speedCap)
                {
                    player.VelocityX += playerAcc;
                }
                if (keyboardState.IsKeyDown(Keys.A) && player.BoundingRect.Center.X - 52 + player.VelocityX - playerAcc > 0 && player.VelocityX >= -speedCap)
                {
                    player.VelocityX -= playerAcc;
                }
                if (keyboardState.IsKeyDown(Keys.W) && player.BoundingRect.Center.Y - 52 -(int)(100 * scale.Y) + player.VelocityY - playerAcc > 0 && player.VelocityY >= -speedCap)
                {
                    player.VelocityY -= playerAcc;
                }
                if (keyboardState.IsKeyDown(Keys.S) && player.BoundingRect.Center.Y + 52 + player.VelocityY + playerAcc < optimalHeight * scale.Y && player.VelocityY <= speedCap)
                {
                    player.VelocityY += playerAcc;
                }

                //Fixes velocity if player slightly exceeds speed cap.
                if (player.VelocityX > speedCap)
                    player.VelocityX = speedCap;
                else if (player.VelocityX < -speedCap)
                    player.VelocityX = -speedCap;

                if (player.VelocityY > speedCap)
                    player.VelocityY = speedCap;
                else if (player.VelocityY < -speedCap)
                    player.VelocityY = -speedCap;

                //Prevents the player from moving past the right side of the screen.
                if (player.BoundingRect.X + imgPlayer.Width + player.VelocityX >= optimalWidth * scale.X)
                    player.VelocityX = 0;

                //Prevents the player from moving past the left side of the screen.
                if (player.BoundingRect.X  + player.VelocityX <= 0)
                    player.VelocityX = 0;

                //Prevents the player from moving past the top side of the screen.
                if (player.BoundingRect.Y + player.VelocityY - (int)(100 * scale.Y) <= 0)
                    player.VelocityY = 0;

                //Prevents the player from moving past the bottom side of the screen.
                if (player.BoundingRect.Y + imgPlayer.Height + player.VelocityY >= optimalHeight * scale.Y)
                    player.VelocityY = 0;

                //Score calculator for distanced moved
                int distanceTraveled = (int)Math.Sqrt(player.VelocityX * player.VelocityX + player.VelocityY * player.VelocityY);
                score += distanceTraveled * multiplier;


                #endregion

                #region Player Shooting

                //Updates the bullets 
                for (int i = 0; i < bullets.Count; ++i)
                {
                    bullets[i].Update();
                    
                    if (bullets[i].IsOffScreen(optimalWidth, optimalHeight, scale))
                    {
                        bullets.RemoveAt(i);
                        i--;
                    }
                }

                //Shoot if the time period is up
                if (mouseState.LeftButton == ButtonState.Pressed && shootPeriod >= shootSpeed)
                {
                    //Resets shootPeriod and makes a laser shot sound.
                    shootPeriod = 0;
                    seLaser.Play(0.15f, 0.1f, 0f);

                    //Temporary variables used to declare the bullet
                    Rectangle boundingRect;
                    float rotation = player.Rotation - (float)(Math.PI / 2);
                    Vector2 vel = new Vector2((float)(32 * Math.Cos(rotation)), (float)(32 * Math.Sin(rotation)));

                    //Bullet One
                    if (player.Rotation <= Math.PI / 2)
                        boundingRect = new Rectangle(Convert.ToInt32(player.BoundingRect.Center.X + (15 * Math.Cos(player.Rotation) + player.VelocityX)), Convert.ToInt32(player.BoundingRect.Center.Y + (15 * Math.Sin(player.Rotation)) + player.VelocityY), imgPBullet.Width, 1);
                    else
                        boundingRect = new Rectangle(Convert.ToInt32(player.BoundingRect.Center.X - (20 * Math.Cos(player.Rotation) - player.VelocityX)), Convert.ToInt32(player.BoundingRect.Center.Y - (20 * Math.Sin(player.Rotation)) - player.VelocityY), imgPBullet.Width, 1);
                    Bullet bullet1 = new Bullet(vel, imgPBullet, rotation, boundingRect);
                    bullets.Add(bullet1);

                    //Bullet Two
                    if (player.Rotation <= Math.PI / 2)
                        boundingRect = new Rectangle(Convert.ToInt32(player.BoundingRect.Center.X - (20 * Math.Cos(player.Rotation) - player.VelocityX)), Convert.ToInt32(player.BoundingRect.Center.Y - (20 * Math.Sin(player.Rotation)) - player.VelocityY), imgPBullet.Width, 1);
                    else
                        boundingRect = new Rectangle(Convert.ToInt32(player.BoundingRect.Center.X + (15 * Math.Cos(player.Rotation) + player.VelocityX)), Convert.ToInt32(player.BoundingRect.Center.Y + (15 * Math.Sin(player.Rotation)) + player.VelocityY), imgPBullet.Width, 1);
                    Bullet bullet2 = new Bullet(vel, imgPBullet, rotation, boundingRect);
                    bullets.Add(bullet2);

                    //Power Up Bullet
                    //shootSpeed = 2;
                }

                //Increments the shootPeriod.
                shootPeriod++;

                //Activates the Nuke
                if (mouseState.RightButton == ButtonState.Pressed && player.Bombs > 0
                    && stopWatch.ElapsedMilliseconds - player.BombTimer >= 0)
                {
                    //Sets players bomb timer and removes a bomb
                    player.BombTimer = (int)(stopWatch.ElapsedMilliseconds + 1300);
                    player.Bombs--;

                    Nuke();
                }

                #endregion

                #region Special

                //Nothing here right now

                #endregion

                #region QuadTree

                quadTree.Clear();

                //Adds the ai
                List<AI> ai = aiManager.ReturnAI();
                foreach (AI a in ai)
                {
                    quadTree.Insert(a);
                }

                //Adds the bullets
                foreach (Bullet b in bullets)
                {
                    quadTree.Insert(b);
                }

                //Adds the player and initalizes the list that will be returned
                quadTree.Insert(player);
                List<Entity> collidableObjects = new List<Entity>();

                //Determines which objects the ai collided with
                for (int j = 0; j < ai.Count; j++)
                {
                    collidableObjects.Clear();
                    collidableObjects = quadTree.RetrieveList(collidableObjects, ai[j], true, quadTree);
                    collidableObjects.Remove(ai[j]);

                    //Collision work
                    for (int k = 0; k < collidableObjects.Count; k++)
                    {
                        //Collision with AI and Bullet
                        if (collidableObjects[k] is Bullet && ai[j].CheckCollision(collidableObjects[k].BoundingRect, 1.6f, false))
                        {
                            player.KillStreak += 1;

                            //Multiplier calculater and draws the floating text
                            if (multiplier < 9 && player.KillStreak / (multiplier * multiplier) > 10)
                            {
                                multiplier++;
                                floatingTexts.Add(new FloatingText("Multiplier " + multiplier + "x", 80, new Vector2(ai[j].BoundingRect.X, ai[j].BoundingRect.Y), particleColors[3, 1]));
                            }
                            else
                            {
                                floatingTexts.Add(new FloatingText("" + ai[j].Score * multiplier, 45, new Vector2(ai[j].BoundingRect.Center.X, ai[j].BoundingRect.Y), particleColors[0, 1]));
                            }

                            //Updates score and removes bullet
                            score += ai[j].Score * multiplier;
                            bullets.Remove(collidableObjects[k] as Bullet);

                            //Effects
                            //sePop.Play(0.7f, 0.5f, 0f);
                            ExplosionEffect(120, random.Next(0, 5), new Vector2(ai[j].BoundingRect.X, ai[j].BoundingRect.Y), 40, 55);
                            
                            //Removes ai
                            aiManager.RemoveAI(ai[j]);
                            ai.RemoveAt(j);
                            j--;
                            
                            break;
                        }

                        //Potential collision between two AI
                        else if (ai[j].Type != Type.Octagon && collidableObjects[k] is AI && !(collidableObjects[k] as AI).IsSpawning
                            && !ai[j].IsSpawning && (collidableObjects[k] as AI).Type == ai[j].Type)
                        {
                            //Lone ai collides with a swarm
                            if (aiManager.SwarmCount > 0 && ai[j].SwarmValue == -1 && (collidableObjects[k] as AI).SwarmValue != -1 &&
                                ai[j].CheckCollision(collidableObjects[k].BoundingRect, 0.9f, false))
                            {
                                aiManager.AddToSwarm(ai[j], (collidableObjects[k] as AI).SwarmValue);
                                ai[j].SwarmValue = (collidableObjects[k] as AI).SwarmValue;
                            }
                            //Lone ai collides with another lone ai
                            else if (ai[j].SwarmValue == -1 && (collidableObjects[k] as AI).SwarmValue == -1 &&
                                ai[j].CheckCollision(collidableObjects[k].BoundingRect, 0.9f, false))
                            {
                                aiManager.CreateSwarm(ai[j], (collidableObjects[k] as AI));
                                ai[j].SwarmValue = aiManager.SwarmCount - 1;
                                (collidableObjects[k] as AI).SwarmValue = aiManager.SwarmCount - 1;
                                
                            }
                            //Collision with two AI that are both in different swarms
                            else if (ai[j].SwarmValue != -1 && (collidableObjects[k] as AI).SwarmValue != -1 && ai[j].SwarmValue != (collidableObjects[k] as AI).SwarmValue &&
                                    ai[j].CheckCollision(collidableObjects[k].BoundingRect, 0.9f , false))
                            {
                                aiManager.JoinSwarms(ai[j].SwarmValue, (collidableObjects[k] as AI).SwarmValue);
                            }
                        }

                        //Collision between player and ai
                        else if (collidableObjects[k] is Player && !ai[j].IsSpawning && ai[j].CheckCollision(collidableObjects[k].BoundingRect, 1f, true))
                        {
                            player.KillStreak = 0;
                            player.Lives--;
                            Nuke();
                        }
                    }
                }

                #endregion

                #region Update Variables

                //Updates bounding rectangles as player moves.
                player.BoundingRect = new Rectangle(Convert.ToInt32(player.BoundingRect.X + player.VelocityX), Convert.ToInt32(player.BoundingRect.Y + player.VelocityY), imgPlayer.Width, imgPlayer.Height);
                spaceLocTwo = new Vector2(spaceLocTwo.X - player.VelocityX, spaceLocTwo.Y - player.VelocityY);
                spaceLocOne = new Vector2(spaceLocOne.X - player.VelocityX, spaceLocOne.Y - player.VelocityY);

                //Calculates player rotation.
                player.Rotation = (float)(Math.Atan2((mouseState.Y - player.BoundingRect.Center.Y), (mouseState.X - player.BoundingRect.Center.X)) + MathHelper.ToRadians(90));
                
                //Event Handler
                UpdateEventHandler((int)stopWatch.ElapsedMilliseconds);
                
                #endregion 

                //Testing stuff
                if (keyboardState.IsKeyDown(Keys.Space))
                {                                                   
                    AddAI(1);
                }

                if (keyboardState.IsKeyDown(Keys.Up) && multiplier < 9)
                {
                    multiplier++;
                }
                if(keyboardState.IsKeyDown(Keys.Down))
                {
                    multiplier = 1;
                }


            }

            //Main Menu
            else if (gameState == 0)
            {
                ReadSong(0);
                MediaPlayer.Play(sngHeaven);
                stopWatch.Start();
                gameState = 2;
            }

            //Options 
            else if (gameState == 1)
            {

            }

            //Pause Screen
            else if (gameState == 3)
            {

            }

            //End Screen
            else if (gameState == 4)
            {

            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            //Play Mode
            if (gameState == 2)
            {
                //Draws the background
                //spriteBatch.Draw(imgBackgroundTwo, new Rectangle(Convert.ToInt32(spaceLocOne.X * 0.1), Convert.ToInt32(spaceLocOne.Y * 0.1), Convert.ToInt32(optimalWidth * 1.4 * scale.X),
                    //Convert.ToInt32(optimalHeight * 1.4 * scale.Y)), null, Color.White);

                spriteBatch.Draw(imgBackgroundOne, new Rectangle(Convert.ToInt32(spaceLocOne.X * 0.2), Convert.ToInt32(spaceLocOne.Y * 0.2), Convert.ToInt32(optimalWidth * 1.4 * scale.X),
                    Convert.ToInt32(optimalHeight * 1.4 * scale.Y)), null, Color.White);

                //Updates and draws the particles
                if (particles.particleManager.AnyActiveParticles())
                {
                    particles.particleManager.UpdateParticles(new Vector2(optimalWidth * scale.X, optimalHeight * scale.Y));
                    particles.particleManager.DrawParticles(spriteBatch);
                }

                //Draws the bullets
                foreach (Bullet b in bullets)
                {
                    b.Draw(spriteBatch);
                }

                //Updates AI
                aiManager.Update(player, new Vector2(optimalWidth * scale.X, optimalHeight * scale.Y));
                List<AI> ai = aiManager.ReturnAI();

                //Draws AI
                foreach (AI a in ai)
                {
                    //If the ai is spawning
                    if (a.IsSpawning)
                    {
                        a.Draw(spriteBatch, imgSpawnAnim);
                    }
                    //Else draw the ai normally
                    else
                    {
                        switch (a.Type)
                        {
                            case Type.Square:
                                a.Draw(spriteBatch, imgESquare);
                                break;

                            case Type.Circle:
                                a.Draw(spriteBatch, imgECircle);
                                break;

                            case Type.Triangle:
                                a.Draw(spriteBatch, imgETriangle);
                                break;

                            case Type.Octagon:
                                a.Draw(spriteBatch, imgEOctagon);
                                break;
                        }
                    }
                }

                //Draws floating texts
                for (int i = 0; i < floatingTexts.Count; i++)
                {
                    floatingTexts[i].Update();

                    //Draws the text if it is still alive
                    if (floatingTexts[i].Life != 0)
                    {
                        floatingTexts[i].Draw(spriteBatch, fontSmall);
                    }
                    //Otherwise remove it
                    else
                    {
                        floatingTexts.RemoveAt(i);
                        i--;
                    }
                }

                //Draws the player
                spriteBatch.Draw(imgPlayer, new Vector2(player.BoundingRect.Center.X, player.BoundingRect.Center.Y), new Rectangle(0, 0, imgPlayer.Width , imgPlayer.Height), 
                    Color.White, player.Rotation, new Vector2((int)player.BoundingRect.Width / 2, (int)player.BoundingRect.Height / 2), new Vector2(1, 1), SpriteEffects.None, 0);

                //Draws HUD
                spriteBatch.Draw(imgMenuBar, new Rectangle(0,0, (int)(optimalWidth * scale.X),100), Color.White); 
                spriteBatch.DrawString(font, "Score", new Vector2(20, 20), Color.White);
                spriteBatch.DrawString(font, PrintScore(Convert.ToString(score)), new Vector2(20, 60), particleColors[0,1]);
                spriteBatch.DrawString(font, "Multiplier", new Vector2((int)(optimalWidth * scale.X) - 220, 20), Color.White);
                DrawPlayerAssets(spriteBatch);

                //Alligns the font if the multiplier is equal to one
                if (multiplier == 1)
                {
                    spriteBatch.DrawString(font, multiplier + "x", new Vector2((int)(optimalWidth * scale.X) - 48, 60), particleColors[0, 1]);
                }
                else if (multiplier < 5)
                {
                    spriteBatch.DrawString(font, multiplier + "x", new Vector2((int)(optimalWidth * scale.X) - 67, 60), particleColors[2, 1]);
                }
                else if (multiplier < 9)
                {
                    spriteBatch.DrawString(font, multiplier + "x", new Vector2((int)(optimalWidth * scale.X) - 67, 60), particleColors[4, 1]);
                }
                else
                {
                    spriteBatch.DrawString(font, multiplier + "x", new Vector2((int)(optimalWidth * scale.X) - 67, 60), particleColors[3, 1]);
                }
            }

            //quadTree.DrawQuadrants(spriteBatch, imgQuadrant);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        //Draws the amount of lives and bombs the player has
        public void DrawPlayerAssets(SpriteBatch sb)
        {
            //Determines start point
            int startPointX = (int)(optimalWidth * scale.X / 2) - (int)(imgPlayerLife.Width * player.Lives / 2) - (5 * player.Lives);
            if (player.Lives == 1)
            {
                startPointX -= 5;
            }
            else if (player.Lives == 2)
            {
                startPointX -= 2;
            }

            //Draws the lives
            for (int i = 0; i < player.Lives; i++)
            {
                sb.Draw(imgPlayerLife, new Rectangle(startPointX + (int)(i * imgPlayerLife.Width) + (5 * i), 15, imgPlayerLife.Width, imgPlayerLife.Height), Color.White);
            }

            //Determines start point
            startPointX = (int)(optimalWidth * scale.X / 2) - (int)(imgBomb.Width * player.Bombs / 2) - (5 * player.Bombs);
            if (player.Bombs == 1)
            {
                startPointX -= 5;
            }
            else if (player.Bombs == 2)
            {
                startPointX -= 2; 
            }
            else if (player.Bombs == 4)
            {
                startPointX += 2;
            }
            
            //Draws the bombs
            for (int j = 0; j < player.Bombs; j++)
            {
                //Can use bombs
                if (stopWatch.ElapsedMilliseconds - player.BombTimer >= 0)
                {
                    sb.Draw(imgBomb, new Rectangle(startPointX + (int)(j * imgBomb.Width) + (5 * j), 60, imgBomb.Width, imgBomb.Height), Color.White);
                }
                //Cannot use bombs
                else
                {
                    sb.Draw(imgBomb, new Rectangle(startPointX + (int)(j * imgBomb.Width) + (5 * j), 60, imgBomb.Width, imgBomb.Height), Color.DimGray);
                }
            }
        }

        //Takes an integer as a string and adds apostrophes were needed
        public string PrintScore(string num)
        {
            string output = "";

            for (int i = 0; i < num.Length; i++)
            {
                if (i != 0 && (num.Length - i) % 3 == 0)
                {
                    output += ",";
                }
                output += num[i];
            }

            return output;
        }

        //Creates an explosion effect, overload allows for munipulation of the speed and life of the particles
        public void ExplosionEffect(int amount, int colorInt, Vector2 location, int lowspeed, int life)
        {
            if (amount > 200)
            {
                amount = 200;
            }

            for (int i = 0; i < amount; ++i)
            {
                if (particles.particleManager.AnyInactiveParticles())
                {
                    Vector2 direction = new Vector2(random.Next(-100, 100), random.Next(-100, 100));
                    direction.Normalize();
                    particles.particleManager.CreateParticle(direction * (float)Math.Pow(random.Next(lowspeed, lowspeed + 50), 2) / 1000, location, imgParticle, life, particleColors[colorInt, random.Next(0, 3)], 0f);
                }
                else
                {
                    break;
                }
            }
        }

        //Kills every Ai in the game
        public void Nuke()
        {
            //Decreaes multiplier by 3 times
            multiplier -= 4;
            player.KillStreak = 11 * multiplier * multiplier;
            multiplier++;

            //Fixes the multiplier if it is negative
            if (multiplier < 1)
            {
                multiplier = 1;
                player.KillStreak = 0;
            }

            //Variables
            List<AI> allAi = aiManager.ReturnAI();
            int availableParticles = particles.particleManager.ReturnInactiveParticles();
            int colorOne = random.Next(0, 5);
            int colorTwo = random.Next(0, 5);

            //Explodes every ai
            for(int i = 0; i < allAi.Count; i++)
            {
                score += allAi[i].Score * multiplier;

                if ((i + 2) % 2 == 0)
                {
                    ExplosionEffect(availableParticles / allAi.Count, colorOne, new Vector2(allAi[i].BoundingRect.X, allAi[i].BoundingRect.Y), 55, 75);
                }
                else
                {
                    ExplosionEffect(availableParticles / allAi.Count, colorTwo, new Vector2(allAi[i].BoundingRect.X, allAi[i].BoundingRect.Y), 55, 75);
                }
            }

            aiManager = new AIManager();
        }

        //Adds a specefic amount of a random AI
        public void AddAI(int amount)
        {
            Type t;
            for (int i = 0; i < amount; ++i)
            {
                int temp = random.Next(0, 4);
                switch (temp)
                {
                    case 0:
                        t = Type.Square;
                        break;

                    case 1:
                        t = Type.Triangle;
                        break;

                    case 2:
                        t = Type.Octagon;
                        break;

                    default:
                        t = Type.Circle;
                        break;
                }
                //t = Type.Square;

                if (t != Type.Circle)
                {
                    aiManager.AddLoneAI(new AI(spawnPoints[random.Next(0, spawnPointsSizeX), random.Next(0, spawnPointsSizeY)], t,
                    new Vector2(player.BoundingRect.X, player.BoundingRect.Y), 60));
                }
                else
                {
                    aiManager.AddLoneAI(new AI(spawnPoints[random.Next(3, spawnPointsSizeX - 1), random.Next(3, spawnPointsSizeY - 1)], t,
                    new Vector2(player.BoundingRect.X, player.BoundingRect.Y), 60));
                }
            }
        }

        //Reads in the song from the debug folder and updates the song variable
        public void ReadSong(int userChoice)
        {
            StreamReader streamReader = new StreamReader("Heaven Rd. 2.txt");

            //Determines what song to play based on the users input
            switch (userChoice)
            {
                case 0:
                    streamReader = new StreamReader("Heaven Rd. 2.txt");
                    break;

                case 1:

                    break;

                case 2:

                    break;
            }

            string line = streamReader.ReadLine();
            int time;
            events.Clear();

            //Only loops if the line read isn't empty
            while (line != "")
            {
                //Determines the variables of the event
                time = Convert.ToInt32(line);
                line = streamReader.ReadLine();

                //Adds the event
                events.Add(new Event(time, line));

                //Reads the next line to determine if the loop should go on
                line = streamReader.ReadLine();
            }

            //File is done being read and all the events have been saved
            streamReader.Close();
        }

        //Updates the event handler
        public void UpdateEventHandler(int songTimePassedMS)
        {
            //Only attempt to play events if there is more than one event remaining
            if (events.Count > 0)
            {
                //If the event time is hit then play the event
                if (songTimePassedMS - events[0].Time >= 0)
                {
                    switch (events[0].Action)
                    {
                        case "Spawn":
                            AddAI(multiplier);
                            break;

                        case "Special":
                            break;
                    }

                    events.RemoveAt(0);
                }
            }
        }
    }
}
