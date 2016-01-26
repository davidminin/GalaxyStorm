using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Galaxy_Storm
{
    class FloatingText
    {
        //Holds the message of the text
        string text;

        //Holds how long the text stays on screen for
        int life;
        public int Life
        {
            get { return life; }
        }

        //Holds the position of the text
        Vector2 position;

        //Holds the color of the text
        Color color;

        //Constructor
        public FloatingText(string s, int l, Vector2 pos, Color col)
        {
            text = s;
            life = l;
            color = col;

            //Holds the horizontal shift
            int shift;

            //Determines the horizontal shift
            switch (s.Length)
            {
                case 2:
                    shift = 32;
                    break;

                case 3:
                    if (s[0] + "" == "1")
                    {
                        shift = 32;
                    }
                    else
                    {
                        shift = 36;
                    }
                    break;

                default:
                    shift = 90;
                    break;
            }
            
            //Declares the position
            position = new Vector2((int)(pos.X - shift), pos.Y);
        }

        //Updates the life and position
        public void Update()
        {
            life--;
            position.Y -= 0.2f;
        }

        //Draws the text
        public void Draw(SpriteBatch sb, SpriteFont font)
        {
            sb.DrawString(font, text, position, color);
        }
    }
}
