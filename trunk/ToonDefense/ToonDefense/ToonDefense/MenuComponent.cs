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
        Texture2D map1button;
        Texture2D map2button;
        Texture2D map3button;
        Vector2 map1buttonPosition;
        Vector2 map2buttonPosition;
        Vector2 map3buttonPosition;

        MouseState prevMouseState;
        SpriteBatch spriteBatch;

        bool created;

        public MenuComponent(Game game)
            : base(game)
        {
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            controlsTexture = Game.Content.Load<Texture2D>("images\\controls");
            map1button = Game.Content.Load<Texture2D>("maps\\map1button");
            map2button = Game.Content.Load<Texture2D>("maps\\map2button");
            map3button = Game.Content.Load<Texture2D>("maps\\map3button");
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            MouseState currentMouseState = Mouse.GetState();

            if (!created && currentMouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released)
            {
                if (onButton(map1buttonPosition))
                {
                    Game.Components.Add(new FadeOutComponent(Game, 0, 500, new GameplayComponent(Game, "map1"), new FadeInComponent(Game, 500, 500)));
                    created = true;
                }
                else if (onButton(map2buttonPosition))
                {
                    Game.Components.Add(new FadeOutComponent(Game, 0, 500, new GameplayComponent(Game, "map2"), new FadeInComponent(Game, 500, 500)));
                    created = true;
                }
                else if (onButton(map3buttonPosition))
                {
                    Game.Components.Add(new FadeOutComponent(Game, 0, 500, new GameplayComponent(Game, "map3"), new FadeInComponent(Game, 500, 500)));
                    created = true;
                }
            }

            map1buttonPosition = new Vector2(GraphicsDevice.PresentationParameters.BackBufferWidth / 2 - map1button.Width / 2 - map1button.Width - 25, GraphicsDevice.PresentationParameters.BackBufferHeight / 2 - map1button.Height / 2.0f);
            map2buttonPosition = new Vector2(GraphicsDevice.PresentationParameters.BackBufferWidth / 2 - map1button.Width / 2, GraphicsDevice.PresentationParameters.BackBufferHeight / 2 - map1button.Height / 2.0f);
            map3buttonPosition = new Vector2(GraphicsDevice.PresentationParameters.BackBufferWidth / 2 - map1button.Width / 2 + map1button.Width + 25, GraphicsDevice.PresentationParameters.BackBufferHeight / 2 - map1button.Height / 2.0f);

            prevMouseState = currentMouseState;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            SpriteFont font = Game.Content.Load<SpriteFont>("fonts\\title");
            Vector2 position = new Vector2(GraphicsDevice.PresentationParameters.BackBufferWidth / 2, map1buttonPosition.Y);
            position -= font.MeasureString("Click on a map to start the game") / 2;
            position -= new Vector2(0, font.MeasureString("Click on a map to start the game").Y);
            spriteBatch.DrawString(font, "Click on a map to start the game", position, Color.White);
            spriteBatch.Draw(controlsTexture, new Vector2(0, GraphicsDevice.PresentationParameters.BackBufferHeight - controlsTexture.Height), Color.White);
            drawMapButtons();
            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void drawMapButtons()
        {
            spriteBatch.Draw(map1button, map1buttonPosition, onButton(map1buttonPosition) ? Color.White : Color.White * 0.7f);
            spriteBatch.Draw(map2button, map2buttonPosition, onButton(map2buttonPosition) ? Color.White : Color.White * 0.7f);
            spriteBatch.Draw(map3button, map3buttonPosition, onButton(map3buttonPosition) ? Color.White : Color.White * 0.7f);
        }

        private bool onButton(Vector2 mapButtonPosition)
        {
            mapButtonPosition += new Vector2(map1button.Width / 2);
            Vector2 mousePosition = new Vector2(prevMouseState.X, prevMouseState.Y);
            return Vector2.Distance(mapButtonPosition, mousePosition) < map1button.Width / 2;
        }
    }
}
