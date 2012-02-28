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
    public class IntroComponent : DrawableGameComponent
    {
        SpriteBatch spriteBatch;
        SpriteFont titleFont;
        SpriteFont subtitleFont;
        Texture2D logo;

        public IntroComponent(Game game)
            : base(game)
        {
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            titleFont = Game.Content.Load<SpriteFont>("fonts\\title");
            subtitleFont = Game.Content.Load<SpriteFont>("fonts\\subtitle");
            logo = Game.Content.Load<Texture2D>("images\\logo");
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            Vector2 position = new Vector2(GraphicsDevice.PresentationParameters.BackBufferWidth / 2, GraphicsDevice.PresentationParameters.BackBufferHeight / 2);
            position -= titleFont.MeasureString("Toon Defense") / 2;
            spriteBatch.DrawString(titleFont, "Toon Defense", position, Color.White);

            Vector2 position2 = new Vector2(GraphicsDevice.PresentationParameters.BackBufferWidth / 2, GraphicsDevice.PresentationParameters.BackBufferHeight / 2);
            position2 -= subtitleFont.MeasureString("Christian Aguilera") / 2 - new Vector2(0, titleFont.MeasureString("Toon Defense").Y - 10);
            spriteBatch.DrawString(subtitleFont, "Christian Aguilera", position2, Color.White);

            Vector2 position3 = new Vector2(GraphicsDevice.PresentationParameters.BackBufferWidth / 2, GraphicsDevice.PresentationParameters.BackBufferHeight / 2);
            position3 -= new Vector2(logo.Width, logo.Height) / 2 + new Vector2(0, titleFont.MeasureString("Toon Defense").Y) + new Vector2(0, logo.Height / 2);
            spriteBatch.Draw(logo, position3, Color.White);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
