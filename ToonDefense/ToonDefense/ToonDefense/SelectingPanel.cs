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
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            MouseState currentMouseState = Mouse.GetState();

            Spaceship spaceship = selected as Spaceship;
            if (spaceship != null && (spaceship.Health < 0 || spaceship.Destinations.Count == 0))
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
            /*Spaceship spaceship = selected as Spaceship;
            if (spaceship != null)
            {

            }

            Tower tower = selected as Tower;*/
            if (selected != null)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                spriteBatch.DrawString(font, selected.ToString(), new Vector2(1300, 0), Color.White);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}