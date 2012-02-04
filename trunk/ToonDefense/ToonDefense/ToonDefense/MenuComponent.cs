﻿using System;
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
        public MenuComponent(Game game)
            : base(game)
        {
        }

        bool pressed = false;
        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) && !pressed)
            {
                pressed = true;
                Game.Components.Add(new FadeOutComponent(Game, 0, 1000));
            }
            if (Keyboard.GetState().IsKeyUp(Keys.Enter))
                pressed = false;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = new SpriteBatch(GraphicsDevice);
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            SpriteFont font = Game.Content.Load<SpriteFont>("fonts\\title");
            Vector2 position = new Vector2(GraphicsDevice.PresentationParameters.BackBufferWidth / 2, GraphicsDevice.PresentationParameters.BackBufferHeight / 2);
            position -= font.MeasureString("Menu") / 2;
            spriteBatch.DrawString(font, "Menu", position, Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
