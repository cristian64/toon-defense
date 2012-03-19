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
            lineColor = new float[4];
            lineColor[0] = 255 / 255.0f;
            lineColor[1] = 0 / 255.0f;
            lineColor[2] = 0 / 255.0f;
            lineColor[3] = 1.0f;

            Destinations = new List<Vector3>();
            Speed = 3;
            InitialHealth = 100;
            Reward = 100;
            Position.Y = 1.2f;
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

                double seconds = gameTime.ElapsedGameTime.TotalSeconds;
                if (direction.LengthSquared() > Speed * Speed * seconds * seconds)
                {
                    direction.Normalize();
                    Position.X += direction.X * Speed * (float)seconds;
                    Position.Z += direction.Y * Speed * (float)seconds;

                    double rotation = Math.Atan2(-direction.Y, direction.X);
                    double difference = (float)rotation - Rotation.Y;
                    if (Math.Abs(difference) > Math.Abs(Math.Abs(difference) - MathHelper.TwoPi))
                        if (difference < 0)
                            difference += MathHelper.TwoPi;
                        else
                            difference -= MathHelper.TwoPi;

                    Rotation.Y += 2 * (float)seconds * (float)difference;
                    if (Rotation.Y > MathHelper.TwoPi)
                        Rotation.Y -= MathHelper.TwoPi;
                    else if (Rotation.Y < -MathHelper.TwoPi)
                        Rotation.Y += MathHelper.TwoPi;
                }
                else
                {
                    direction.Normalize();
                    Position.X += direction.X * Speed * (float)seconds;
                    Position.Z += direction.Y * Speed * (float)seconds;
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
                position.Y -= 30;
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                spriteBatch.Draw(blackTexture, new Rectangle((int)position.X, (int)position.Y, 32, 4), Color.White);
                spriteBatch.Draw(greenTexture, new Rectangle((int)position.X + 1, (int)position.Y + 1, (int)(30 * (Health / (float)InitialHealth)), 2), Color.White);
                spriteBatch.End();
            }
            base.Draw(gameTime);
        }

        public override string[] ToText()
        {
            String aux1 = "";
            aux1 += "Health:  \n";
            aux1 += "Speed:  \n";
            aux1 += "Reward:  ";
            String aux2 = "";
            aux2 += Health + "/" + InitialHealth + "\n";
            aux2 += Speed + "\n";
            aux2 += Reward;
            string[] aux = new string[2];
            aux[0] = aux1;
            aux[1] = aux2;
            return aux;
        }
    }
}
