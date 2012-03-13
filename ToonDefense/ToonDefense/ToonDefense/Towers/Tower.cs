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
using ToonDefense.Spaceships;

namespace ToonDefense.Towers
{
    public class Tower : Object
    {
        public Spaceship Target;
        public float Sight;
        public int Damage;
        public float Delay;
        public float DelayCounter;
        public int Price;
        public int UpgradePrice;
        public bool Upgraded;
        SoundEffect upgradingSound;
        public bool Sold;

        public Tower(Game game, Camera camera)
            : base(game, camera)
        {
            Target = null;
            Sight = 5;
            Damage = 10;
            Delay = 1000;
            Price = 100;
            UpgradePrice = 100;
            Upgraded = false;
            Sold = false;
        }

        protected override void LoadContent()
        {
            upgradingSound = Game.Content.Load<SoundEffect>("sounds\\upgrade");
            base.LoadContent();
        }

        public virtual void Upgrade()
        {
            texture = Game.Content.Load<Texture2D>("models\\gold");
            effect.Parameters["Texture"].SetValue(texture);
            upgradingSound.Play();
            Upgraded = true;
        }

        public override void Draw(GameTime gameTime)
        {
            if (ToonDefense.Debug)
                if (Target != null)
                    PrimitiveDrawings.DrawLine(GraphicsDevice, Camera, Position, Target.Position, Color.Violet);
            if (Selected)
            {
                Vector3 position = Position;
                position.Y = 0;
                PrimitiveDrawings.DrawSphere(Game, GraphicsDevice, Camera, position, Sight);
            }
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            if (Target == null || Target.Health <= 0 || (Target.Position - Position).LengthSquared() > Sight * Sight)
            {
                FindTarget();
            }

            DelayCounter += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (DelayCounter > Delay)
            {
                DelayCounter -= Delay;
                if (Target != null && Target.Health > 0)
                {
                    Shoot();
                }
            }

            base.Update(gameTime);
        }

        public virtual void Shoot()
        {
        }

        public void FindTarget()
        {
            Target = null;
            float minDistance = float.MaxValue;
            foreach (DrawableGameComponent i in GameplayComponent.LastInstance.DrawableComponents)
            {
                Spaceship spaceship = i as Spaceship;
                if (spaceship != null)
                {
                    float distance = (spaceship.Position - Position).LengthSquared();
                    if ((spaceship.Position - Position).LengthSquared() <= Sight * Sight && distance < minDistance && spaceship.Health > 0)
                    {
                        Target = spaceship;
                        minDistance = distance;
                    }
                }
            }
        }

        public override string[] ToText()
        {
            String aux1 = "";
            aux1 += "Sight:  \n";
            aux1 += "Damage:  \n";
            aux1 += "Delay:  \n";
            aux1 += "Price:  ";
            String aux2 = "";
            aux2 += Sight + "\n";
            aux2 += Damage + "\n";
            aux2 += Delay + "\n";
            aux2 += Price;
            string[] aux = new string[2];
            aux[0] = aux1;
            aux[1] = aux2;
            return aux;
        }
    }
}
