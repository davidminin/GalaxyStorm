using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Galaxy_Storm
{
    //This class holds all of the ai in the game and manages them
    class AIManager
    {
        //Holds the lists of possible ai
        List<AI> loneAi = new List<AI>();
        List<Swarm> swarms = new List<Swarm>();

        //Returns the list of individual ai and sets their swarm value
        public List<AI> ReturnAI()
        {
            List<AI> allAI = new List<AI>();

            //Sets the swarm value for all lone ai 
            foreach (AI a in loneAi)
            {
                a.SwarmValue = -1;
                allAI.Add(a);
            }

            //Sets the swarm value for all swarm ai and adds to the return list
            for (int i = 0; i < swarms.Count; i++)
            {
                foreach (AI b in swarms[i].AI)
                {
                    b.SwarmValue = i;
                    allAI.Add(b);
                }
            }

            return allAI;
        }

        //Adds a lone ai
        public void AddLoneAI(AI a)
        {
            loneAi.Add(a);
        }

        //Removes an ai
        public void RemoveAI(AI a)
        {
            if (a.SwarmValue != -1)
            {
                swarms[a.SwarmValue].Grid.RemovePoint();
                swarms[a.SwarmValue].AI.Remove(a);
            }
            else
            {
                loneAi.Remove(a);
            }
        }

        //Creates a swarm
        public void CreateSwarm(AI a, AI b)
        {
            loneAi.Remove(a);
            loneAi.Remove(b);
            a.SwarmValue = SwarmCount;
            b.SwarmValue = SwarmCount;
            swarms.Add(new Swarm(a, b));
        }

        //Adds to a swarm
        public void AddToSwarm(AI a, int swarmValue)
        {
            loneAi.Remove(a);
            a.SwarmValue = swarmValue;
            swarms[swarmValue].AI.Add(a);
            swarms[swarmValue].Grid.AddPoint();
        }

        //Joins one swarm into the other swarm
        public void JoinSwarms(int indexOne, int indexTwo)
        {
            if (swarms[indexTwo].AI.Count > swarms[indexOne].AI.Count)
            {
                int tempIndex = indexTwo;
                indexTwo = indexOne;
                indexOne = tempIndex;
            }

            //Adds one swarm to another
            foreach (AI a in swarms[indexTwo].AI)
            {
                a.SwarmValue = indexOne;
                swarms[indexOne].AI.Add(a);
                swarms[indexOne].Grid.AddPoint();
            }

            //Removes the seconds swarm
            swarms.RemoveAt(indexTwo);

            //Shifts the swarm values of the other swarms
            for (int i = 0; i < swarms.Count - indexTwo; i++)
            {
                foreach (AI b in swarms[indexTwo + i].AI)
                {
                    b.SwarmValue = indexTwo + i;
                }
            }
        }

        //Returns the count of swarms
        public int SwarmCount
        {
            get { return swarms.Count; }
        }

        //Returns the list of swarm grids corresponding to each swarm
        public List<SwarmGrid> ReturnSwarmGrids()
        {
            List<SwarmGrid> grid = new List<SwarmGrid>();

            foreach (Swarm s in swarms)
            {
                grid.Add(s.Grid);
            }

            return grid;
        }

        //Updates all the ai within the manager
        public void Update(Player p, Vector2 screenSize)
        {
            //Updates individual ai
            foreach (AI a in loneAi)
            {
                a.Update(p, screenSize);          
            }

            //Updates swarms
            for(int i = 0; i < swarms.Count; i++)
            {
                //If there are more than two ai in the swarm, preform the update
                if (swarms[i].AI.Count >= 2)
                {
                    swarms[i].Update(p, screenSize);
                }
                //Otherwise disband the swarm
                else
                {
                    if (swarms[i].AI.Count == 1)
                    {
                        AddLoneAI(swarms[i].AI[0]);
                    }
                    swarms.RemoveAt(i);
                    i--;
                }
            }
        }

        //Returns a swarm at a given index
        public Swarm ReturnSwarmAtIndex(int index)
        {
            return swarms[index];
        }

        /// <summary>
        /// Holds the structure of a swarm of ai
        /// </summary>
        public class Swarm
        {
            //Holds the groups velocity
            Vector2 groupVelocity;

            //Holds the Swarm Grid associated with the swarm
            SwarmGrid grid;
            public SwarmGrid Grid
            {
                get { return grid; }
            }

            //Holds the ai of the swarm
            List<AI> ai = new List<AI>();
            public List<AI> AI
            {
                get { return ai; }
            }

            //Initializes the swarm
            public Swarm(AI a, AI b)
            {
                ai.Add(a);
                ai.Add(b);

                //Creates a very small velocity to 
                groupVelocity = a.Velocity;

                //Creates the swarm grid if needed
                switch (a.Type)
                {
                    case Type.Square:

                        grid = new SwarmGrid(a.BoundingRect.Width, a, b);
                        break;

                    case Type.Circle:

                        grid = new SwarmGrid(a.BoundingRect.Width, a, b);
                        break;

                    case Type.Triangle:

                        grid = new SwarmGrid(a.BoundingRect.Width, a, b);
                        break;
                }
            }
            
            //Holds the center of the swarm
            Point center = new Point();

            //Updates the ai in the swarm and returns the id
            public void Update(Player p, Vector2 screenDimensions)
            {
                int index = -1;
                int frameC = SetVelocityAndReturnFrameCheck(p, screenDimensions);
                float rotation = (float)(Math.Atan2(groupVelocity.Y, groupVelocity.X) + Math.PI / 2);
                Vector2 direcV = new Vector2((p.BoundingRect.Center.X - center.X), (p.BoundingRect.Center.Y - center.Y));
                direcV.Normalize();
                grid.MovePoints(groupVelocity);
                

                //Loops through every ai and 
                foreach (AI a in ai)
                {
                    index++;
                    switch (a.Type)
                    {
                        case Type.Square:
                            
                            a.Update(groupVelocity + ReturnSwarmGridVector(index, direcV), rotation, frameC);
                            break;

                        case Type.Circle:

                            a.Update(groupVelocity + ReturnSwarmGridVector(index, direcV), rotation, frameC);
                            break;

                        case Type.Triangle:

                            a.Update(groupVelocity + ReturnSwarmGridVector(index, direcV), rotation, frameC);
                            break;

                        case Type.Octagon:

                            a.Update(groupVelocity + ReturnSwarmGridVector(index, direcV), rotation, frameC);
                            break;
                    }
                }
            }

            //Determines the vector which will move the ai to the swarm grid point 
            private Vector2 ReturnSwarmGridVector(int index, Vector2 direcV)
            {
                Vector2 aiPosition = new Vector2(0, 0);
                Vector2 swarmGridVector = new Vector2(0, 0);

                //Determines if the ai is in phase
                switch (AI[index].Type)
                {
                    case Type.Square:

                        aiPosition = new Vector2(AI[index].BoundingRect.X, AI[index].BoundingRect.Y);
                        aiPosition += groupVelocity;
                        break;

                    case Type.Circle:

                        aiPosition = new Vector2(AI[index].BoundingRect.X, AI[index].BoundingRect.Y);
                        aiPosition += groupVelocity + (direcV * ai[index].Speed / 2);

                        break;

                    case Type.Triangle:

                        aiPosition = new Vector2(AI[index].BoundingRect.X, AI[index].BoundingRect.Y);
                        aiPosition += groupVelocity;
                        break;

                    case Type.Octagon:

                        aiPosition = new Vector2(AI[index].BoundingRect.X, AI[index].BoundingRect.Y);
                        aiPosition += groupVelocity;
                        break;
                }

                //If the points dont equal to eachother, the ai is out of phase phase
                if (aiPosition != grid.Points[index])
                {
                    swarmGridVector = new Vector2((Grid.Points[index].X - AI[index].BoundingRect.X),
                    (Grid.Points[index].Y - AI[index].BoundingRect.Center.Y)) / 3;
                }

                return swarmGridVector;
            }

            //Determines the velocity of a swarm of ai and returns the frame checker
            private int SetVelocityAndReturnFrameCheck(Player p, Vector2 screenSize)
            {
                center.X = 0;
                center.Y = 0;

                //Calculates Center
                for (int i = 0; i < ai.Count; i++)
                {
                    center.X += ai[i].BoundingRect.X;
                    center.Y += ai[i].BoundingRect.Y;
                }
                center.X /= ai.Count;
                center.Y /= ai.Count;

                //Holds a temporary velocity used to calculate the ai velocities
                Vector2 tempV;

                //Determines how the velocity should be calculated and applies it
                switch (ai[0].Type)
                {
                    //Velocity for a swarm of squares, animation updates every 3 frames
                    case Type.Square:

                        tempV = new Vector2((p.BoundingRect.Center.X - center.X), (p.BoundingRect.Center.Y - center.Y));
                        tempV.Normalize();
                        groupVelocity += tempV;
                        groupVelocity.Normalize();
                        groupVelocity *= ai[0].Speed;
                        return 2;

                    //Velocity for a swarm of circles, animation updates every 5 frames
                    case Type.Circle:

                        tempV = new Vector2(-groupVelocity.Y, groupVelocity.X) / 10;
                        groupVelocity += tempV;
                        groupVelocity.Normalize();
                        groupVelocity *= ai[0].Speed;
                        return 4;

                    //Velocity for a swarm of triangles, animation updates every 3 frames
                    case Type.Triangle:

                        tempV = new Vector2((p.BoundingRect.Center.X - center.X), (p.BoundingRect.Center.Y - center.Y));
                        tempV.Normalize();
                        groupVelocity += tempV;
                        groupVelocity.Normalize();
                        groupVelocity *= ai[0].Speed;
                        return 2;  
                    
                    //The type was not found
                    default:
                        return -1;
                }
            }
        }
    }
}
