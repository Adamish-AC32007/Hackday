// Animation.cs

#region using declarations
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
#endregion

namespace WindowsGame1
{
    class Animation
    {
        #region constants
        // The image representing the collection of images used for animation
        Texture2D texture;

        // The area where we want to display the image strip in the game
        Rectangle rectangle = new Rectangle();

        // Width of a given frame
        public int FrameWidth;

        // Height of a given frame
        public int FrameHeight;

        // The index of the current frame we are displaying
        int currentFrame;

        public Vector2 position;
        public Vector2 origin;
        public Vector2 velocity;

        public float timer;
        public float interval=60;

        MouseState prevState, currState;
        // Gamepad states used to determine button presses
        GamePadState currentGamePadState;

        #endregion

        #region initialize
        public void Initialize(Texture2D myTexture, Vector2 myPosition,
        int frameWidth, int frameHeight, int frameCount,
        int frametime, Color color, float scale, bool looping)
        {
            // Keep a local copy of the values passed in
            this.FrameWidth = frameWidth;
            this.FrameHeight = frameHeight;

            position = myPosition;
            texture = myTexture;
        }
        #endregion

        #region updates
        //method to change the players animation based on movement direction
        public void PlayerUpdate(GameTime gameTime)
        {

            currState = Mouse.GetState();

            rectangle = new Rectangle(currentFrame * FrameWidth, 0, FrameWidth, FrameHeight);
            origin = new Vector2(rectangle.Width / 2, rectangle.Height / 2);

            position = position + velocity;

            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.D))
            {
                AnimateFrontRight(gameTime);
                velocity.X = 3;
            }
            else if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.A))
            {
                AnimateFrontLeft(gameTime);
                velocity.X = 3;
            }
            else if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.W))
            {
                AnimateUp(gameTime);
                velocity.Y = 3;
            }
            else if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.S))
            {
                AnimateDown(gameTime);
                velocity.Y = 3;
            }
            else if (currState.LeftButton.Equals(ButtonState.Pressed) ||
               (currentGamePadState.Triggers.Right.Equals(ButtonState.Pressed)))
               // if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Space))
            {
                AnimateShoot(gameTime);
                velocity.Y = 3;
            }
            else velocity = Vector2.Zero;
        }


        public void EnemyUpdate(GameTime gameTime)
        {
            rectangle = new Rectangle(currentFrame * FrameWidth, 0, FrameWidth, FrameHeight);
            origin = new Vector2(rectangle.Width / 2, rectangle.Height / 2);

            position = position + velocity;

            if (Keyboard.GetState(PlayerIndex.Two).IsKeyDown(Keys.Right))
            {
                AnimateFrontRight(gameTime);
                velocity.X = 3;
            }
            else if (Keyboard.GetState(PlayerIndex.Two).IsKeyDown(Keys.Left))
            {
                AnimateFrontLeft(gameTime);
                velocity.X = 3;
            }
            else if (Keyboard.GetState(PlayerIndex.Two).IsKeyDown(Keys.Up))
            {
                AnimateUp(gameTime);
                velocity.Y = 3;
            }
            else if (Keyboard.GetState(PlayerIndex.Two).IsKeyDown(Keys.Down))
            {
                AnimateDown(gameTime);
                velocity.Y = 3;
            }
            else if (currState.LeftButton.Equals(ButtonState.Pressed) && 
                prevState.LeftButton.Equals(ButtonState.Released))
            {
                AnimateShoot(gameTime);
                velocity.Y = 3;
            }
            else velocity = Vector2.Zero;
        }

        #endregion

        #region Animation directions
        public void AnimateFrontRight(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds / 2;

            if (timer > interval)
            {
                currentFrame++;
                timer = 0;

                if (currentFrame > 5)
                    currentFrame = 0;
            }
        }

        public void AnimateFrontLeft(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds / 2;

            if (timer > interval)
            {
                currentFrame++;
                timer = 0;

                if (currentFrame > 11 || currentFrame < 6)
                    currentFrame = 6;
            }
        }

        public void AnimateUp(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds / 2;

            if (timer > interval)
            {
                currentFrame++;
                timer = 0;

                if (currentFrame > 11 || currentFrame < 9)
                    currentFrame = 9;
            }
        }

        public void AnimateDown(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds / 2;

            if (timer > interval)
            {
                currentFrame++;
                timer = 0;

                if (currentFrame > 5 || currentFrame < 0)
                    currentFrame = 0;
            }
        }
        
        public void AnimateShoot(GameTime gameTime)
        {
            currentFrame = 15;
        }
        #endregion

        #region draw
        // Draw the Animation Strip
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, rectangle, Color.White, 0f, origin, 1.0f, SpriteEffects.None, 0);
        }
        #endregion

    }
}
