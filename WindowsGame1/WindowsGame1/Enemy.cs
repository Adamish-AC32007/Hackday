#region Using statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
#endregion

namespace WindowsGame1
{
    class Enemy
    {



        #region enum Declaration
        /// <summary>
        ///  EnemyAIState is used to keep track of what the enemy is doing
        /// </summary>
        enum EnemyAIState
        {
            // when the enemy can't "see" the player
            Wandering,

            // chasing the player
            Chasing,

            // the final enemy is avoiding the player
            Fleeing,
 
            // the final enemy is actively trying to kill the player
            Enraged,

            // when the enemy catches the player
            Caught,
        }
        #endregion



        #region Variables
        // Animation representing the enemy
        public Animation EnemyAnimation;

        // vector to store the enemies position on the screen
        public Vector2 enemyPosition;

        // vector to store the player's position.
        public Vector2 PlayerPosition;

        // The state of the Enemy
        public bool Active;

        // The hit points of the enemy, if this goes to zero the enemy dies
        public int Health;

        // The amount of damage the enemy inflicts on the player
        public int Damage;

        // The amount of score the enemy will give to the player
        public int Value;

        // Get the width of the enemy
        public int Width
        {
            get { return EnemyAnimation.FrameWidth; }
        }

        // Get the height of the enemy
        public int Height
        {
            get { return EnemyAnimation.FrameHeight; }
        }

        // The speed at which the enemy moves
        // his speed will increase to increase the challenge.
         float enemyMoveSpeed = 5.0f;

        float enemyTurnSpeed = 0.10f;

        float MaxEnemySpeed = 7.0f;

        float enemyChaseDistance = 250.0f;

        float enemyCaughtDistance = 650.0f;

        float enemyHysteresis = 15.0f;

        Vector2 enemyTextureCenter;
        EnemyAIState enemyState = EnemyAIState.Wandering;
        float enemyOrientation;
        Vector2 enemyWanderDirection;

        bool lastEnemy;

        const float enemyEvadeDistance = 150.0f;

        Random random = new Random();

        GraphicsDeviceManager graphics;
        #endregion



        #region Initialize
        public void Initialize(Animation animation, Vector2 position, Vector2 playerPosition, GraphicsDeviceManager Graphics, bool LastEnemy)
        {
            // Load the enemy ship texture
            EnemyAnimation = animation;

            // Set the position of the enemy
            enemyPosition = position;

            // get the position of the player
            PlayerPosition = playerPosition;

            graphics = Graphics;

            // boolean value sent true if the enemy is the last one in the current wave
            lastEnemy = LastEnemy;

            // We initialize the enemy to be active so it will be update in the game
            Active = true;

            // Set the health of the enemy
            Health = 10;

            // Set the amount of damage the enemy can do
            Damage = 10;

            // Set the score value of the enemy
            Value = 10;

            // calculate the center of the enemy
            enemyTextureCenter = new Vector2(animation.FrameWidth / 2, animation.FrameHeight / 2);
        }
        #endregion



        #region Update and Maths Functions

        public void Update(GameTime gameTime)
        {
            //System.Diagnostics.Debug.WriteLine(enemyState);
            // update enemy will run the AI code that control's the enemy movements
            UpdateEnemy();

            // Update the position of the Animation
            EnemyAnimation.position = enemyPosition;

            // Update Animation
            EnemyAnimation.EnemyUpdate(gameTime);
            
            // Make sure the enemy stays on the screen
            enemyPosition = ClampToViewport(enemyPosition);
            
            // If the enemy is past the screen or its health reaches 0 then deactivate it
            if (enemyPosition.X < -Width || Health <= 0)
            {
                // By setting the Active flag to false, the game will remove this objet from the 
                // active game list
                Active = false;
            }
            
        }
       
        // keep enemies on the screen
        private Vector2 ClampToViewport(Vector2 Position)
        {
            Viewport vp = graphics.GraphicsDevice.Viewport;
            Position.X = MathHelper.Clamp(Position.X, vp.X, vp.X + vp.Width);
            Position.Y = MathHelper.Clamp(Position.Y, vp.Y, vp.Y + vp.Height);
            return Position;
        }
         

