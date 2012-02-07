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
        public List<Vector2> Destinations;
        public float Speed;
        public int Health;
        public int Reward;

        public Spaceship(Game game, Camera camera)
            :base(game, camera)
        {
            Destinations = new List<Vector2>();
            Speed = 1;
            Health = 100;
            Reward = 100;
        }

        public override void Update(GameTime gameTime)
        {
            if (Destinations.Count > 0 && gameTime.ElapsedGameTime.TotalMilliseconds > 0)
            {
                Vector2 position = new Vector2(Position.X, Position.Z);
                Vector2 destination = Destinations[0];
                Vector2 direction = destination - position;

                if (direction.LengthSquared() > Speed * Speed / gameTime.ElapsedGameTime.TotalMilliseconds / gameTime.ElapsedGameTime.TotalMilliseconds)
                {
                    direction.Normalize();
                    Position.X += direction.X * Speed / (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    Position.Z += direction.Y * Speed / (float)gameTime.ElapsedGameTime.TotalMilliseconds;
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
    }
}
