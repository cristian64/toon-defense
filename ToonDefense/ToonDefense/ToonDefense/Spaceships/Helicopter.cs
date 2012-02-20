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
    public class Helicopter : Spaceship
    {
        public Helicopter(Game game, Camera camera)
            : base(game, camera)
        {
            Speed = 1.7f;
            InitialHealth = 700;
            Reward = 50;
            Position.Y = 1.2f;
        }

        protected override void LoadContent()
        {
            model = Game.Content.Load<Model>("models\\helicopter");
            texture = Game.Content.Load<Texture2D>("models\\helicoptertexture");
            effect = Game.Content.Load<Effect>("effects\\Toon").Clone();
            effect.Parameters["Texture"].SetValue(texture);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            propellerRotation += MathHelper.Pi / 0.13f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            base.Update(gameTime);
        }

        float propellerRotation = 0;
        public override void Draw(GameTime gameTime)
        {
            DrawShadow();
            Matrix world = Matrix.CreateScale(Scale) * Matrix.CreateRotationZ(-0.2f) * Matrix.CreateRotationY(Rotation.Y) * Matrix.CreateTranslation(Position);
            foreach (ModelMeshPart part in model.Meshes[2].MeshParts)
            {
                part.Effect = effect;
                effect.Parameters["World"].SetValue(world);
                effect.Parameters["View"].SetValue(Camera.View);
                effect.Parameters["Projection"].SetValue(Camera.Projection);
                effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(world)));
            }
            model.Meshes[2].Draw();

            foreach (ModelMeshPart part in model.Meshes[1].MeshParts)
            {
                part.Effect = effect;
                Matrix world2 = Matrix.CreateRotationY(propellerRotation) * Matrix.CreateTranslation(new Vector3(0.05f, 0.17f, 0)) * world;
                effect.Parameters["World"].SetValue(world2);
                effect.Parameters["View"].SetValue(Camera.View);
                effect.Parameters["Projection"].SetValue(Camera.Projection);
                effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(world2)));
            }
            model.Meshes[1].Draw();

            foreach (ModelMeshPart part in model.Meshes[0].MeshParts)
            {
                part.Effect = effect;
                Matrix world3 = Matrix.CreateRotationZ(propellerRotation) * Matrix.CreateTranslation(new Vector3(-1, 0.13f, -0.07f)) * world;
                effect.Parameters["World"].SetValue(world3);
                effect.Parameters["View"].SetValue(Camera.View);
                effect.Parameters["Projection"].SetValue(Camera.Projection);
                effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(world3)));
            }
            model.Meshes[0].Draw();

            base.Draw(gameTime);
        }
    }
}
