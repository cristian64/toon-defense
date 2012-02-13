﻿using System;
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
    public class Projectile : Object
    {
        public int Damage;
        public Spaceship Target;
        public Vector3 Speed;
        public float Acceleration;
        public float Friction;
        public bool NoTarget;

        public Projectile(Game game, Camera camera)
            :base(game, camera)
        {
            Speed = Vector3.Up * 15;
            Acceleration = 110;
            Friction = 100;
        }

        protected override void LoadContent()
        {
            model = Game.Content.Load<Model>("models\\missile");
            texture = Game.Content.Load<Texture2D>("models\\missiletexture");
            effect = Game.Content.Load<Effect>("effects\\Toon").Clone();
            effect.Parameters["Texture"].SetValue(texture);
            effect.Parameters["LineThickness"].SetValue(0.0f);
            effect.Parameters["DiffuseIntensity"].SetValue(4.0f);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            ProjectileTrailParticleSystem.LastInstance.AddParticle(Position, Vector3.Zero);
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

                        speedDirection = Vector3.Normalize(Speed);
                        Rotation.Y = (float)Math.Atan2(-speedDirection.Z, speedDirection.X);
                        Rotation.Z = (float)Math.Atan2(speedDirection.Y, Math.Abs(speedDirection.X));
                    }
                    else
                    {
                        ExplosionParticleSystem.LastInstance.AddParticle(Position, Vector3.Zero);
                        ExplosionParticleSystem.LastInstance.AddParticle(Position, Vector3.Zero);
                        ExplosionParticleSystem.LastInstance.AddParticle(Position, Vector3.Zero);
                        ExplosionParticleSystem.LastInstance.AddParticle(Position, Vector3.Zero);
                        ExplosionParticleSystem.LastInstance.AddParticle(Position, Vector3.Zero);
                        ExplosionParticleSystem.LastInstance.AddParticle(Position, Vector3.Zero);
                        ExplosionParticleSystem.LastInstance.AddParticle(Position, Vector3.Zero);
                        ExplosionParticleSystem.LastInstance.AddParticle(Position, Vector3.Zero);
                        Target.Health -= Damage;
                        NoTarget = true;
                    }
                }
            }
            else
            {
                ExplosionParticleSystem.LastInstance.AddParticle(Position, Vector3.Zero);
                ExplosionParticleSystem.LastInstance.AddParticle(Position, Vector3.Zero);
                ExplosionParticleSystem.LastInstance.AddParticle(Position, Vector3.Zero);
                ExplosionParticleSystem.LastInstance.AddParticle(Position, Vector3.Zero);
                NoTarget = true;
            }
            base.Update(gameTime);
        }

        public Matrix GetRotationMatrix(Vector3 source, Vector3 target)
        {
            float dot = Vector3.Dot(source, target);
            if (!float.IsNaN(dot))
            {
                float angle = (float)Math.Acos(dot);
                if (!float.IsNaN(angle))
                {
                    Vector3 cross = Vector3.Cross(source, target);
                    cross.Normalize();
                    Matrix rotation = Matrix.CreateFromAxisAngle(cross, angle);
                    return rotation;
                }
            }
            return Matrix.Identity;
        }

        public Matrix LookAt(Vector3 position, Vector3 lookat)
        {
            Matrix rotation = new Matrix();

            rotation.Forward = Vector3.Normalize(lookat - position);
            rotation.Right = Vector3.Normalize(Vector3.Cross(rotation.Forward, Vector3.Up));
            rotation.Up = Vector3.Normalize(Vector3.Cross(rotation.Right, rotation.Forward));

            return rotation;
        }

        public override void Draw(GameTime gameTime)
        {
            Matrix world = Matrix.CreateScale(Scale) * Matrix.CreateRotationZ(Rotation.Z) * Matrix.CreateRotationY(Rotation.Y) * Matrix.CreateTranslation(Position);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effect;
                    effect.Parameters["World"].SetValue(world);
                    effect.Parameters["View"].SetValue(Camera.View);
                    effect.Parameters["Projection"].SetValue(Camera.Projection);
                    effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(world)));
                }
                mesh.Draw();
            }

            base.Draw(gameTime);
        }
    }
}
