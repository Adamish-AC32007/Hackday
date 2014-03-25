using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
    class Player
    {
        // Animation representing the player
        public Animation PlayerAnimation;
        public Vector2 Position;

        public bool Active;

        public int Health;

        // Get the width of the animated player 
        public int Width
        {
            get { return PlayerAnimation.FrameWidth; }
        }

        // Get the height of the animated player 
        public int Height
        {
            get { return PlayerAnimation.FrameHeight; }
        }

        // Initialize the animated player
        public void Initialize(Animation animation, Vector2 position)
        {
            PlayerAnimation = animation;

            // Set the starting position of the player around the middle of the screen and to the back
            Position = position;

            // Set the player to be active
            Active = true;

            // Set the player health
            Health = 100;
        }

        // Update the player animation
        public void Update(GameTime gameTime)
        {
            PlayerAnimation.position = Position;
            PlayerAnimation.PlayerUpdate(gameTime);
        }

        // Draw the player
        public void Draw(SpriteBatch spriteBatch)
        {
            PlayerAnimation.Draw(spriteBatch);
        }

    }
}
