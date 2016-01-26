using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Galaxy_Storm
{
    class QuadTree
    {
        //Constant values
        const int MAX_OBJECTS = 3;
        const int MAX_LEVELS = 5; 

        //Holds which level the quadrant is in
        int level;

        //Holds the entities that are in the quadrant
        List<Entity> objects;

        //Holds the corner of the quadrant
        Point topLeftCorner;

        //Holds the width and height of the quadrant
        int width;
        int height;

        //Holds smaller quadrants that make up the current one
        QuadTree[] nodes;


        //Constructor
        public QuadTree(int nLevel, Point corner, int w, int h) 
        {
            level = nLevel;
            objects = new List<Entity>();
            topLeftCorner = corner;
            width = w;
            height = h;
            nodes = new QuadTree[4];
        }


        //Clears the quadrant along with any quadrants that it holds
        public void Clear()
        {
            objects.Clear();

            //Loops through the smaller quadrants and clears them if they exist
            if (nodes[0] != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    nodes[i].Clear();
                    nodes[i] = null;
                }
            }
        }

        //Splits the current quadrant into four sub nodes
        private void Split()
        {
            //Gets the dimensions of the new nodes
            int subWidth = (int)(width / 2);
            int subHeight = (int)(height / 2);

            //Declares the bounderies of the nodes
            nodes[0] = new QuadTree(level + 1, new Point(topLeftCorner.X + subWidth, topLeftCorner.Y), subWidth, subHeight);
            nodes[1] = new QuadTree(level + 1, topLeftCorner, subWidth, subHeight);
            nodes[2] = new QuadTree(level + 1, new Point(topLeftCorner.X, topLeftCorner.Y + subHeight), subWidth, subHeight);
            nodes[3] = new QuadTree(level + 1, new Point(topLeftCorner.X + subWidth, topLeftCorner.Y + subHeight), subWidth, subHeight);
        }

        //Determines which quadrant the object can fit in and returns -1 if it cannot fit into any of the quadrants
        private int GetIndex(Rectangle rect)
        {
            int index = -1;
            int midX = topLeftCorner.X + width / 2;
            int midY = topLeftCorner.Y + height / 2;

            //If it can fit in the top half
            if (rect.Center.Y + 5 < midY)
            {
                //If it can fit in the left side, Quadrant 2
                if (rect.Center.X + 5 < midX) 
                {
                    index = 1;
                }
                //If it can fit on the right side, Quadrant 1
                else if (rect.Center.X - 5 > midX)
                {
                    index = 0;
                }
            }
            //If it can fit in the bottom half
            else if (rect.Center.Y - 5 > midY)
            {
                //If it can fit in the left side, Quadrant 3
                if (rect.Center.X + 5 < midX)
                {
                    index = 2;
                }
                //If it can fit on the right side, Quadrant 4
                else if (rect.Center.X - 5 > midX)
                {
                    index = 3;
                }
            }

            return index;
        }

        //Adds a player to a quadtree
        public void Insert(Entity p)
        {
            //If the subnodes exist, attempt to add the player to a subquadrant
            if (nodes[0] != null)
            {
                int index = GetIndex(p.BoundingRect);

                //Adds to a sub quadrant
                if (index != -1)
                {
                    nodes[index].Insert(p);

                    return;
                }
                //Else add it to the parent node
                else
                {
                    objects.Add(p);
                }
            }
            else
            {
                objects.Add(p);
            }

            //Split the quadtree if possible
            if (objects.Count() >= MAX_OBJECTS && level < MAX_LEVELS)
            {
                //It is possible to split
                if (nodes[0] == null)
                {
                    Split();

                    int counter = objects.Count - 1;
                    //A counter used to cycle through all the objects
                    while(counter > -1)
                    {
                        int index = GetIndex(objects[counter].BoundingRect);

                        //Place the object in a sub node
                        if (index != -1)
                        {
                            nodes[index].Insert(objects[counter]);
                            objects.RemoveAt(counter);
                        }

                        counter--;
                    }
                }
            }
        }

        //Retrieves all possible objects that can collide with the given object
        public List<Entity> RetrieveList(List<Entity> returnEntity, Entity e, bool searching, QuadTree quad)
        {
            //If there are more subnodes, recursively return the objects there
            if (searching)
            {
                int tempIndex = quad.GetIndex(e.BoundingRect);

                //Attempts to continue searching 
                if (tempIndex >= 0 && quad.nodes[0] != null)
                {
                    returnEntity = RetrieveList(returnEntity, e, true, quad.nodes[tempIndex]);
                }
                //Otherwise begin retrieving the list
                else
                {
                    returnEntity = RetrieveList(returnEntity, e, false, quad);
                }
            }
            else
            {
                //If there are more subnodes, recursively return the objects there
                if (quad.nodes[0] != null)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        returnEntity = quad.nodes[i].RetrieveList(returnEntity, e, false, quad.nodes[i]);
                    }
                }

                //Adds all the objects in the given node
                for (int i = 0; i < quad.objects.Count; i++)
                {
                    returnEntity.Add(quad.objects[i]);
                }
            }

            return returnEntity;
        }

        //Retrieves all possible objects that can collide with the given object
        public List<Entity> RetrieveList(List<Entity> returnEntity, Entity e)
        {
            //If there are more subnodes, recursively return the objects there
            if (nodes[0] != null)
            {
                returnEntity = nodes[0].RetrieveList(returnEntity, e);
                returnEntity = nodes[1].RetrieveList(returnEntity, e);
                returnEntity = nodes[2].RetrieveList(returnEntity, e);
                returnEntity = nodes[3].RetrieveList(returnEntity, e);
            }

            //Adds all the objects in the given node
            for (int i = 0; i < objects.Count; i++)
            {
                returnEntity.Add(objects[i]);
            }

            return returnEntity;
        }

        //Determines which quadtree an entity is in
        private QuadTree DetermineQuadTree(Entity e)
        {
            return null;
        }

        //Draws the quadrants on the screen
        public void DrawQuadrants(SpriteBatch sb, Texture2D tex)
        {
            sb.Draw(tex, new Rectangle(topLeftCorner.X, topLeftCorner.Y, width, height), Color.White);
            
            if (nodes[0] != null)
            {
                nodes[0].DrawQuadrants(sb, tex);
                nodes[1].DrawQuadrants(sb, tex);
                nodes[2].DrawQuadrants(sb, tex);
                nodes[3].DrawQuadrants(sb, tex);
            }
        }
    }
}
