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
    public class TeslaCoil : Tower
    {
        ParticleEmitter forceFieldEmitter;
        Vector3 headPosition;

        public TeslaCoil(Game game, Camera camera)
            : base(game, camera)
        {
            Sight = 5;
            Damage = 10;
            Delay = 400;
            Price = 300;
        }

        protected override void LoadContent()
        {
            model = Game.Content.Load<Model>("models\\teslacoil");
            texture = Game.Content.Load<Texture2D>("models\\teslacoiltexture");
            effect = Game.Content.Load<Effect>("effects\\Toon").Clone();
            effect.Parameters["Texture"].SetValue(texture);

            Position.Y = Height / 2.0f;
            forceFieldEmitter = new ParticleEmitter(ForceFieldParticleSystem.LastInstance, 1, Position);

            base.LoadContent();
        }

        public override void Shoot()
        {
            ForceFieldParticleSystem.LastInstance.AddParticle(Target.Position, Position - Target.Position);
            Target.Health -= Damage;
            base.Shoot();
        }

        public override void Update(GameTime gameTime)
        {
            headPosition = Position + new Vector3(0, Height / 2 + 0.15f, 0);
            forceFieldEmitter.Update(gameTime, headPosition);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            DrawShadow();
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
