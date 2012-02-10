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
    public class Explorer : Spaceship
    {
        public Explorer(Game game, Camera camera)
            : base(game, camera)
        {
        }

        protected override void LoadContent()
        {
            model = Game.Content.Load<Model>("models\\explorer");
            texture = Game.Content.Load<Texture2D>("models\\explorertexture");
            effect = Game.Content.Load<Effect>("effects\\Toon").Clone();
            effect.Parameters["Texture"].SetValue(texture);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            WhiteTrailParticleSystem.LastInstance.AddParticle(Position, Vector3.Zero);
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                Position.Z -= 0.1f;
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                Position.X -= 0.1f;
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                Position.X += 0.1f;
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                Position.Z += 0.1f;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Vector3 shadowPosition = Position;
            shadowPosition.Y = 0;
            DrawShadow(shadowPosition, 1);
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
