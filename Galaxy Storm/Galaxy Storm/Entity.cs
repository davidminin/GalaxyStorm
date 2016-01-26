using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Galaxy_Storm
{
    class Entity
    {
        //This class is used to determine which objects are non static, player, bullets, and ai

        //Holds the bounding rectangle of the entity
        protected Rectangle boundingRect;
        public Rectangle BoundingRect
        {
            get { return boundingRect; }
            set { boundingRect = value; }
        }

        //Holds the velocity of the entity
        protected Vector2 velocity;

        //Holds the rotation fo the entity
        protected float rotation;
    }
}
