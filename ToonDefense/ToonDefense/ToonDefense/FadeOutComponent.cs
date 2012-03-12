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
    public class FadeOutComponent : DrawableGameComponent
    {
        SpriteBatch spriteBatch;
        Texture2D texture;
        int delay;
        int duration;
        int milliseconds;
        GameComponent nextComponent;
        GameComponent nextNextComponent;

        public FadeOutComponent(Game game, int delay = 5000, int duration = 1000, GameComponent nextComponent = null, GameComponent nextNextComponent = null)
            : base(game)
        {
            this.delay = delay;
            this.duration = duration;
            this.nextComponent = nextComponent;
            this.nextNextComponent = nextNextComponent;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            texture = new Texture2D(GraphicsDevice, 1, 1, true, SurfaceFormat.Color);
            Color[] colors = new Color[1];
            colors[0] = Color.Black;
            texture.SetData(colors);
        }

        public override void Update(GameTime gameTime)
        {
            if (milliseconds > delay + duration)
            {
                if (Game.Components.IndexOf(this) > 0)
                {
                    GameplayComponent gameplayComponent = Game.Components[Game.Components.IndexOf(this) - 1] as GameplayComponent;
                    if (gameplayComponent != null)
                        gameplayComponent.Ost.Stop();
                    Game.Components.RemoveAt(Game.Components.IndexOf(this) - 1);

                }
                Game.Components.Remove(this);
                if (nextComponent != null)
                    Game.Components.Add(nextComponent);
                if (nextNextComponent != null)
                    Game.Components.Add(nextNextComponent);
                if (Game.Components.Count == 0)
                    Game.Exit();
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            milliseconds += gameTime.ElapsedGameTime.Milliseconds;
            if (milliseconds > delay)
            {
                Rectangle rectangle = new Rectangle(0, 0, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                spriteBatch.Draw(texture, rectangle, Color.Black * ((milliseconds - delay) / (float)duration));
                spriteBatch.End();
            }
            base.Draw(gameTime);
        }
    }
}
