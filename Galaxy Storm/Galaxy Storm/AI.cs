using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Galaxy_Storm
{
    enum Type
    {
        Square,
        Circle,
        Triangle,
        Octagon
    }

    class AI : Entity
    {
        //Holds the length of each ai in pixels
        int LENGTH = 36;

        //Holds the Speed of the ai
        int speed;

        //Holds the source rectangle of the ai
        Rectangle srcRect;

        //Holds the value of which frame it is currently on from 0 to 5.
        int imgFrameNum;

        //Holds the maximum amount of frames in the sprite sheet
        int maxFrames;

        //Holds a count used to determine when to switch frames
        int count;

        //Holds the amount of frames needed to pass before the ai is spawned
        int spawnTime;

        //Holds if the ai is spawning or not
        bool isSpawning = true;
        public bool IsSpawning
        {
            get { return isSpawning; }
        }

        //Holds the score earned for killing the ai
        int score;
        public int Score
        {
            get { return score; }
        }

        //Returns velocity
        public Vector2 Velocity
        {
            get { return velocity; }
        }

        //Holds what swarm the ai is in, -1 means it is alone
        int swarmValue = -1;
        public int SwarmValue
        {
            get { return swarmValue; }
            set { swarmValue = value; }
        }

        //Holds the type of enemy the ai is
        Type type;
        public Type Type
        {
            get { return type; }
        }

        //Holds the health of the ai.
        int health;
        public int Health
        {
            get { return health; }
            set { health = value; }
        }

        //Returns the Speed of the ai
        public int Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        //Constructor
        public AI(Vector2 pos, Type t, Vector2 playerPos, int time)
        {
            count = 0;
            imgFrameNum = 0;
            rotation = (float)(Math.Atan2((playerPos.Y - BoundingRect.Center.Y), (playerPos.X - BoundingRect.Center.X)) + (Math.PI / 2));
            type = t;
            swarmValue = -1;
            maxFrames = 3;
            spawnTime = time;

            //Sets certain properties based on the type of ai
            switch (type)
            {
                case Type.Square:
                    health = 100;
                    Speed = 6;
                    velocity = new Vector2(Speed * (float)Math.Cos(rotation), Speed * (float)Math.Sin(rotation));
                    boundingRect = new Rectangle((int)pos.X, (int)pos.Y, 36, 36);
                    srcRect = new Rectangle(0, 72, 36, 36);
                    score = 50;
                    break;

                case Type.Circle:
                    health = 100;
                    Speed = 6;
                    velocity = new Vector2(Speed * (float)Math.Cos(rotation), Speed * (float)Math.Sin(rotation));
                    boundingRect = new Rectangle((int)pos.X, (int)pos.Y, 36, 36);
                    srcRect = new Rectangle(0, 0, 36, 36);
                    score = 25;
                    break;

                case Type.Triangle:
                    health = 100;
                    Speed = 7;
                    velocity = new Vector2(Speed * (float)Math.Cos(rotation), Speed * (float)Math.Sin(rotation));                                                                  
                    boundingRect = new Rectangle((int)pos.X, (int)pos.Y, 22, 22);
                    srcRect = new Rectangle(0, 108, 22, 22);
                    score = 50;
                    break;

                case Type.Octagon:
                    health = 100;
                    Speed = 8;
                    velocity = new Vector2(0,0);
                    boundingRect = new Rectangle((int)pos.X, (int)pos.Y, 36, 36);
                    srcRect = new Rectangle(0, 36, 36, 36);
                    score = 25;
                    break;
            }
        }

        //Checks for collision between another object at a given scale
        public bool CheckCollision(Rectangle obj, float scale , bool useCenter)
        {
            //Determine radius using a scale
            int radius1 = (int)((boundingRect.Height / 2) * scale);
            int radius2 = (int)((obj.Width / 2) * scale);

            //Declare variables
            int x1 = boundingRect.X;
            int y1 = boundingRect.Y;
            int x2 = obj.X;
            int y2 = obj.Y;

            //If the object is an ai, the redetermine center
            if (useCenter)
            {
                x2 = obj.Center.X;
                y2 = obj.Center.Y;
            }

            //Determine collision if hypotenuse of trig triangles is less than or equal to both radii added
            if (Math.Sqrt((x2 - x1) * (x2 - x1) + (y1 - y2) * (y1 - y2)) <= (radius1 + radius2))
            {
                 return true;
            }
            else
            {
                return false;
            }
        }

        //Updates the AI
        public void Update(Player p, Vector2 screenSize)
        {
            int frameChecker = 0;
            Vector2 vecD = new Vector2((p.BoundingRect.Center.X - BoundingRect.X), (p.BoundingRect.Center.Y - BoundingRect.Y));
            vecD.Normalize();
            rotation = (float)(Math.Atan2(vecD.Y, vecD.X) + Math.PI / 2);

            //If the ai is not spawning
            if (!isSpawning)
            {
                //Updates the ai based on their ai type
                switch (type)
                {
                    #region Square
                    case Type.Square:

                        //Update for a square
                        frameChecker = 2;

                        //Determines velocity
                        velocity.Normalize();
                        velocity += vecD;
                        velocity.Normalize();
                        velocity *= Speed;

                        boundingRect.X += (int)velocity.X;
                        boundingRect.Y += (int)velocity.Y;

                        break;
                    #endregion

                    #region Circle
                    case Type.Circle:

                        //Update for a circle
                        frameChecker = 4;

                        //Creates centripetal motion
                        velocity.Normalize();

                        Vector2 centripalVector = new Vector2(-velocity.Y, velocity.X) / 10;
                        velocity += centripalVector;
                        velocity.Normalize();
                        velocity *= speed;

                        //Adds the centripetal motion with the direction vector
                        boundingRect.X += (int)(velocity.X);
                        boundingRect.Y += (int)(velocity.Y);

                        break;
                    #endregion

                    #region Octagon
                    case Type.Octagon:

                        //Update for an octagon
                        frameChecker = 5;

                        if (velocity.X == 0 && velocity.Y == 0)
                        {
                            velocity = vecD * speed;
                        }

                        //Bounces the ai if they hit the side of the screen
                        if (BoundingRect.X + LENGTH > screenSize.X)
                        {
                            velocity = new Vector2(-velocity.X, velocity.Y);
                        }
                        else if (BoundingRect.X - LENGTH < 0)
                        {
                            velocity = new Vector2(-velocity.X, velocity.Y);
                        }
                        else if (BoundingRect.Y + LENGTH > screenSize.Y)
                        {
                            velocity = new Vector2(velocity.X, -velocity.Y);
                        }
                        else if (BoundingRect.Y - LENGTH < 100)
                        {
                            velocity = new Vector2(velocity.X, -velocity.Y);
                            boundingRect.Y += 2;
                        }

                        rotation = (float)(Math.Atan2(velocity.Y, velocity.X) + Math.PI / 2);

                        boundingRect.X += (int)velocity.X;
                        boundingRect.Y += (int)velocity.Y;

                        break;
                    #endregion

                    #region Triangle
                    case Type.Triangle:

                        frameChecker = 1;

                        //Determines velocity
                        velocity.Normalize();
                        velocity += vecD;
                        velocity.Normalize();
                        velocity *= Speed;

                        boundingRect.X += (int)velocity.X;
                        boundingRect.Y += (int)velocity.Y;

                        break;
                    #endregion
                }
            }
            //Else continue spawning
            else
            {
                switch (type)
                {
                    case Type.Square:
                        frameChecker = 10;
                        break;

                    case Type.Circle:
                        frameChecker = 10;
                        break;

                    case Type.Triangle:
                        frameChecker = 10;
                        break;

                    case Type.Octagon:
                        frameChecker = 5;
                        break;
                }

                if (count == spawnTime)
                {
                    FinishSpawning();
                }
            }

            Animate(frameChecker);
        }
        
        //Updates the AI
        public void Update(Vector2 velocity, float rot, int frameC)
        {
            rotation = rot;

            boundingRect.X += (int)velocity.X;
            boundingRect.Y += (int)velocity.Y;

            Animate(frameC);
        }

        //Called when the ai is done spawning
        public void FinishSpawning()
        {
            isSpawning = false;

            //Declares variables needed when spawning is complete
            switch (type)
            {
                case Type.Square:
                    srcRect = new Rectangle(0, 0, 36, 36);
                    maxFrames = 6;
                    break;

                case Type.Circle:
                    srcRect = new Rectangle(0, 0, 36, 36);
                    maxFrames = 4;
                    break;

                case Type.Triangle:
                    srcRect = new Rectangle(0, 0, 22, 22);
                    maxFrames = 1;
                    break;

                case Type.Octagon:
                    srcRect = new Rectangle(0, 0, 36, 36);
                    maxFrames = 5;
                    break;
            }
        }

        //Animates the ai
        private void Animate(int frameChecker)
        {
            //Updates the animation every couple updates
            ++count;
            if (count % frameChecker == 0)
            {
                //Increments the frame
                if (imgFrameNum != maxFrames - 1)
                {
                    imgFrameNum++;
                }
                else
                {
                    imgFrameNum = 0;
                }

                srcRect = new Rectangle(boundingRect.Width * imgFrameNum, srcRect.Y, boundingRect.Width, boundingRect.Height);
            }
        }

        //Draws the AI
        public void Draw(SpriteBatch sb, Texture2D spriteSheet)
        {
            sb.Draw(spriteSheet, boundingRect, srcRect, Color.White, rotation, new Vector2(boundingRect.Width / 2, boundingRect.Height / 2), SpriteEffects.None, 0);
        }
    }
}
