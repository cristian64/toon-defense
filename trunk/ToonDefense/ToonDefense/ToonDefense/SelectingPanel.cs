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
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            MouseState currentMouseState = Mouse.GetState();

            Spaceship spaceship = selected as Spaceship;
            if (spaceship != null && (spaceship.Health < 0 || spaceship.Destinations.Count == 0))
                selected = null;

            if (currentMouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released)
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
            Spaceship spaceship = selected as Spaceship;
            if (spaceship != null)
            {

            }

            Tower tower = selected as Tower;
            if (tower != null)
            {

            }

            base.Draw(gameTime);
        }
    }
}