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
        public Flamethower(Game game, Camera camera)
            : base(game, camera)
        {
            Sight = 5;
            Damage = 1;
            Delay = 1;
            Price = 100;
        }

        protected override void LoadContent()
        {
            model = Game.Content.Load<Model>("models\\flamethrower");
            texture = Game.Content.Load<Texture2D>("models\\flamethrowertexture");
            effect = Game.Content.Load<Effect>("effects\\Toon").Clone();
            effect.Parameters["Texture"].SetValue(texture);

            Position.Y = Height / 2.0f;

            base.LoadContent();
        }

        public override void Shoot()
        {
            Vector3 targetDirection =  Vector3.Normalize(Target.Destinations[0] - Target.Position);
            targetDirection.Y = 0;
            Vector3 direction = Vector3.Normalize(Target.Position + 2 * targetDirection - Position);
            FireParticleSystem.LastInstance.AddParticle(Position + direction, 3 * direction);
            base.Shoot();
        }

        public override void Update(GameTime gameTime)
        {
            if (Target != null)
            {
                Vector3 targetDirection = Vector3.Normalize(Target.Destinations[0] - Target.Position);
                targetDirection.Y = 0;
                Vector3 direction = Vector3.Normalize(Target.Position + 2 * targetDirection - Position);
                Rotation.Y = (float)Math.Atan2(-direction.Z, direction.X);
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Vector3 shadowPosition = Position;
            shadowPosition.Y = 0;
            DrawShadow(shadowPosition, 1.8f);
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
