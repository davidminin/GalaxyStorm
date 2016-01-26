using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Galaxy_Storm
{
    //Class used to handle the locations of the ai within a swarm
    class SwarmGrid
    {
        //Holds all the points
        List<Vector2> points = new List<Vector2>();
        public List<Vector2> Points
        {
            get { return points; }
        }

        //Holds the amount of layers in the grid, always at layer 1 when initialized
        int layers = 0;
        public int Layers
        {
            get { return layers; }
        }

        //Holds the size of every ai
        int size;

        //Holds the angle of expansion
        double angle = Math.PI * 2;

        //Holds the amount the angle must increase by
        double arcLength;

        //Holds the amount of objects in the layer
        int pointsInLayer = 0;

        //Constructer of the SwarmGrid Ai 'a' holds the center point for the grid
        public SwarmGrid(int Size, AI a, AI b)
        {
            size = Size;

            //Adds all the points
            points.Add(new Vector2(a.BoundingRect.X, a.BoundingRect.Y));
            AddPoint();
        }

        //Removes a point at the end of the list
        public void RemovePoint()
        {
            points.RemoveAt(points.Count - 1);
            pointsInLayer--;
            angle -= arcLength;

            if (pointsInLayer == 0)
            {
                layers--;
                pointsInLayer = (int)(2 * Math.PI * layers);

                arcLength = 2 * Math.PI / pointsInLayer;
                angle = Math.PI * 2 - arcLength;
            }
        }

        //Expands the grid by another point
        public void AddPoint()
        {
            if (pointsInLayer == (int)(2 * Math.PI * layers))
            {
                layers++;
                pointsInLayer = 0;

                //Gets the new angle increment (arcLength) and resets the current angle
                int totPointsInLayer = (int)(2 * Math.PI * layers);
                arcLength = 2 * Math.PI / totPointsInLayer;
                angle = -arcLength;
            }

            angle += arcLength;
            pointsInLayer++;
            points.Add(new Vector2((int)(size * layers * Math.Cos(angle)) + points[0].X, (int)(size * layers * Math.Sin(angle) + points[0].Y)));

        }

        //Moves all the points by a certain vector
        public void MovePoints(Vector2 moveV)
        {
            for (int i = 0; i < points.Count; i++)
            {
                points[i] += moveV;
            }
        }

        //Draws the Swarm Grid, for testing purposes
        public void Draw(SpriteBatch sb, Texture2D img, SpriteFont sf)
        {
            for (int i = 0; i < points.Count; i++)
            {
                sb.Draw(img, new Vector2(points[i].X - (size / 2), points[i].Y - size), Color.White);
            }

            sb.DrawString(sf, layers + "", points[0], Color.White);
            sb.DrawString(sf, angle + "", points[1], Color.White);
            sb.DrawString(sf, arcLength + "", new Vector2(points[0].X , points[0].Y + 20), Color.White);
        }
    }
}
