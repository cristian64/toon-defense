using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ToonDefense
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Sphere : Object
    {
        public Sphere(Game game, Camera camera)
            : base(game, camera)
        {
        }

        protected override void LoadContent()
        {
            this.model = this.Game.Content.Load<Model>("models\\sphere");
            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            GraphicsDevice.BlendState = BlendState.Additive;
            Matrix world = Matrix.CreateScale(Scale) * Matrix.CreateScale(2) * Matrix.CreateRotationY(Rotation.Y) * Matrix.CreateTranslation(Position);

            foreach (ModelMesh mm in this.model.Meshes)
            {
                foreach (BasicEffect e in mm.Effects)
                {
                    e.Projection = Camera.Projection;
                    e.View = Camera.View;
                    e.World = world;
                    e.Alpha = 0.35f;
                }
                mm.Draw();
            }
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            base.Draw(gameTime);
        }
    }
}
