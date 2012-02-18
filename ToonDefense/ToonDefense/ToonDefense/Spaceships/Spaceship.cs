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

namespace ToonDefense.Spaceships
{
    public class Spaceship : Object
    {
        SpriteBatch spriteBatch;
        Texture2D blackTexture;
        Texture2D greenTexture;
        public List<Vector3> Destinations;
        public float Speed;
        public int Health;
        public int InitialHealth;
        public int Reward;

        public Spaceship(Game game, Camera camera)
            :base(game, camera)
        {
            Destinations = new List<Vector3>();
            Speed = 3;
            InitialHealth = 100;
            Reward = 100;
            Position.Y = 2;
        }

        public override void Initialize()
        {
            Health = InitialHealth;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = GameplayComponent.LastInstance.SpriteBatch;
            blackTexture = new Texture2D(GraphicsDevice, 1, 1, true, SurfaceFormat.Color);
            Color[] colors = new Color[1];
            colors[0] = Color.Black;
            blackTexture.SetData(colors);
            greenTexture = new Texture2D(GraphicsDevice, 1, 1, true, SurfaceFormat.Color);
            colors[0] = Color.Lime;
            greenTexture.SetData(colors);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (Destinations.Count > 0 && gameTime.ElapsedGameTime.TotalMilliseconds > 0)
            {
                Vector2 position = new Vector2(Position.X, Position.Z);
                Vector2 destination = new Vector2(Destinations[0].X, Destinations[0].Z);
                Vector2 direction = destination - position;

                if (direction.LengthSquared() > Speed * Speed * gameTime.ElapsedGameTime.TotalSeconds * gameTime.ElapsedGameTime.TotalSeconds)
                {
                    direction.Normalize();
                    Position.X += direction.X * Speed * (float)(gameTime.ElapsedGameTime.TotalSeconds);
                    Position.Z += direction.Y * Speed * (float)(gameTime.ElapsedGameTime.TotalSeconds);
                    Rotation.Y = (float)Math.Atan2(-direction.Y, direction.X);
                }
                else
                {
                    Position.X = destination.X;
                    Position.Z = destination.Y;
                    Destinations.RemoveAt(0);
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Vector3 position = Camera.RayFromWorldToScreen(Position);
            if (position.Z < 1)
            {
                position.Y -= 20;
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                spriteBatch.Draw(blackTexture, new Rectangle((int)position.X, (int)position.Y, 32, 4), Color.White);
                spriteBatch.Draw(greenTexture, new Rectangle((int)position.X + 1, (int)position.Y + 1, (int)(30 * (Health / (float)InitialHealth)), 2), Color.White);
                spriteBatch.End();
            }
            base.Draw(gameTime);
        }
    }
}
