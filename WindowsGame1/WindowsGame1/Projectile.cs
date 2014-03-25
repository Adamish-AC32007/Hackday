﻿// Projectile.cs
#region Using declarations
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion
namespace WindowsGame1
{
    class Projectile
    {
        #region Constants

        // Image representing the Projectile
        public Texture2D Texture;

        // Position of the Projectile relative to the upper left side of the screen
        public Vector2 projectilePosition;

        // State of the Projectile
        public bool Active;

        // The amount of damage the projectile can inflict to an enemy
        public int Damage;

        // Represents the viewable boundary of the game
        public Viewport viewport;

        // vector to store the direction of the projectile
        public Vector2 Direction { get; set; }

        // vector to store the mouse location
        Vector2 mousePosition;

        // Get the width of the projectile 
        public int Width
        {
        get { return Texture.Width; }
        }

        // Get the height of the projectile 
        public int Height
        {
        get { return Texture.Height; }
        }

        // Determines how fast the projectile moves
        public float projectileMoveSpeed;

        #endregion

        #region Initialize
        // this class is the invoker class which executes the command to fire the weapon at the mouse position
        public void Initialize(Viewport viewport, Texture2D texture, Vector2 position, Vector2 mousePos)
        {
            this.mousePosition = mousePos;
            this.viewport = viewport;
            this.Texture = texture;
            this.projectilePosition = position;

            this.Active = true;

            Direction = GetDirection(this.projectilePosition, this.mousePosition);

            Damage = 2;

            projectileMoveSpeed = 40f;
        }
        #endregion

        #region Update and Draw

        public void Update()
        {
            this.projectilePosition += Direction * projectileMoveSpeed;
        }

        // get the direction of the trajectory for the bullet
        private Vector2 GetDirection(Vector2 positionOfObject, Vector2 directionToPointAt)
        {
            Vector2 direction = directionToPointAt - positionOfObject;
            direction.Normalize();

            return direction;
        }

        // draw the bullet to the screen
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.Texture, this.projectilePosition, null, Color.White, 0f,
            new Vector2(Width / 2, Height / 2), 1f, SpriteEffects.None, 0f);
        }
        #endregion
    }
}