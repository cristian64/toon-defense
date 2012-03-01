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

namespace ToonDefense
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ToonDefense : Microsoft.Xna.Framework.Game
    {
        public static bool Debug = false;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState prevKeyboardState;

        public ToonDefense()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreparingDeviceSettings += preparingDeviceSettings;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = (int)(0.8 * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width);
            graphics.PreferredBackBufferHeight = (int)(0.8 * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            //graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            //graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            //graphics.IsFullScreen = true;
            graphics.PreferMultiSampling = true;
            this.Window.Title = "Toon Defense";
        }

        private void preparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
        } 

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Components.Add(new IntroComponent(this));
            Components.Add(new FadeInComponent(this, 1000, 1000));
            Components.Add(new FadeOutComponent(this, 10000, 1000, new MenuComponent(this), new FadeInComponent(this, 1000, 1000)));
            //Components.Add(new GameplayComponent(this));
            //Components.Add(new FadeInComponent(this, 0, 300));
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
            base.LoadContent();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();

            // Allows the game to exit
            if (currentKeyboardState.IsKeyDown(Keys.Escape) && prevKeyboardState.IsKeyUp(Keys.Escape))
                Components.Add(new FadeOutComponent(this, 0, 500));

            // Allows the game to change size of the screen
            if (currentKeyboardState.IsKeyDown(Keys.F11) && prevKeyboardState.IsKeyUp(Keys.F11))
            {
                if (!graphics.IsFullScreen)
                {
                    graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                    graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                }
                else
                {
                    graphics.PreferredBackBufferWidth = (int)(0.8 * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width);
                    graphics.PreferredBackBufferHeight = (int)(0.8 * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
                }
                graphics.IsFullScreen = !graphics.IsFullScreen;
                graphics.ApplyChanges();
            }

            // Allows the game to change antialiasing
            if (currentKeyboardState.IsKeyDown(Keys.F12) && prevKeyboardState.IsKeyUp(Keys.F12))
            {
                graphics.PreferMultiSampling = !graphics.PreferMultiSampling;
                graphics.ApplyChanges();
            }
            // Allows the game to show debug features
            if (currentKeyboardState.IsKeyDown(Keys.F1) && prevKeyboardState.IsKeyUp(Keys.F1))
                Debug = !Debug;

            prevKeyboardState = currentKeyboardState;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
