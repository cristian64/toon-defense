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
        public String MapName;
        public List<Vector3> Waypoints;
        Color[] buildableAreas;

        public World(Game game, Camera camera, String mapName = "map1")
            : base(game, camera)
        {
            this.MapName = mapName;
        }

        protected override void LoadContent()
        {
            model = Game.Content.Load<Model>("models\\map");
            texture = Game.Content.Load<Texture2D>("maps\\" + MapName + "texture");
            effect = (BasicEffect)model.Meshes[0].Effects[0];
            effect.Texture = texture;
            effect.TextureEnabled = true;
            effect.DiffuseColor = new Vector3(1);

            // Process XML file with waypoints and real size of the map.
            Scale.X = Game.Content.Load<List<Vector2>>("maps\\" + MapName).First().X;
            Scale.Z = Game.Content.Load<List<Vector2>>("maps\\" + MapName).First().Y;
            Game.Content.Load<List<Vector2>>("maps\\" + MapName).RemoveAt(0);
            Waypoints = new List<Vector3>();
            foreach (Vector2 i in Game.Content.Load<List<Vector2>>("maps\\" + MapName))
                Waypoints.Add(TextureUnitsToWorldUnits(i));

            // Process buildable areas from the binary image.
            Texture2D buildableTexture = Game.Content.Load<Texture2D>("maps\\" + MapName + "buildable");
            buildableAreas = new Color[buildableTexture.Width * buildableTexture.Height];
            buildableTexture.GetData<Color>(buildableAreas);

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
            for (int i = 0; i < Waypoints.Count - 1; i++)
                PrimitiveDrawings.DrawLine(GraphicsDevice, Camera, Waypoints[i], Waypoints[i + 1], Color.White);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            base.Draw(gameTime);
        }

        public Vector2 WorldUnitsToTextureUnits(Vector3 position)
        {
            return new Vector2(
                (float)Math.Round((texture.Width - 1) * (position.X + Scale.X / 2) / Scale.X),
                (float)Math.Round((texture.Height -1) * (position.Z + Scale.Z / 2) / Scale.Z));
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
            Vector2 discrete = WorldUnitsToTextureUnits(position);
            return buildableAreas[(int)discrete.X + (int)discrete.Y * texture.Width].Equals(Color.Black);
        }
    }
}
