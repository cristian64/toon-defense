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
    public class FadeInComponent : DrawableGameComponent
    {
        SpriteBatch spriteBatch;
        Texture2D texture;
        int delay;
        int duration;
        int milliseconds;

        public FadeInComponent(Game game, int delay = 0, int duration = 1000)
            : base(game)
        {
            this.delay = delay;
            this.duration = duration;
            milliseconds = 0;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            texture = new Texture2D(GraphicsDevice, 1, 1, true, SurfaceFormat.Color);
            Color[] colors = new Color[1];
            colors[0] = Color.Black;
            texture.SetData(colors);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (milliseconds > delay + duration)
            {
                Game.Components.Remove(this);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            milliseconds += gameTime.ElapsedGameTime.Milliseconds;
            Rectangle rectangle = new Rectangle(0, 0, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            if (milliseconds >= delay)
                spriteBatch.Draw(texture, rectangle, Color.Black * (1.0f - (milliseconds - delay) / (float)duration));
            else
                spriteBatch.Draw(texture, rectangle, Color.Black);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
