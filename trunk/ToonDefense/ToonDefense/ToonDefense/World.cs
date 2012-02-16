﻿using System;
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
        BasicEffect basicEffect;
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
            basicEffect = (BasicEffect)model.Meshes[0].Effects[0];
            basicEffect.Texture = texture;
            basicEffect.TextureEnabled = true;
            basicEffect.DiffuseColor = new Vector3(1);

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
                basicEffect.Projection = Camera.Projection;
                basicEffect.View = Camera.View;
                basicEffect.World = world;
                mesh.Draw();
            }
            #if DEBUG
            for (int i = 0; i < Waypoints.Count - 1; i++)
                PrimitiveDrawings.DrawLine(GraphicsDevice, Camera, Waypoints[i], Waypoints[i + 1], Color.White);
            #endif

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
            int shift = (int)discrete.X + (int)discrete.Y * texture.Width;
            if (0 <= shift && shift < buildableAreas.Count())
                return buildableAreas[(int)discrete.X + (int)discrete.Y * texture.Width].Equals(Color.Black);
            else
                return false;
        }

        public void SetNotBuildable(Vector3 position1, Vector3 position2)
        {
            Vector2 discrete1 = WorldUnitsToTextureUnits(position1);
            Vector2 discrete2 = WorldUnitsToTextureUnits(position2);

            for (int i = (int)discrete1.X; i < discrete2.X; i++)
                for (int j = (int)discrete1.Y; j < discrete2.Y; j++)
                    buildableAreas[i + j * texture.Width] = Color.White;
        }
    }
}
