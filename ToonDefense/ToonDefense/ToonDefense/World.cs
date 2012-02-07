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
    public class World : Object
    {
        Model model;
        Texture2D texture;
        BasicEffect effect;
        private String mapName;
        private List<Vector2> path;
        private int[,] terrain;

        public World(Game game, Camera camera, String mapName = "map1")
            : base(game, camera)
        {
            this.mapName = mapName;
        }

        protected override void LoadContent()
        {
            model = Game.Content.Load<Model>("models\\map");
            texture = Game.Content.Load<Texture2D>("maps\\" + mapName);
            effect = (BasicEffect)model.Meshes[0].Effects[0];
            effect.Texture = texture;
            effect.TextureEnabled = true;
            effect.DiffuseColor = new Vector3(1);
            LoadMap();

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Matrix world = Matrix.CreateScale(Scale) * Matrix.CreateTranslation(Position);
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            foreach (ModelMesh mesh in model.Meshes)
            {
                effect.Projection = Camera.Projection;
                effect.View = Camera.View;
                effect.World = world;
                mesh.Draw();
            }
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            base.Draw(gameTime);
        }

        public void LoadMap()
        {
            Scale.X = 16;
            Scale.Z = 16;
            path = new List<Vector2>();
            terrain = new int[texture.Width, texture.Height];
            //Scale, Path and Buildable must be filled.
            //TODO: load level info, map size and
        }

        public Vector2 WorldToTexture(Vector3 position)
        {
            Vector3 absPosition = position + Scale;
            Vector2 result;
            result.X = (float)Math.Floor(texture.Width * absPosition.X / Scale.X);
            result.Y = (float)Math.Floor(texture.Height * absPosition.Z / Scale.Z);
            return result;
        }

        public Vector3 IntersectionWithFloor(Vector3 lineStart, Vector3 lineEnd)
        {
            
        }

        public bool IsBuildable(Vector3 position)
        {
            return false;
        }
    }
}
