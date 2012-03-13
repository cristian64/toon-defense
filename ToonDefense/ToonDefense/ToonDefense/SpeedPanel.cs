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
using ToonDefense.Towers;

namespace ToonDefense
{
    public class SpeedPanel : DrawableGameComponent
    {
        public static SpeedPanel LastInstance = null;
        SpriteBatch spriteBatch;
        Texture2D pauseButton;
        Texture2D pauseButtonOn;
        Texture2D playButton;
        Texture2D playButtonOn;
        Texture2D soundButton;
        Texture2D soundButtonOn;
        MouseState prevMouseState;
        int size;

        public SpeedPanel(Game game)
            : base(game)
        {
            LastInstance = this;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            pauseButton = Game.Content.Load<Texture2D>("images\\pause");
            pauseButtonOn = Game.Content.Load<Texture2D>("images\\pauseon");
            playButton = Game.Content.Load<Texture2D>("images\\play");
            playButtonOn = Game.Content.Load<Texture2D>("images\\playon");
            soundButton = Game.Content.Load<Texture2D>("images\\sound");
            soundButtonOn = Game.Content.Load<Texture2D>("images\\soundon");
            size = 32;
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            MouseState currentMouseState = Mouse.GetState();

            if (currentMouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released)
            {
                if (GraphicsDevice.Viewport.Height - size <= currentMouseState.Y && currentMouseState.Y <= GraphicsDevice.Viewport.Height)
                {
                    if (0 <= currentMouseState.X && currentMouseState.X <= size)
                    {
                        if (SoundEffect.MasterVolume == 0)
                            SoundEffect.MasterVolume = 1;
                        else
                            SoundEffect.MasterVolume = 0;
                    }
                    else if (size + 1 <= currentMouseState.X && currentMouseState.X <= size * 2)
                    {
                        if (GameplayComponent.LastInstance.SpeedLevel != SpeedLevel.PAUSED)
                            GameplayComponent.LastInstance.SpeedLevel = SpeedLevel.PAUSED;
                        else
                            GameplayComponent.LastInstance.SpeedLevel = SpeedLevel.NORMAL;
                    }
                    else if (size * 2 + 1 <= currentMouseState.X && currentMouseState.X <= size * 3)
                    {
                        if (GameplayComponent.LastInstance.SpeedLevel != SpeedLevel.FAST)
                            GameplayComponent.LastInstance.SpeedLevel = SpeedLevel.FAST;
                        else
                            GameplayComponent.LastInstance.SpeedLevel = SpeedLevel.NORMAL;
                    }

                    if (GameplayComponent.LastInstance.SpeedLevel == SpeedLevel.FAST)
                        GameplayComponent.LastInstance.Ost.Pitch = 1.0f;
                    else
                        GameplayComponent.LastInstance.Ost.Pitch = 0.0f;

                    if (GameplayComponent.LastInstance.SpeedLevel == SpeedLevel.PAUSED)
                        GameplayComponent.LastInstance.Ost.Pause();
                    else
                        GameplayComponent.LastInstance.Ost.Resume();
                }
            }

            prevMouseState = currentMouseState;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            if (GameplayComponent.LastInstance.SpeedLevel == SpeedLevel.NORMAL)
            {
                spriteBatch.Draw(pauseButton, new Rectangle(size, GraphicsDevice.Viewport.Height - size, size, size), Color.White);
                spriteBatch.Draw(playButton, new Rectangle(size * 2, GraphicsDevice.Viewport.Height - size, size, size), Color.White);
            }
            if (GameplayComponent.LastInstance.SpeedLevel == SpeedLevel.PAUSED)
            {
                spriteBatch.Draw(pauseButtonOn, new Rectangle(size, GraphicsDevice.Viewport.Height - size, size, size), Color.White);
                spriteBatch.Draw(playButton, new Rectangle(size * 2, GraphicsDevice.Viewport.Height - size, size, size), Color.White);
            }
            if (GameplayComponent.LastInstance.SpeedLevel == SpeedLevel.FAST)
            {
                spriteBatch.Draw(pauseButton, new Rectangle(size, GraphicsDevice.Viewport.Height - size, size, size), Color.White);
                spriteBatch.Draw(playButtonOn, new Rectangle(size * 2, GraphicsDevice.Viewport.Height - size, size, size), Color.White);
            }
            if (SoundEffect.MasterVolume == 0)
                spriteBatch.Draw(soundButton, new Rectangle(0, GraphicsDevice.Viewport.Height - size, size, size), Color.White);
            else
                spriteBatch.Draw(soundButtonOn, new Rectangle(0, GraphicsDevice.Viewport.Height - size, size, size), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public bool IsOnButtons(int x, int y)
        {
            Rectangle rectangle = new Rectangle(0, GraphicsDevice.Viewport.Height - size, size * 3, size);
            return rectangle.X <= x && x <= rectangle.X + rectangle.Width &&
                rectangle.Y <= y && y <= rectangle.Y + rectangle.Height;
        }
    }
}