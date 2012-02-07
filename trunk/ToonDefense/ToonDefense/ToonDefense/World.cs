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
        String mapName;

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

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            this.Rotation.Y += MathHelper.Pi / 10 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Matrix world = Matrix.CreateScale(10f) * Matrix.CreateTranslation(Position);
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
    }
}
