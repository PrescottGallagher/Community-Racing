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
using System.Windows.Forms;

namespace Village_Racing_3
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Player player;
        TileMap tiles;
        Camera camera;
        int gameState;
        Vector2 LEPos;
        Texture2D selector;
        System.Windows.Forms.OpenFileDialog openFileDialog1;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
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
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            this.IsMouseVisible = true;
            graphics.PreferredBackBufferHeight = 768;
            graphics.PreferredBackBufferWidth = 1024;
            graphics.ApplyChanges();
            player = new Player();
            camera = new Camera(GraphicsDevice.Viewport);
            tiles = new TileMap(Content.Load<Texture2D>("tileSheet"), camera, Content.Load<Texture2D>("BlockSelector"));
            selector = Content.Load<Texture2D>("BlockSelector");
            tiles.toLevel();
            LEPos = new Vector2(300, 300);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                this.Exit();
            if (this.IsActive)
            {
                camera.LookAt(LEPos);

                if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
                    LEPos.X -= 5;
                if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
                    LEPos.X += 5;
                if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
                    LEPos.Y -= 5;
                if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
                    LEPos.Y += 5;

                if(Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl) && Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.S))
                    File.WriteAllBytes("level.dat", IntegerSerializer<int>.serialize(tiles.levelOne));

                tiles.Update();

                // TODO: Add your update logic here
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.GetViewMatrix(new Vector2(1.0f)));
            tiles.Draw(spriteBatch);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.GetViewMatrix(new Vector2(0.0f)));
            tiles.DrawForeGround(spriteBatch);
            spriteBatch.End();

            this.Window.Title = Mouse.GetState().X.ToString() + ", " + Mouse.GetState().Y.ToString();
            


            base.Draw(gameTime);
        }
    }
}
