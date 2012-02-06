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
    public class BattleCruiser : Object
    {
        Model model;
        Texture2D texture;
        Effect effect;

        public BattleCruiser(Game game, Camera camera)
            : base(game, camera)
        {
        }

        protected override void LoadContent()
        {
            model = Game.Content.Load<Model>("models\\Ship");
            texture = model.Meshes[0].Effects[0].Parameters["Texture"].GetValueTexture2D();// Game.Content.Load<Texture2D>("models\\ShipDiffuse");
            effect = Game.Content.Load<Effect>("effects\\Toon").Clone();
            effect.Parameters["Texture"].SetValue(texture);
            effect.Parameters["LineThickness"].SetValue(15);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            this.Rotation.Y += MathHelper.Pi / 10 * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                Position.Y++;
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                Position.X--;
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                Position.X++;
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                Position.Y--;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Matrix world = Matrix.CreateScale(0.004f) * Matrix.CreateRotationY(Rotation.Y) * Matrix.CreateTranslation(Position);
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
