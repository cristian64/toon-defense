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

namespace ToonDefense
{
    public class Object : DrawableGameComponent
    {
        public Camera Camera;
        public Vector3 Position;
        public Vector3 Scale;
        public Vector3 Rotation;

        public Model shadowModel;
        public Texture2D shadowTexture;
        public BasicEffect shadowEffect;

        public Object(Game game, Camera camera)
            : base(game)
        {
            Camera = camera;
            Position = Vector3.Zero;
            Scale = Vector3.One;
            Rotation = Vector3.Zero;
        }

        protected override void LoadContent()
        {
            shadowModel = Game.Content.Load<Model>("models\\shadow");
            shadowTexture = Game.Content.Load<Texture2D>("models\\shadowtexture");
            shadowEffect = (BasicEffect)shadowModel.Meshes[0].Effects[0];
            shadowEffect.Texture = Game.Content.Load<Texture2D>("models\\shadowtexture");
            shadowEffect.TextureEnabled = true;

            base.LoadContent();
        }

        public void DrawShadow(Vector3 position, float scale)
        {
            Matrix world = Matrix.CreateScale(scale * 0.5f) * Matrix.CreateTranslation(position);
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            foreach (ModelMesh mesh in shadowModel.Meshes)
            {
                foreach (BasicEffect e in mesh.Effects)
                {
                    e.Projection = Camera.Projection;
                    e.View = Camera.View;
                    e.World = world;
                }
                mesh.Draw();
            }
            GraphicsDevice.BlendState = BlendState.Opaque;
        }
    }
}
