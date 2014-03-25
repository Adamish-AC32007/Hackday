// Background.cs
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
    class Background
    {
        // The image representing the background
        Texture2D texture;

        public void Initialize(ContentManager content, String texturePath)
        {
            // Load the background texture we will be using
            texture = content.Load<Texture2D>(texturePath);
        }


        public void Update()
        {
            //code to load in the next frame of the background
            //screenFrame = new Rectangle(currentFrame * FrameWidth, 0, FrameWidth, FrameHeight);
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture,Vector2.Zero, Color.White);
        }

    }
}
