using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ToonDefense
{
    public class MenuComponent : DrawableGameComponent
    {
        Texture2D controlsTexture;
        MouseState prevMouseState;

        public MenuComponent(Game game)
            : base(game)
        {
        }

        protected override void LoadContent()
        {
            controlsTexture = Game.Content.Load<Texture2D>("images\\controls");
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            MouseState currentMouseState = Mouse.GetState();

            if (currentMouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released)
                Game.Components.Add(new FadeOutComponent(Game, 0, 1000, new GameplayComponent(Game), new FadeInComponent(Game, 1000, 1000)));

            prevMouseState = currentMouseState;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = new SpriteBatch(GraphicsDevice);
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            SpriteFont font = Game.Content.Load<SpriteFont>("fonts\\title");
            Vector2 position = new Vector2(GraphicsDevice.PresentationParameters.BackBufferWidth / 2, GraphicsDevice.PresentationParameters.BackBufferHeight / 2);
            position -= font.MeasureString("Click on a map to start the game") / 2;
            position.Y = 100;
            spriteBatch.DrawString(font, "Click on a map to start the game", position, Color.White);
            spriteBatch.Draw(controlsTexture, new Vector2(0, GraphicsDevice.PresentationParameters.BackBufferHeight - controlsTexture.Height), Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