        private void UpdateEnemy()
        {
            int reaction;
            float enemyChaseThreshold = enemyChaseDistance;
            float enemyCaughtThreshold = enemyCaughtDistance;

            #region Check States
            // if the enemy is idle, he prefers to stay idle. we do this by making the
            // chase distance smaller, so the enemy will be less likely to begin chasing
            // the player.
            if (enemyState == EnemyAIState.Wandering)
            {
                enemyChaseThreshold -= enemyHysteresis / 5;
            }
            // similarly, if the enemy is active, he prefers to stay active. we
            // accomplish this by increasing the range of values that will cause the
            // enemy to go into the active state.
            else if (enemyState == EnemyAIState.Chasing)
            {
                enemyChaseThreshold += enemyHysteresis / 2;
                enemyCaughtThreshold -= enemyHysteresis / 2;
            }
            else if (enemyState == EnemyAIState.Caught)
            {
                enemyCaughtThreshold += enemyHysteresis / 2;
            }
            else if (enemyState == EnemyAIState.Fleeing)
            {

            }
            else if (enemyState == EnemyAIState.Enraged)
            {
                enemyChaseThreshold += enemyHysteresis / 5;
            }

            #endregion

            // calculate the enemies distance from the player
            float distanceFromPlayer = Vector2.Distance(enemyPosition, PlayerPosition);

            #region Set States
            if (distanceFromPlayer > enemyChaseThreshold)
            {
                // if the player is far away from the enemy, it should be idle
                // and wander around the screen
                enemyState = EnemyAIState.Wandering;
            }
            else if (distanceFromPlayer < enemyChaseThreshold)
            {
                //enemyState = EnemyAIState.Chasing;
            }
            else if (lastEnemy == true)
            {
                reaction = random.Next(1,2);
                
                // randomly decide if the last enemy in a wave will fight or flee
                if (reaction == 1)
                {
                    enemyState = EnemyAIState.Fleeing;
                }
                else if(reaction==2)
                {
                    enemyState = EnemyAIState.Enraged;
                }
            }
            #endregion

            float currentEnemySpeed;

            if (enemyState == EnemyAIState.Chasing)
            {
                enemyOrientation = TurnToFace(enemyPosition, PlayerPosition,
                    enemyOrientation, enemyTurnSpeed);
                currentEnemySpeed = MaxEnemySpeed;
            }
            else if (enemyState == EnemyAIState.Wandering)
            {
                Wander(enemyPosition, ref enemyWanderDirection, ref enemyOrientation,
                    enemyTurnSpeed);
                currentEnemySpeed = .25f * MaxEnemySpeed;
            }
            else if (enemyState == EnemyAIState.Fleeing)
            {
                Vector2 seekPosition = 2 * enemyPosition - PlayerPosition;

                enemyOrientation = TurnToFace(enemyPosition, seekPosition,
                    enemyOrientation, enemyTurnSpeed);

                currentEnemySpeed = .75f * MaxEnemySpeed;
            }
            else if (enemyState == EnemyAIState.Enraged)
            {
                enemyOrientation = TurnToFace(enemyPosition, PlayerPosition,
                    enemyOrientation, enemyTurnSpeed);

                // give the enraged enemy a speed boost
                currentEnemySpeed = MaxEnemySpeed;

                // give the enraged enemy an extra 50% health
                Health = Health + (Health / 2);

                // increase the enraged enemy's damage output
                Damage = Damage + (Damage / 2);
            }
            else
            {
                enemyState = EnemyAIState.Caught;
                currentEnemySpeed = 0.0f;
            }

            Vector2 heading = new Vector2((float)Math.Cos(enemyOrientation),
                (float)Math.Sin(enemyOrientation));

            enemyPosition += heading * currentEnemySpeed;
        }

        private void Wander(Vector2 Position, ref Vector2 enemyWanderDirection, ref float enemyOrientation, float enemyTurnSpeed)
        {
            enemyWanderDirection.X += MathHelper.Lerp(-.25f, .25f, (float)random.NextDouble());
            enemyWanderDirection.Y += MathHelper.Lerp(-.25f, .25f, (float)random.NextDouble());

            if (enemyWanderDirection != Vector2.Zero)
            {
                enemyWanderDirection.Normalize();
            }

            enemyOrientation = TurnToFace(Position, Position + enemyWanderDirection, enemyOrientation, .15f * enemyTurnSpeed);

            Vector2 screenCenter = Vector2.Zero;
            screenCenter.X = 400;
            screenCenter.Y = 300;

            float distanceFromScreenCenter = Vector2.Distance(screenCenter, Position);
            float MaxDistanceFromScreenCenter = Math.Min(screenCenter.Y, screenCenter.X);

            float normalizedDistance =
                distanceFromScreenCenter / MaxDistanceFromScreenCenter;

            float turnToCenterSpeed = .3f * normalizedDistance * normalizedDistance
                * enemyTurnSpeed;

            enemyOrientation = TurnToFace(Position, screenCenter, enemyOrientation,
                turnToCenterSpeed);
            
        }

        private float TurnToFace(Vector2 Position, Vector2 faceThis, float currentAngle, float enemyTurnSpeed)
        {
            float x = faceThis.X - Position.X;
            float y = faceThis.Y - Position.Y;

            float desiredAngle = (float)Math.Atan2(y, x);

            float difference = WrapAngle(desiredAngle - currentAngle);

            difference = MathHelper.Clamp(difference, -enemyTurnSpeed, enemyTurnSpeed);

            return WrapAngle(currentAngle + difference);
        }

        private float WrapAngle(float radians)
        {
            while (radians < -MathHelper.Pi)
            {
                radians += MathHelper.TwoPi;
            }
            while (radians > MathHelper.Pi)
            {
                radians -= MathHelper.TwoPi;
            }
            return radians;
        }


        #endregion



        #region Draw
        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the animation
            EnemyAnimation.Draw(spriteBatch);
        }
        #endregion
    }
}
