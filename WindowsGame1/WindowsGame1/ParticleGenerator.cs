using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WindowsGame1
{
    class ParticleGenerator
    {
        Texture2D texture;

        float spawnWidth;
        float density;

        List<SandStorm> sandstorm = new List<SandStorm>();

        float timer;

        Random rand1, rand2;

        public ParticleGenerator(Texture2D newTexture, float newSpawnWidth, float newDensity)
        {
            texture = newTexture;
            spawnWidth = newSpawnWidth;
            density = newDensity;

            rand1 = new Random();
            rand2 = new Random();
        }

        public void CreateParticle()
        {
            //eliminates screen slow down
            double x = rand1.Next();

            sandstorm.Add(new SandStorm(texture,
                new Vector2(-50 + (float)rand1.NextDouble() * spawnWidth, 0),
                new Vector2(1, rand2.Next(4,7))));
        }

        public void Update(GameTime gameTime, GraphicsDevice graphics)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // while game is running
            while (timer > 0)
            {
                //as game runs more particles will spawn
                timer -= 1f / density;

                CreateParticle();
            }

            for (int i = 0; i < sandstorm.Count; i++)
            {
                sandstorm[i].Update();

                //if sand particle goes off the bottom of the screen
                if (sandstorm[i].Position.Y > graphics.Viewport.Height)
                {
                    //remove that particle
                    sandstorm.RemoveAt(i);
                    i--;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (SandStorm sand in sandstorm)
            {
                sand.Draw(spriteBatch);
            }
                
        }
    }
}
