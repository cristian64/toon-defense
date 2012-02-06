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
        public Effect shadowEffect;

        public Object(Game game, Camera camera)
            : base(game)
        {
            Camera = camera;
            Position = new Vector3();
            Scale = new Vector3();
            Rotation = new Vector3();
        }

        protected override void LoadContent()
        {
            shadowModel = Game.Content.Load<Model>("models\\shadow");
            shadowTexture = Game.Content.Load<Texture2D>("models\\shadowtexture");
            shadowEffect = Game.Content.Load<Effect>("effects\\Toon").Clone();

            shadowEffect.Parameters["Texture"].SetValue(shadowTexture);
            shadowEffect.Parameters["LineThickness"].SetValue(0.0f);

            base.LoadContent();
        }

        public void DrawShadow(Vector3 position, float scale)
        {
            Matrix world = Matrix.CreateScale(scale * 0.5f) * Matrix.CreateTranslation(position);
            foreach (ModelMesh mesh in shadowModel.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = shadowEffect;
                    shadowEffect.Parameters["World"].SetValue(world);
                    shadowEffect.Parameters["View"].SetValue(Camera.View);
                    shadowEffect.Parameters["Projection"].SetValue(Camera.Projection);
                    shadowEffect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(world)));
                }
                mesh.Draw();
            }
        }
    }
}
