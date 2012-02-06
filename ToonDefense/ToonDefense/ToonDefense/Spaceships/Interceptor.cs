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
    public class Interceptor : Object
    {
        Model model;
        Texture2D texture;
        Effect effect;

        public Interceptor(Game game, Camera camera)
            : base(game, camera)
        {
        }

        protected override void LoadContent()
        {
            model = Game.Content.Load<Model>("models\\explorer");
            texture = Game.Content.Load<Texture2D>("models\\explorertexture");
            effect = Game.Content.Load<Effect>("effects\\Toon").Clone();

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

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            Matrix world = Matrix.CreateScale(0.10f) * Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateRotationY(-MathHelper.PiOver2) * Matrix.CreateRotationY(Rotation.Y) * Matrix.CreateTranslation(Position);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effect;
                    effect.Parameters["World"].SetValue(world);
                    effect.Parameters["View"].SetValue(Camera.View);
                    effect.Parameters["Projection"].SetValue(Camera.Projection);
                    effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(world)));
                    effect.Parameters["Texture"].SetValue(texture);

                }
                mesh.Draw();
            }

            base.Draw(gameTime);
        }
    }
}
