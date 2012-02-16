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
    public class Zeppelin : Spaceship
    {
        public Zeppelin(Game game, Camera camera)
            : base(game, camera)
        {
            Speed = 2;
            InitialHealth = 100;
            Reward = 200;
            Position.Y = 3;
        }

        protected override void LoadContent()
        {
            model = Game.Content.Load<Model>("models\\zeppelin");
            texture = Game.Content.Load<Texture2D>("models\\zeppelintexture");
            effect = Game.Content.Load<Effect>("effects\\Toon").Clone();
            effect.Parameters["Texture"].SetValue(texture);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            Rotation.X += MathHelper.Pi / 1.5f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Vector3 shadowPosition = Position;
            shadowPosition.Y = 0;
            DrawShadow(shadowPosition, 1);
            Matrix world = Matrix.CreateScale(Scale) * Matrix.CreateRotationX(Rotation.X) * Matrix.CreateRotationY(Rotation.Y) * Matrix.CreateTranslation(Position);
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
