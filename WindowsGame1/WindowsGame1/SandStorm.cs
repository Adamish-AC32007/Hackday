using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WindowsGame1
{
    class SandStorm
    {
        Texture2D texture;
        Vector2 position;
        Vector2 velocity;

        public Vector2 Position
        {
            get { return position; }
        }

        public SandStorm(Texture2D newTexture, Vector2 newPosition, Vector2 newVolcity)
        {
            texture = newTexture;
            position = newPosition;
            velocity = newVolcity;
        }

        public void Update()
        {
            position += velocity;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }
    }
}
