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
    public class GameplayComponent : DrawableGameComponent
    {
        public GameplayComponent(Game game)
            : base(game)
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            SpriteFont font = Game.Content.Load<SpriteFont>("fonts\\title");
            Vector2 position = new Vector2(GraphicsDevice.PresentationParameters.BackBufferWidth / 2, GraphicsDevice.PresentationParameters.BackBufferHeight / 2);
            position -= font.MeasureString("Gameplay") / 2;
            spriteBatch.DrawString(font, "Gameplay", position, Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
