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

namespace ToonDefense.Spaceships
{
    public class DeltaDart : Spaceship
    {
        public DeltaDart(Game game, Camera camera)
            : base(game, camera)
        {
            Speed = 6;
            InitialHealth = 600;
            Reward = 300;
            Position.Y = 0.75f;
        }

        protected override void LoadContent()
        {
            model = Game.Content.Load<Model>("models\\deltadart");
            texture = Game.Content.Load<Texture2D>("models\\deltadarttexture");
            effect = Game.Content.Load<Effect>("effects\\Toon").Clone();
            effect.Parameters["Texture"].SetValue(texture);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            Vector3 direction = Destinations[0] - Position;
            direction.Y = 0;
            direction.Normalize();
            Vector3 normal = new Vector3(-direction.Z, 0, direction.X);
            
            WhiteTrailParticleSystem.LastInstance.AddParticle(Position + normal * 0.12f - direction * (Width / 2), Vector3.Zero);
            WhiteTrailParticleSystem.LastInstance.AddParticle(Position - normal * 0.12f - direction * (Width / 2), Vector3.Zero);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            DrawShadow();
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
