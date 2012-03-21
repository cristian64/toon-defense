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
using ToonDefense.ParticleSystem;

namespace ToonDefense.Towers
{
    public class LaserCannon : Tower
    {
        SoundEffectInstance sound;
        ParticleEmitter laserEmitter;
        Vector3 headPosition;

        public LaserCannon(Game game, Camera camera)
            : base(game, camera)
        {
            Name = "Laser Cannon";
            laserEmitter = new ParticleEmitter(LaserParticleSystem.LastInstance, 3, Position + Vector3.Up * 0.2f);
            Sight = 6;
            Damage = 30;
            Delay = 100;
            Price = 1000;
            UpgradePrice = 2000;
            Upgraded = false;
        }

        public override void Upgrade()
        {
            Sight = 7;
            Damage = 45;
            base.Upgrade();
        }

        protected override void LoadContent()
        {
            model = Game.Content.Load<Model>("models\\lasercannon");
            texture = Game.Content.Load<Texture2D>("models\\lasercannontexture");
            effect = Game.Content.Load<Effect>("effects\\Toon").Clone();
            effect.Parameters["Texture"].SetValue(texture);
            sound = Game.Content.Load<SoundEffect>("sounds\\laser").CreateInstance();

            Position.Y = Height / 2.0f;
            headPosition = Position + new Vector3(0, Height / 2 + 0.15f, 0);

            base.LoadContent();
        }

        public override void Shoot()
        {
            LaserParticleSystem.LastInstance.AddParticle(Position + Vector3.Up * 0.2f, Vector3.Up * 0.2f);
            LaserParticleSystem.LastInstance.AddParticle(Position + Vector3.Up * 0.2f, Vector3.Down * 0.2f);
            LaserParticleSystem.LastInstance.AddParticle(Position + Vector3.Up * 0.2f, Vector3.Right * 0.2f);
            LaserParticleSystem.LastInstance.AddParticle(Position + Vector3.Up * 0.2f, Vector3.Left * 0.2f);
            LaserParticleSystem.LastInstance.AddParticle(Position + Vector3.Up * 0.2f, Vector3.Forward * 0.2f);
            LaserParticleSystem.LastInstance.AddParticle(Position + Vector3.Up * 0.2f, Vector3.Backward * 0.2f);
            Target.Health -= Damage;
            base.Shoot();
        }

        public override void Update(GameTime gameTime)
        {
            laserEmitter.Update(gameTime, Position + Vector3.Up * 0.2f);

            if (Target != null && sound.State != SoundState.Playing && ToonDefense.SoundsCount == SoundsCount.LOTS)
                sound.Play();
            else if (Target == null && sound.State == SoundState.Playing)
                sound.Stop();

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (Target != null)
                if (Upgraded)
                    PrimitiveDrawings.DrawPyramid(GraphicsDevice, Camera, Position + Vector3.Up * 0.2f, Target.Position, Color.Yellow);
                else
                    PrimitiveDrawings.DrawPyramid(GraphicsDevice, Camera, Position + Vector3.Up * 0.2f, Target.Position, Color.Magenta);

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Matrix world = Matrix.CreateScale(Scale) * Matrix.CreateTranslation(Position);
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
