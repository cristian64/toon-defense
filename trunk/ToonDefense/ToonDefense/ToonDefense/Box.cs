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
    public class Box : Object
    {
        private Model model;

        public Box(Game game, Camera camera)
            : base(game, camera)
        {
        }

        protected override void LoadContent()
        {
            this.model = this.Game.Content.Load<Model>("models\\box");
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            this.Rotation.Y += MathHelper.Pi / 10 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Matrix world = Matrix.CreateRotationY(Rotation.Y) * Matrix.CreateTranslation(Position);

            foreach (ModelMesh mm in this.model.Meshes)
            {
                foreach (BasicEffect e in mm.Effects)
                {
                    e.Projection = Camera.Projection;
                    e.View = Camera.View;
                    e.World = world;

                    e.LightingEnabled = true;
                    e.PreferPerPixelLighting = true;
                    e.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(-0.2f, -0.8f, -1));
                }
                mm.Draw();
            }

            base.Draw(gameTime);
        }
    }
}
