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
using ToonDefense.ParticleSystem;

namespace ToonDefense.Projectiles
{
    public class Plasma : Projectile
    {
        SoundEffect sound;
        public Plasma(Game game, Camera camera)
            :base(game, camera)
        {
            Speed = 2 * Vector3.Up;
            Acceleration = 15;
            Friction = 10;
        }

        protected override void LoadContent()
        {
            sound = Game.Content.Load<SoundEffect>("sounds\\plasma");
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            PlasmaParticleSystem.LastInstance.AddParticle(Position, Vector3.Zero);

            if (Target == null || Target.Health <= 0)
            {
                FindTarget();
            }

            if (Target != null && Target.Health > 0)
            {
                if (gameTime.ElapsedGameTime.TotalMilliseconds > 0)
                {
                    Vector3 direction = Vector3.Normalize(Target.Position - Position);
                    Speed += (direction * Acceleration) * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    Vector3 speedDirection = Vector3.Normalize(Speed);
                    Speed -= (speedDirection * Friction) * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    float distanceSquared = (Target.Position - Position).LengthSquared();
                    if (distanceSquared > 0.2f && distanceSquared > Speed.LengthSquared() * gameTime.ElapsedGameTime.TotalSeconds * gameTime.ElapsedGameTime.TotalSeconds)
                    {
                        Position += Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                    else
                    {
                        PlasmaExplosionParticleSystem.LastInstance.AddParticle(Position, Vector3.Zero);
                        PlasmaExplosionParticleSystem.LastInstance.AddParticle(Position, Vector3.Zero);
                        PlasmaExplosionParticleSystem.LastInstance.AddParticle(Position, Vector3.Zero);
                        PlasmaExplosionParticleSystem.LastInstance.AddParticle(Position, Vector3.Zero);
                        PlasmaExplosionParticleSystem.LastInstance.AddParticle(Position, Vector3.Zero);
                        PlasmaExplosionParticleSystem.LastInstance.AddParticle(Position, Vector3.Zero);
                        PlasmaExplosionParticleSystem.LastInstance.AddParticle(Position, Vector3.Zero);
                        PlasmaExplosionParticleSystem.LastInstance.AddParticle(Position, Vector3.Zero);
                        Target.Health -= Damage;
                        NoTarget = true;
                        if (ToonDefense.SoundsCount == SoundsCount.LOTS)
                            sound.Play();
                    }
                }
            }
            else
            {
                PlasmaExplosionParticleSystem.LastInstance.AddParticle(Position, Vector3.Zero);
                PlasmaExplosionParticleSystem.LastInstance.AddParticle(Position, Vector3.Zero);
                PlasmaExplosionParticleSystem.LastInstance.AddParticle(Position, Vector3.Zero);
                PlasmaExplosionParticleSystem.LastInstance.AddParticle(Position, Vector3.Zero);
                NoTarget = true;
            }
            base.Update(gameTime);
        }
    }
}
