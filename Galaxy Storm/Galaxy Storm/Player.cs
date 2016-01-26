//Author: David Minin   
//Project Name: Galaxy Storm
//File Name: Galaxy Storm
//Creation Date: January 31, 2013
//Modification Date: Febuary 20, 2013
//Description: Holds the properties used for entities.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Galaxy_Storm
{
    class Player : Entity
    {
        //Holds the texture of the player
        Texture2D image;

        //Used in determining how much time must pass for the bombs to be available again, in milleseconds
        int bombTimer = 0;
        public int BombTimer
        {
            get { return bombTimer; }
            set { bombTimer = value; }
        }

        public Player(Rectangle rect, Texture2D img, int l, int b)
        {
            rotation = 0f;
            boundingRect = rect;
            image = img;
            lives = l;
            bombs = b;
        }

        //Holds how many ai the player has killed consecutively
        int killStreak;
        public int KillStreak
        {
            get { return killStreak; }
            set { killStreak = value; }
        }

        //Holds the amount of life the player has remaining
        int lives;
        public int Lives
        {
            get { return lives; }
            set { lives = value; }
        }

        //Holds the amount of life the player has remaining
        int bombs;
        public int Bombs
        {
            get { return bombs; }
            set { bombs = value; }
        }

        //Getter and setter for Rotation
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        //Getter and setter for Velocity
        public float VelocityX
        {
            get { return velocity.X; }
            set { velocity.X = value; }
        }

        //Getter and setter for Velocity
        public float VelocityY
        {
            get { return velocity.Y; }
            set { velocity.Y = value; }
        }
    }
}
