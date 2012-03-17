using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ToonDefense
{
    public class Player : DrawableGameComponent
    {
        public static Player LastInstance = null;
        public int Lives = 100;
        public int Money = 100;
        public int Kills = 0;
        SpriteBatch spriteBatch;
        Texture2D heart;
        Texture2D skull;
        Texture2D money;
        SpriteFont font;
        SpriteFont font2;
        Texture2D redBackground;
        float alphaGameover = 0.0f;

        public Player(Game game)
            : base(game)
        {
            LastInstance = this;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            heart = Game.Content.Load<Texture2D>("images\\heart");
            skull = Game.Content.Load<Texture2D>("images\\skull");
            money = Game.Content.Load<Texture2D>("images\\money");
            font = Game.Content.Load<SpriteFont>("fonts\\hud");
            font2 = Game.Content.Load<SpriteFont>("fonts\\gameover");
            redBackground = new Texture2D(GraphicsDevice, 1, 1, true, SurfaceFormat.Color);
            redBackground.SetData(new Color[1] { Color.Black });

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.LeftControl) && Keyboard.GetState().IsKeyDown(Keys.RightControl))
                Money += 1000;

            if (Lives <= 0 && Game.Components.Count == 1)
            {
                Game.Components.Add(new FadeOutComponent(Game, 3000, 500, new MenuComponent(Game), new FadeInComponent(Game, 500, 500)));
            }

            if (Lives <= 0 && alphaGameover < 1)
                alphaGameover += 0.01f;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            float alpha = 0.6f;
            int size = 32;
            int padding = 5;
            int margin = 25;
            int topTextures = 3;
            int topFonts = 3;
            int livesSize = (int)font.MeasureString(Lives.ToString()).X;
            int moneySize = (int)font.MeasureString(Money.ToString()).X;
            int killsSize = (int)font.MeasureString(Kills.ToString()).X;

            spriteBatch.Draw(heart, new Rectangle(padding, topTextures, size, size), Color.White);
            spriteBatch.DrawString(font, Lives.ToString(), new Vector2(padding + size + padding, topFonts), Color.Black * alpha);
            spriteBatch.DrawString(font, Lives.ToString(), new Vector2(padding + size + padding - 1, topFonts - 1), new Color(1, Lives / 100.0f, Lives / 100.0f) * 0.9f);

            spriteBatch.Draw(money, new Rectangle(padding + size + padding + livesSize + margin, topTextures, size, size), Color.White);
            spriteBatch.DrawString(font, Money.ToString(), new Vector2(padding + size + padding + livesSize + margin + size + padding, topFonts), Color.Black * alpha);
            spriteBatch.DrawString(font, Money.ToString(), new Vector2(padding + size + padding + livesSize + margin + size + padding - 1, topFonts - 1), Color.White * 0.9f);

            spriteBatch.Draw(skull, new Rectangle(padding + size + padding + livesSize + margin + size + padding + moneySize + margin, topTextures, size, size), Color.White);
            spriteBatch.DrawString(font, Kills.ToString(), new Vector2(padding + size + padding + livesSize + margin + size + padding + moneySize + margin + size + padding, topFonts), Color.Black * alpha);
            spriteBatch.DrawString(font, Kills.ToString(), new Vector2(padding + size + padding + livesSize + margin + size + padding + moneySize + margin + size + padding - 1, topFonts - 1), Color.White * 0.9f);

            if (Lives <= 0)
            {
                spriteBatch.Draw(redBackground, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White * alphaGameover);
                spriteBatch.DrawString(font2, "Game Over", new Vector2(GraphicsDevice.Viewport.Width / 2 - font2.MeasureString("Game Over").X / 2, GraphicsDevice.Viewport.Height / 2 - font2.MeasureString("Game Over").Y / 2), Color.White * alphaGameover);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
