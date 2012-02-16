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
    public class ScienceVessel : Spaceship
    {
        public ScienceVessel(Game game, Camera camera)
            : base(game, camera)
        {
            Speed = 1;
            InitialHealth = 2000;
            Reward = 1000;
        }

        protected override void LoadContent()
        {
            model = Game.Content.Load<Model>("models\\sciencevessel");
            texture = Game.Content.Load<Texture2D>("models\\sciencevesseltexture");
            effect = Game.Content.Load<Effect>("effects\\Toon").Clone();
            effect.Parameters["Texture"].SetValue(texture);

            Position.Y = Height / 2.0f;

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            Rotation.Z -= MathHelper.Pi / 1 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Vector3 shadowPosition = Position;
            shadowPosition.Y = 0;
            DrawShadow(shadowPosition, 2);
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
