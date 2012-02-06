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
    public class Explorer : Object
    {
        SpriteBatch spriteBatch;
        Model model;
        Texture2D texture;
        Effect celShader;
        Effect outlineShader;
        Texture2D celMap;

        RenderTarget2D renderTarget;

        public Explorer(Game game, Camera camera)
            : base(game, camera)
        {
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            model = Game.Content.Load<Model>("models\\explorer");
            texture = Game.Content.Load<Texture2D>("models\\explorertexture");
            celShader = Game.Content.Load<Effect>("effects\\CelShader").Clone();
            outlineShader = Game.Content.Load<Effect>("effects\\OutlineShader").Clone();
            celMap = Game.Content.Load<Texture2D>("effects\\celMap");

            celShader.Parameters["LightDirection"].SetValue(new Vector4(0, 0, 1.0f, 1.0f));
            celShader.Parameters["ColorMap"].SetValue(texture);
            celShader.Parameters["CelMap"].SetValue(celMap);

            outlineShader.Parameters["Thickness"].SetValue(1.5f);
            outlineShader.Parameters["Threshold"].SetValue(0.5f);
            outlineShader.Parameters["ScreenSize"].SetValue(new Vector2(GraphicsDevice.Viewport.Bounds.Width, GraphicsDevice.Viewport.Bounds.Height));

            renderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);

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
            /*GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            GraphicsDevice.Clear(Color.Wheat * 0.0f);*/

            Matrix world = Matrix.CreateScale(0.10f) * Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateRotationY(-MathHelper.PiOver2) * Matrix.CreateRotationY(Rotation.Y) * Matrix.CreateTranslation(Position);
            celShader.Parameters["Projection"].SetValue(Camera.Projection);
            celShader.Parameters["View"].SetValue(Camera.View);
            celShader.Parameters["World"].SetValue(world);
            celShader.Parameters["InverseWorld"].SetValue(Matrix.Invert(world));
            model.Meshes[0].MeshParts[0].Effect = celShader;
            model.Meshes[0].Draw();

            /*GraphicsDevice.SetRenderTarget(null);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, outlineShader);
            spriteBatch.Draw(renderTarget, Vector2.Zero, Color.White);
            spriteBatch.End();*/

            base.Draw(gameTime);
        }
    }
}
