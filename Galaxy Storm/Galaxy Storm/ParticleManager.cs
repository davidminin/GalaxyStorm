//Author: David Minin   
//Project Name: Galaxy Storm
//File Name: Galaxy Storm
//Creation Date: January 31, 2013
//Modification Date: January 23, 2014
//Description: Holds the classes, properties, and subprograms used for allParticles.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Galaxy_Storm
{
    class ParticleManager
    {
        //Holds the Particle allParticles
        public ParticleArray particleManager;

        //Initializes the Particle Manager
        public ParticleManager(int capacity)
        {
            particleManager = new ParticleArray(capacity);
            particleManager.PopulateArray();
        }

        //Holds the framework for a particle
        class Particle
        {
            //Holds the maximum life of the particle
            public int maxLife;

            //Holds the velocity of the particle.
            public Vector2 velocity;

            //Holds the position of the particle.
            public Vector2 position;

            //Holds the image of the particle.
            public Texture2D image;

            //Holds the time left of the particle.
            public int timeRemaining;

            //Holds the color of the particle.
            public Color color;

            //Holds the color the particle will turn in to
            public Color lerpC;

            //Holds the rotation of the particle
            public float rotation;

            //Holds the length of the particle
            public float length;
        }

        //Holds the framework for circulating allParticles to prevent lag due to memory usage
        public class ParticleArray
        {
            Particle[] allParticles;
            List<int> inactiveParticles;
            List<int> activeParticles;

            //Constructs a list of allParticles with a given capacity
            public ParticleArray(int capacity)
            {
                allParticles = new Particle[capacity];
                inactiveParticles = new List<int>();
                activeParticles = new List<int>();
            }

            //Creates a particle
            public void CreateParticle(Vector2 vel, Vector2 pos, Texture2D img, int life, Color col, float rot)
            {
                Particle particle = ReturnParticle(FirstInactiveIndex());
                activeParticles.Add(FirstInactiveIndex());
                inactiveParticles.RemoveAt(0);

                //Sets all the properties for the particle
                particle.velocity = vel;
                particle.position = pos;
                particle.image = img;
                particle.timeRemaining = life;
                particle.lerpC = col;
                particle.color = Color.White;
                particle.rotation = rot;
                particle.length = 1;
                particle.maxLife = life;
                particle.rotation = (float)(Math.Atan2(vel.Y, vel.X) + (Math.PI / 2));
            }

            //Updates the particles
            public void UpdateParticles(Vector2 monitor)
            {
                List<int> particlesToRemove = new List<int>();

                //Loops through all active particles
                for (int i = 0; i < activeParticles.Count; ++i)
                {
                    int pIndex = activeParticles[i];
                    Vector2 pos = allParticles[pIndex].position;
                    int maxLife = allParticles[pIndex].maxLife;
                    
                    allParticles[pIndex].timeRemaining--;
                    int timeRemaining = allParticles[pIndex].timeRemaining;
                    Vector2 tempV = allParticles[pIndex].velocity;

                    //Updates the Particle
                    if (timeRemaining > 0 && (pos.X > 0 && pos.X < monitor.X)
                        && (pos.Y > 0 && pos.Y < monitor.Y))
                    {
                        if (timeRemaining <  maxLife - 5)
                        {
                            if (timeRemaining >= 17)
                            {
                                allParticles[pIndex].color = Color.Lerp(Color.White, allParticles[pIndex].lerpC, 0.9f - (timeRemaining / (float)(maxLife)));
                            }
                            else
                            {
                                allParticles[pIndex].color = Color.Lerp(Color.White, allParticles[pIndex].lerpC, 0.6f);
                            }
                            allParticles[pIndex].velocity = tempV * 0.965f;

                            if (timeRemaining < 25)
                                allParticles[pIndex].color.A = (byte)(10 * timeRemaining);
                        }

                        allParticles[pIndex].velocity = tempV * 0.955f;
                        allParticles[pIndex].position += tempV;
                        tempV.Normalize();
                        allParticles[pIndex].length = tempV.Length() * (timeRemaining / 1.5f) + 10;
                    }
                    //Adds inactive particles to be removed
                    else
                    {
                        particlesToRemove.Add(pIndex);
                    }
                }

                //Kills inactive particles
                if (particlesToRemove.Count > 0)
                {
                    for (int j = 0; j < particlesToRemove.Count; ++j)
                    {
                        int index = particlesToRemove[j];
                        activeParticles.Remove(index);
                        inactiveParticles.Add(index);
                    }
                }
            }

            //Draws the active particles
            public void DrawParticles(SpriteBatch sb)
            {
                for (int i = 0; i < activeParticles.Count; ++i)
                {
                    Particle p = allParticles[activeParticles[i]];
                    sb.Draw(p.image, new Rectangle((int)p.position.X, (int)p.position.Y, 4, (int)p.length), null, p.color, p.rotation, new Vector2(0, 0), SpriteEffects.None, 0);
                }
            }

            //Populates the Array with empty particles
            public void PopulateArray()
            {
                for (int i = 0; i < allParticles.Length; i++)
                {
                    inactiveParticles.Add(i);
                    allParticles[i] = new Particle();
                }
            }

            //Returns a Particle at a given index
            private Particle ReturnParticle(int index)
            {
                return allParticles[index];
            }

            //Returns the first inactive particle  
            public int FirstInactiveIndex()
            {
                return inactiveParticles[0];
            }

            //Determines if there are any inactive particles for use
            public bool AnyInactiveParticles()
            {
                if (inactiveParticles.Count > 0)
                    return true;
                else
                    return false;
            }

            //Determines if there are any inactive particles for use
            public bool AnyActiveParticles()
            {
                if (activeParticles.Count > 0)
                    return true;
                else
                    return false;
            }

            //Testing Purposes
            public int ReturnActiveParticles()
            {
                return activeParticles.Count;
            }
            public int ReturnInactiveParticles()
            {
                return inactiveParticles.Count;
            }
        }
    }
}
