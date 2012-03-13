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
using ToonDefense.Spaceships;

namespace ToonDefense
{
    public class SelectingPanel : DrawableGameComponent
    {
        Object selected;
        Camera camera;
        World world;
        SpriteBatch spriteBatch;
        SpriteFont font;
        SpriteFont font2;
        MouseState prevMouseState;

        public SelectingPanel(Game game, Camera camera, World world)
            : base(game)
        {
            this.camera = camera;
            this.world = world;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Game.Content.Load<SpriteFont>("fonts\\selection");
            font2 = Game.Content.Load<SpriteFont>("fonts\\selectionname");
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            MouseState currentMouseState = Mouse.GetState();

            Spaceship spaceship = selected as Spaceship;
            if (spaceship != null && (spaceship.Health <= 0 || spaceship.Destinations.Count == 0))
                selected = null;

            int grabbingAmount = Math.Abs(currentMouseState.X - camera.GrabbingX) + Math.Abs(currentMouseState.Y - camera.GrabbingY);

            if (currentMouseState.LeftButton == ButtonState.Released && prevMouseState.LeftButton == ButtonState.Pressed && (!camera.Grabbing || grabbingAmount < 5))
            {
                if (selected != null)
                    selected.Selected = false;
                selected = null;

                float distance = float.MaxValue;
                Object candidate = null;
                Ray ray = camera.RayFromScreenToWorldRay(currentMouseState.X, currentMouseState.Y);
                foreach (DrawableGameComponent i in GameplayComponent.LastInstance.DrawableComponents)
                {
                    if (i as Spaceship != null || i as Tower != null)
                    {
                        Object selectable = i as Object;
                        float? intersection = selectable.Intersects(ray);
                        if (intersection != null && distance > intersection)
                            candidate = selectable;
                    }
                }

                selected = candidate;
                if (selected != null)
                    selected.Selected = true;
            }

            prevMouseState = currentMouseState;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (selected != null)
            {
                string[] text = selected.ToText();
                Vector2 position = new Vector2(GraphicsDevice.PresentationParameters.BackBufferWidth - Math.Max(font2.MeasureString(selected.Name).X, font.MeasureString(text[0]).X + font.MeasureString(text[1]).X), 0);
                position += new Vector2(-20, 10);
                Vector2 position2 = position + new Vector2(0, font2.MeasureString(selected.Name).Y);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                spriteBatch.DrawString(font2, selected.Name, position + Vector2.UnitX, Color.Black);
                spriteBatch.DrawString(font2, selected.Name, position - Vector2.UnitX, Color.Black);
                spriteBatch.DrawString(font2, selected.Name, position + Vector2.UnitY, Color.Black);
                spriteBatch.DrawString(font2, selected.Name, position - Vector2.UnitY, Color.Black);
                spriteBatch.DrawString(font2, selected.Name, position, selected as Spaceship != null ? Color.Red : Color.Lime);
                spriteBatch.DrawString(font, text[0], position2 + Vector2.UnitX, Color.Black);
                spriteBatch.DrawString(font, text[0], position2 - Vector2.UnitX, Color.Black);
                spriteBatch.DrawString(font, text[0], position2 + Vector2.UnitY, Color.Black);
                spriteBatch.DrawString(font, text[0], position2 - Vector2.UnitY, Color.Black);
                spriteBatch.DrawString(font, text[0], position2, Color.White);
                position2 += new Vector2(font.MeasureString(text[0]).X, 0);
                spriteBatch.DrawString(font, text[1], position2 + Vector2.UnitX, Color.Black);
                spriteBatch.DrawString(font, text[1], position2 - Vector2.UnitX, Color.Black);
                spriteBatch.DrawString(font, text[1], position2 + Vector2.UnitY, Color.Black);
                spriteBatch.DrawString(font, text[1], position2 - Vector2.UnitY, Color.Black);
                spriteBatch.DrawString(font, text[1], position2, Color.White);
                spriteBatch.End();

                Tower tower = selected as Tower;
                if (tower != null)
                {

                }
            }

            base.Draw(gameTime);
        }
    }
}