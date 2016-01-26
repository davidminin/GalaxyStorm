//Author: David Minin   
//Project Name: Galaxy Storm
//File Name: Galaxy Storm
//Creation Date: January 31, 2013
//Modification Date: Febuary 16, 2013
//Description: Holds the properties and subprograms used for bullets.
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Galaxy_Storm
{
    class Bullet : Entity
    {

        //Holds the image of the bullet.
        Texture2D texture;

        //Gets the velocity of the bullet
        public Vector2 GetVelocity
        {
            get { return velocity; }
        }

        //Gets the velocity of the bullet
        public float GetRotation
        {
            get { return rotation; }
        }

        public Bullet(Vector2 vel, Texture2D txt, float rot, Rectangle bRect)
        {
            velocity = vel;
            texture = txt;
            rotation = rot;
            boundingRect = bRect;
        }

        //Pre: A spritebatch, bullets image, bullets velocity, bullets bounding rectangle, bullets rotaion.
        //Post: Outputs the bullet onto the screen.
        //Description: Draws the bullet onto the screen given the variables when called upon.
        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, new Vector2(boundingRect.X, boundingRect.Y), null, Color.White, rotation, new Vector2((int)boundingRect.Width / 2, (int)boundingRect.Height / 2), 1f, SpriteEffects.None, 0);
        }

        public void Update()
        {
            boundingRect.X = boundingRect.X + (int)velocity.X;
            boundingRect.Y = boundingRect.Y + (int)velocity.Y;
        }

        //Determines if the bullet is off screen
        public bool IsOffScreen(int optimalWidth, int optimalHeight, Vector2 scale)
        {
            if (boundingRect.Center.X < -50 || boundingRect.Center.X > optimalWidth * scale.X + 50 || boundingRect.Center.Y < -50 || boundingRect.Center.Y > optimalHeight * scale.Y + 50)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
