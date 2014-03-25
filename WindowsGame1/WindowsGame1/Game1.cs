#region Using statements
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
using System.IO;
#endregion
namespace WindowsGame1
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {



        #region GameState
        enum GameState
        {
            MainMenu,

            Playing,

            Instructions,

            Highscore,

            GameOver,

            Username,
        }
        GameState CurrentGameState = GameState.MainMenu;

        #endregion



        #region Variables

        // screen Adjustments
        int screenWidth = 800;
        int screenHeight = 600;

        cButton btnPlay;
        cButton btnMain;
        cButton btnInstructions;
        cButton btnHighscores;
        cButton btnFinished;

        // generate new player object
        Player player = new Player();
        Animation playerAnimation;

        // create instance of cassandra DB
        SimpleClient cassandra;

        // store the player's position on the screen
        Vector2 playerPosition;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        ParticleGenerator sand;

        // Keyboard states used to determine key presses
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        // Gamepad states used to determine button presses
        GamePadState currentGamePadState;
        GamePadState previousGamePadState;

        // A movement speed for the player
        float playerMoveSpeed;

        // Image used to display the static background
        Texture2D mainBackground;

        // Enemies
        Texture2D enemyTexture;
        List<Enemy> enemies;

        // The rate at which the enemies appear
        TimeSpan enemySpawnTime;
        TimeSpan previousSpawnTime;

        // A random number generator
        Random random;

        Texture2D projectileTexture;
        List<Projectile> projectiles;
        List<enemyProjectile> enemyProjectiles;

        // the rate of fire for the enemy bullet
        TimeSpan fireTime;
        TimeSpan previousFireTime;

        MouseState prevState, currState;

        SoundEffect click;
        SoundEffect gunshot;
        SoundEffect reload;

        Song MenuMusic;
        Song gameplayMusic;

        // number to hold the player's high score
        int score;

        // font to display UI elements
        SpriteFont font;
        SpriteFont newfont;

        // used to determine enemy AI behaviour
         bool lastEnemy;

        // counts the number of kills the player scored
        int kills = 0;

        // the players current ammunition count
        int ammo = 14;

        // holds the current number of shells in the players weapon
        int magazine = 7;

        float rotation;

        String username="test";
        int accuracy = 0;
        int totalshots = 0;
        int meleekills = 0;
        int enemyHit = 0;

        // vector to hold the mouse position on the screen
        Vector2 mousePosition;

        // manager to handle file operations
        FileManager openFile = new FileManager();

        // bool to check whether the players highscore has been saved
        bool saved = false;

        // bool to see if the chosen username already exists
        bool found = false;

        Guid playTime;
        #endregion



        #region Initialize
        public Game1()
        {
            this.IsMouseVisible = true;
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.PreferredBackBufferWidth = screenWidth;

            // code to turn the game into a full screen window
            /*
            if (!graphics.IsFullScreen)
                graphics.ToggleFullScreen;
            */

            graphics.ApplyChanges();

            Content.RootDirectory = "Content";
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //Initialize the player class
            player = new Player();
            playerAnimation = new Animation();

            //initialize the Cassandra DB
            cassandra = new SimpleClient();

            playerAnimation.Initialize(Content.Load<Texture2D>("Player"), Vector2.Zero, 47, 40, 6, 4, Color.White, 1f, true);

            // Set a constant player move speed
            playerMoveSpeed = 8.0f;

            // Initialize the enemies list
            enemies = new List<Enemy>();

            // Set the time keepers to zero
            previousSpawnTime = TimeSpan.Zero;

            // Used to determine how fast enemy respawns
            enemySpawnTime = TimeSpan.FromSeconds(1.0f);

            // Initialize our random number generator
            random = new Random();

            projectiles = new List<Projectile>();
            enemyProjectiles = new List<enemyProjectile>();

            // Set the bullet to fire every 2 seconds
            fireTime = TimeSpan.FromSeconds(2f);

            // set player's high score to zero
            score = 0;

            base.Initialize();
        }
        #endregion



        #region Load & Unload content
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            sand = new ParticleGenerator(Content.Load<Texture2D>("Sand"), graphics.GraphicsDevice.Viewport.Width, 100);

            playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y
            + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
            player.Initialize(playerAnimation, playerPosition);

            btnPlay = new cButton(Content.Load<Texture2D>("Button"), graphics.GraphicsDevice);
            btnPlay.setPosition(new Vector2(500, 300));

            btnInstructions = new cButton(Content.Load<Texture2D>("Instructions"), graphics.GraphicsDevice);
            btnInstructions.setPosition(new Vector2(500, 400));

            btnMain = new cButton(Content.Load<Texture2D>("MainButton"), graphics.GraphicsDevice);
            btnMain.setPosition(new Vector2(0, 550));

            btnHighscores = new cButton(Content.Load<Texture2D>("Highscores"), graphics.GraphicsDevice);
            btnHighscores.setPosition(new Vector2(500, 500));
            
            btnFinished = new cButton(Content.Load<Texture2D>("Button"), graphics.GraphicsDevice);
            btnFinished.setPosition(new Vector2(500, 300));

            mainBackground = Content.Load<Texture2D>("desert");
            enemyTexture = Content.Load<Texture2D>("Enemy");
            projectileTexture = Content.Load<Texture2D>("bullet");

            // load the gameplay music
            MenuMusic = Content.Load<Song>("Music");
            gameplayMusic = Content.Load<Song>("Hitman");

            // load the gunshot sound effect
            gunshot = Content.Load<SoundEffect>("gunshot");
            click = Content.Load<SoundEffect>("empty_click");
            reload = Content.Load<SoundEffect>("reload");

            font = Content.Load<SpriteFont>("gameFont");
            newfont = Content.Load<SpriteFont>("highscore");


            PlayMusic(MenuMusic);
        }

        private void PlayMusic(Song gameplayMusic)
        {
            try
            {
                //play the music
                MediaPlayer.Play(gameplayMusic);

                //loop the currently repeating song
                MediaPlayer.IsRepeating = true;
            }
            catch { }
        }


        private void StopMusic()
        {
            try
            {
                //play the music
                MediaPlayer.Stop();
            }
            catch { }
        }
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }
        #endregion



        #region Update

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();
            mousePosition = new Vector2(mouse.X, mouse.Y);

            switch (CurrentGameState)
            {
                case GameState.MainMenu:
                    {
                        if (btnPlay.isClicked == true)
                        {
                            CurrentGameState = GameState.Username;
                        }
                        if (btnInstructions.isClicked == true)
                        {
                            CurrentGameState = GameState.Instructions;
                        }

                        if (btnHighscores.isClicked == true)
                        {
                            CurrentGameState = GameState.Highscore;
                        }
                        btnPlay.Update(mouse);
                        btnInstructions.Update(mouse);
                        btnHighscores.Update(mouse);
                        break;
                    }

                // get the user to enter their username here
                case GameState.Username:
                    {
                        username = Microsoft.VisualBasic.Interaction.InputBox("Enter username","Username","");
                        if (username!="" && username!="test")
                        {
                            CurrentGameState = GameState.Playing;
                            StopMusic();
                            PlayMusic(gameplayMusic);
                        }
                        btnMain.Update(mouse);
                        break;
                    }


                case GameState.Highscore:
                    {
                        if (btnMain.isClicked == true)
                        {
                            CurrentGameState = GameState.MainMenu;
                        }
                        btnMain.Update(mouse);

                        break;
                    }


                case GameState.Instructions:
                    {
                        if (btnMain.isClicked == true)
                        {
                            CurrentGameState = GameState.MainMenu;
                        }
                        btnMain.Update(mouse);

                        break;
                    }

                case GameState.Playing:
                    {           
                        sand.Update(gameTime, graphics.GraphicsDevice);
                        Animation playerAnimation = new Animation();

                        // Save the previous state of input methods so we can determine single key/button presses
                        previousGamePadState = currentGamePadState;
                        previousKeyboardState = currentKeyboardState;
                        prevState = currState;

                        // Read the current state of the input devices and store it
                        currentKeyboardState = Keyboard.GetState();
                        currentGamePadState = GamePad.GetState(PlayerIndex.One);
                        currState = Mouse.GetState();

                        // Allows the game to exit
                        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                         (currentKeyboardState.IsKeyDown(Keys.Escape)))
                            this.Exit();

                        //Update the player
                        UpdatePlayer(gameTime);

                        // Update the enemies
                        UpdateEnemies(gameTime);

                        // check for collisions
                        UpdateCollision();

                        // Update the projectiles
                        UpdateProjectiles(gameTime);

                        base.Update(gameTime);

                        break;
                    }

                case GameState.GameOver:
                    {
                        if (saved == false)
                        {
                            openFile.writeFile(score);
                            saved = true;

                            // if the player never fires his weapon
                            // his accuracy will be 0, because cannot divide by 0;
                            try
                            {
                                accuracy = ((enemyHit) / totalshots) * 100;
                            }
                            catch (DivideByZeroException)
                            {
                                accuracy = 0;
                            }
                            playTime = Guid.NewGuid();

                            cassandra.LoadData(username, accuracy, totalshots, meleekills, kills, score,playTime);
                            CurrentGameState = GameState.MainMenu;
                            btnPlay.isClicked = false;
                            btnMain.Update(mouse);
                        }
                        break;
                    }
            }
        }

        void UpdatePlayer(GameTime gameTime)
        {
            player.Update(gameTime);

            // Get Thumbstick Controls
            player.Position.X += currentGamePadState.ThumbSticks.Left.X * playerMoveSpeed;
            player.Position.Y -= currentGamePadState.ThumbSticks.Left.Y * playerMoveSpeed;

            // Use the Keyboard / Dpad
            if (currentKeyboardState.IsKeyDown(Keys.A) ||
            currentGamePadState.DPad.Left == ButtonState.Pressed)
            {
                player.Position.X -= playerMoveSpeed;
                rotation += 0.1f;
            }
            if (currentKeyboardState.IsKeyDown(Keys.D) ||
            currentGamePadState.DPad.Right == ButtonState.Pressed)
            {
                player.Position.X += playerMoveSpeed;
                rotation -= 0.1f;
            }
            if (currentKeyboardState.IsKeyDown(Keys.W) ||
            currentGamePadState.DPad.Up == ButtonState.Pressed)
            {
                player.Position.Y -= playerMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.S) ||
            currentGamePadState.DPad.Down == ButtonState.Pressed)
            {
                player.Position.Y += playerMoveSpeed;
            }

            //detecting a single mouse click or trigger pull to fire the weapon
            if (prevState.LeftButton == ButtonState.Released &&
                currState.LeftButton == ButtonState.Pressed ||
            (currentGamePadState.Triggers.Right.Equals(ButtonState.Pressed)))
            {
                if (ammo >= 1)
                {
                    // this is the concrete command used in the command pattern
                    // Add the projectile, but add it to the front and center of the player
                    AddProjectile(player.Position + new Vector2(player.Width / 2, 0));

                    // play the gunshot sound
                    gunshot.Play();

                    // decrease player ammo count
                    ammo--;

                    // increase shot counter
                    totalshots++;

                    // decrease player magazine count
                    magazine--;

                }
                else if (ammo == 0)
                {
                    click.Play();
                }
            }

            if (currentKeyboardState.IsKeyDown(Keys.R) ||
                currentGamePadState.Buttons.X == ButtonState.Pressed)
            {

                //reload.Play();
                if (ammo >= 7)
                {
                    magazine = 7;
                }
                else if (ammo >= 1 && ammo <= 7)
                {
                    magazine = ammo;
                }
            }

            // Make sure that the player does not go out of bounds
            player.Position.X = MathHelper.Clamp(player.Position.X, 0, GraphicsDevice.Viewport.Width - player.Width);
            player.Position.Y = MathHelper.Clamp(player.Position.Y, 0, GraphicsDevice.Viewport.Height - player.Height);


            // reset score if player health goes to zero
            if (player.Health <= 0)
            {
                CurrentGameState = GameState.GameOver;
            }
        }

        private void UpdateEnemies(GameTime gameTime)
        {
            // Spawn a new enemy enemy every 1.5 seconds
            if (gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime)
            {
                previousSpawnTime = gameTime.TotalGameTime;

                // Add an Enemy
                AddEnemy();
            }

            int choice = 0;

            // Update the Enemies
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                enemies[i].Update(gameTime);
                if (enemies[i].Active == false)
                {
                    enemies.RemoveAt(i);
                    kills++;
                    choice = random.Next(1, 2);
                    if (choice == 1 || ammo <= 2)
                    {
                        // randomly give the player between 1 and 4 bullets after each kill
                        ammo += random.Next(1, 4);
                    }
                    else if (choice == 2)
                    {
                        continue;
                    }
                    try
                    {
                        score += 10;
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        
                    }
                }
                else if (gameTime.TotalGameTime - previousFireTime > fireTime)
                {
                    //reset firetime
                    previousFireTime = gameTime.TotalGameTime;

                    AddEnemyProjectile(enemies[i].enemyPosition + new Vector2(enemies[i].Width / 2, 0), (player.Position));
                }
            }
        }

        private void UpdateProjectiles(GameTime gameTime)
        {
            // Update the Projectiles
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                projectiles[i].Update();

                if (projectiles[i].Active == false || (Vector2.Distance(projectiles[i].projectilePosition, player.Position) > 1000))
                {
                    projectiles.RemoveAt(i);
                }
            }
            // Update the Projectiles
            for (int i = enemyProjectiles.Count - 1; i >= 0; i--)
            {
                enemyProjectiles[i].Update();

                if (enemyProjectiles[i].Active == false || (Vector2.Distance(enemyProjectiles[i].projectilePosition, player.Position) > 1000))
                {
                    enemyProjectiles.RemoveAt(i);
                }
            }

        }

        private void UpdateCollision()
        {
            // Use the Rectangle's built-in intersect function to 
            // determine if two objects are overlapping

            Rectangle rectangle1;

            //enemy collision
            Rectangle rectangle2;

            // Only create the rectangle once for the player
            rectangle1 = new Rectangle((int)player.Position.X,
            (int)player.Position.Y,
            player.Width,
            player.Height);

            // enemyProjectile vs Player Collision
            for (int a = 0; a < enemyProjectiles.Count; a++)
            {
                // Create the rectangles we need to determine if we collided with each other
                rectangle2 = new Rectangle((int)enemyProjectiles[a].projectilePosition.X -
                enemyProjectiles[a].Width / 2, (int)enemyProjectiles[a].projectilePosition.Y -
                enemyProjectiles[a].Height / 2, enemyProjectiles[a].Width / 2, enemyProjectiles[a].Height / 2);

                // Determine if the player collides with the enemy projectile
                if (rectangle1.Intersects(rectangle2))
                {
                    enemyProjectiles[a].Active = false;
                    player.Health -= enemyProjectiles[a].Damage;
                }
            }

            // Do the collision between the player and the enemies
            for (int i = 0; i < enemies.Count; i++)
            {
                rectangle2 = new Rectangle((int)enemies[i].enemyPosition.X,
                (int)enemies[i].enemyPosition.Y,
                enemies[i].Width,
                enemies[i].Height);

                // Determine if the two objects collided with each
                // other
                if (rectangle1.Intersects(rectangle2))
                {
                    // Subtract the health from the player based on
                    // the enemy damage
                    player.Health -= enemies[i].Damage;

                    // Since the enemy collided with the player
                    // destroy it
                    enemies[i].Health = 0;
                    meleekills++;

                    // If the player health is less than zero we died
                    if (player.Health <= 0)
                        player.Active = false;
                }

                // Projectile vs Enemy Collision
                for (int a = 0; a < projectiles.Count; a++)
                {
                    for (int j = 0; j < enemies.Count; j++)
                    {
                        // Create the rectangles we need to determine if we collided with each other
                        rectangle1 = new Rectangle((int)projectiles[a].projectilePosition.X -
                        projectiles[a].Width / 2, (int)projectiles[a].projectilePosition.Y -
                        projectiles[a].Height / 2, projectiles[a].Width, projectiles[a].Height);

                        rectangle2 = new Rectangle((int)enemies[j].enemyPosition.X - enemies[j].Width / 2,
                        (int)enemies[j].enemyPosition.Y - enemies[j].Height / 2,
                        enemies[j].Width, enemies[j].Height);

                        // Determine if the two objects collided with each other
                        if (rectangle1.Intersects(rectangle2))
                        {
                            enemies[j].Health -= projectiles[a].Damage;
                            projectiles[a].Active = false;
                            enemyHit+=1;
                        }
                    }
                }

            }
        }
        #endregion



        #region Draw
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // set default background colour to black
            GraphicsDevice.Clear(Color.Black);

            // Start drawing
            spriteBatch.Begin();

            switch (CurrentGameState)
            {
                case GameState.MainMenu:
                    {
                        // draw the main menu screen
                        spriteBatch.Draw(Content.Load<Texture2D>("MainMenu"),
                            new Rectangle(0, 0, screenWidth, screenHeight),
                            Color.White);

                        btnPlay.Draw(spriteBatch);
                        btnInstructions.Draw(spriteBatch);
                        btnHighscores.Draw(spriteBatch);

                        //Stop drawing
                        spriteBatch.End();

                        break;
                    }

                case GameState.Username:
                    {
                        // draw the main menu screen
                        spriteBatch.Draw(Content.Load<Texture2D>("MainMenu"),
                            new Rectangle(0, 0, screenWidth, screenHeight),
                            Color.White);

                        btnFinished.Draw(spriteBatch);
                        btnMain.Draw(spriteBatch);

                        //Stop drawing
                        spriteBatch.End();

                        break;
                    }

                case GameState.Highscore:
                    {
                        spriteBatch.Draw(Content.Load<Texture2D>("MainMenu"), new Rectangle(0, 0, screenWidth, screenHeight), Color.White);

                        openFile.openFile();

                        btnMain.Draw(spriteBatch);

                        //Stop drawing
                        spriteBatch.End();

                        break;
                    }

                case GameState.Instructions:
                    {
                        // draw the main menu screen
                        spriteBatch.Draw(Content.Load<Texture2D>("MainMenu"), new Rectangle(0, 0, screenWidth, screenHeight), Color.White);

                        spriteBatch.DrawString(font, "Movement Controls:\n\nUp: W\n\nLeft: A\n\nDown: S\n\nRight: D\n\nShoot: Left Click\n\nReload: R",
                        new Vector2(500,
                        250), Color.White);

                        btnMain.Draw(spriteBatch);

                        //Stop drawing
                        spriteBatch.End();

                        break;
                    }

                case GameState.Playing:
                    {
                        //draw the background image
                        Rectangle screenRect = new Rectangle(0, 0, screenWidth, screenHeight);
                        spriteBatch.Draw(mainBackground, screenRect, Color.White);

                        // draw the sandstorm
                        sand.Draw(spriteBatch);

                        // Draw the Player
                        player.Draw(spriteBatch);

                        // Draw the Enemies
                        for (int i = 0; i < enemies.Count; i++)
                        {
                            enemies[i].Draw(spriteBatch);
                        }

                        // Draw the Projectiles
                        for (int i = 0; i < projectiles.Count; i++)
                        {
                            projectiles[i].Draw(spriteBatch);
                        }

                        // draw the enemy projectiles
                        for (int i = 0; i < enemyProjectiles.Count; i++)
                        {
                            enemyProjectiles[i].Draw(spriteBatch);
                        }

                        // Draw the score
                        spriteBatch.DrawString(font, "Score: " + score,
                            new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X,
                                GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);

                        // Draw the player health
                        spriteBatch.DrawString(font, "Health: " + player.Health,
                            new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X,
                            GraphicsDevice.Viewport.TitleSafeArea.Y + 30), Color.White);

                        // Draw the player health
                        spriteBatch.DrawString(font, "Ammo: " + ammo,
                            new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X,
                            GraphicsDevice.Viewport.TitleSafeArea.Y + 60), Color.White);

                        base.Draw(gameTime);

                        //Stop drawing
                        spriteBatch.End();

                        break;
                    }
                case GameState.GameOver:
                    {
                        spriteBatch.Draw(Content.Load<Texture2D>("GameOver"), new Rectangle(0, 0, screenWidth, screenHeight), Color.White);

                        //Stop drawing
                        spriteBatch.End();

                        break;
                    }
            }
        }
        #endregion



        #region AddContent
        private void AddEnemy()
        {
            // Create the animation object
            Animation enemyAnimation = new Animation();

            if (enemies.Count < 4)
            {
                // Initialize the animation with the correct animation information
                enemyAnimation.Initialize(enemyTexture, Vector2.Zero, 47, 40, 6, 4, Color.White, 1f, true);

                // Randomly generate the position of the enemy
                Vector2 position = new Vector2(GraphicsDevice.Viewport.Width + enemyTexture.Width / 2, random.Next(100, GraphicsDevice.Viewport.Height - 100));

                // Create an enemy
                Enemy enemy = new Enemy();

                if (kills == 3)
                {
                    // Initialize the enemy with the value that its the last one in the current wave
                    enemy.Initialize(enemyAnimation, position, playerPosition, graphics, lastEnemy = true);
                }
                else
                {
                    // Initialize the enemy with the value that its not the last one in the wave
                    enemy.Initialize(enemyAnimation, position, playerPosition, graphics, lastEnemy = false);
                }

                // Add the enemy to the active enemies list
                enemies.Add(enemy);
            }
            else { }
        }

        // this method executes the commandbase used in the command pattern
        private void AddProjectile(Vector2 position)
        {
            Projectile projectile = new Projectile();
            projectile.Initialize(GraphicsDevice.Viewport, projectileTexture, position, mousePosition);
            projectiles.Add(projectile);
        }

        // this method executes the commandbase used in the command pattern
        private void AddEnemyProjectile(Vector2 enemyPosition, Vector2 playerPosition)
        {
            enemyProjectile enemyProjectile = new enemyProjectile();
            enemyProjectile.Initialize(GraphicsDevice.Viewport, projectileTexture, enemyPosition, playerPosition, mousePosition);
            enemyProjectiles.Add(enemyProjectile);
        }
        #endregion



    }
}
