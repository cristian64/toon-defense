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
        private List<Vector3> waypoints;
        private int[,] terrain;

        public World(Game game, Camera camera, String mapName = "map1")
            : base(game, camera)
        {
            this.mapName = mapName;
        }

        protected override void LoadContent()
        {
            model = Game.Content.Load<Model>("models\\map");
            texture = Game.Content.Load<Texture2D>("maps\\" + mapName + "texture");
            effect = (BasicEffect)model.Meshes[0].Effects[0];
            effect.Texture = texture;
            effect.TextureEnabled = true;
            effect.DiffuseColor = new Vector3(1);
            Scale.X = 20;
            Scale.Z = 20;
            waypoints = new List<Vector3>();
            foreach (Vector2 i in Game.Content.Load<List<Vector2>>("maps\\" + mapName))
                waypoints.Add(TextureUnitsToWorldUnits(i));
            terrain = new int[texture.Width, texture.Height];

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
            for (int i = 0; i < waypoints.Count - 1; i++)
                PrimitiveDrawings.DrawLine(GraphicsDevice, Camera, waypoints[i], waypoints[i + 1], Color.White);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            base.Draw(gameTime);
        }

        public Vector2 WorldUnitsToTextureUnits(Vector3 position)
        {
            return new Vector2(
                (float)Math.Round(texture.Width * (position.X + Scale.X / 2) / Scale.X),
                (float)Math.Round(texture.Height * (position.Z + Scale.Z / 2) / Scale.Z));
        }

        public Vector3 TextureUnitsToWorldUnits(Vector2 position)
        {
            return new Vector3(
                Scale.X * position.X / (texture.Width - 1) - Scale.X / 2,
                0,
                Scale.Z * position.Y / (texture.Height - 1) - Scale.Z / 2);
        }

        public bool IsBuildable(Vector3 position)
        {
            return false;
        }
    }
}
