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
using ToonDefense.Projectiles;

namespace ToonDefense.Towers
{
    public class Flamethower : Tower
    {
        SoundEffectInstance sound;
        public Flamethower(Game game, Camera camera)
            : base(game, camera)
        {
            Name = "Flamethrower";
            Sight = 3;
            Damage = 1;
            Delay = 32;
            Price = 2000;
        }

        protected override void LoadContent()
        {
            model = Game.Content.Load<Model>("models\\flamethrower");
            texture = Game.Content.Load<Texture2D>("models\\flamethrowertexture");
            effect = Game.Content.Load<Effect>("effects\\Toon").Clone();
            effect.Parameters["Texture"].SetValue(texture);
            sound = Game.Content.Load<SoundEffect>("sounds\\flame").CreateInstance();

            Position.Y = Height / 2.0f;

            base.LoadContent();
        }

        public override void Shoot()
        {
            Target.Health -= Damage;
            base.Shoot();
        }

        public override void Update(GameTime gameTime)
        {
            if (Target != null)
            {
                Vector3 targetDirection = Target.Destinations[0] - Target.Position;
                targetDirection.Y = 0;
                targetDirection.Normalize();
                Vector3 direction = Vector3.Normalize(Target.Position + Target.Speed * targetDirection - Position);
                Rotation.Y = (float)Math.Atan2(-direction.Z, direction.X);
                FireParticleSystem.LastInstance.AddParticle(Position + direction, 3 * direction); 
            }

            if (Target != null && sound.State != SoundState.Playing)
                sound.Play();
            if (Target == null && sound.State == SoundState.Playing)
                sound.Stop();

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Matrix world = Matrix.CreateScale(Scale) * Matrix.CreateRotationY(Rotation.Y) * Matrix.CreateTranslation(Position);
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
