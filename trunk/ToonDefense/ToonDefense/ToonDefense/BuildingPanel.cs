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
using ToonDefense.Towers;

namespace ToonDefense
{
    public class BuildingPanel : DrawableGameComponent
    {
        Tower tower;
        Camera camera;
        World world;
        Player player;
        SpriteBatch spriteBatch;
        RenderTarget2D renderTarget;

        public BuildingPanel(Game game, Camera camera, World world, Player player)
            : base(game)
        {
            this.camera = camera;
            this.world = world;
            this.player = player;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            renderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && tower == null)
            {
                tower = new LaserCannon(Game, camera);
                tower.Initialize();
            }

            if (Mouse.GetState().LeftButton == ButtonState.Pressed && tower != null)
            {
                Vector3 floorPosition = camera.RayFromScreenToFloor(Mouse.GetState().X, Mouse.GetState().Y);
                tower.Position.X = floorPosition.X;
                tower.Position.Z = floorPosition.Z;
            }
            else if (Mouse.GetState().LeftButton == ButtonState.Released && tower != null)
            {
                if (world.IsBuildable(new Vector3(tower.Position.X + tower.Width / 2.0f, 0, tower.Position.Z + tower.Depth / 2.0f)) &&
                    world.IsBuildable(new Vector3(tower.Position.X + tower.Width / 2.0f, 0, tower.Position.Z - tower.Depth / 2.0f)) &&
                    world.IsBuildable(new Vector3(tower.Position.X - tower.Width / 2.0f, 0, tower.Position.Z + tower.Depth / 2.0f)) &&
                    world.IsBuildable(new Vector3(tower.Position.X - tower.Width / 2.0f, 0, tower.Position.Z - tower.Depth / 2.0f)) &&
                    world.IsBuildable(new Vector3(tower.Position.X, 0, tower.Position.Z)))
                {
                    GameplayComponent.LastInstance.SpawnComponents.Add(tower);
                    world.SetNotBuildable(new Vector3(tower.Position.X - tower.Width / 2.0f, 0, tower.Position.Z - tower.Depth / 2.0f), new Vector3(tower.Position.X + tower.Width / 2.0f, 0, tower.Position.Z + tower.Depth / 2.0f));
                }
                tower = null;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (tower != null)
            {
                GraphicsDevice.SetRenderTarget(renderTarget);
                GraphicsDevice.Clear(Color.White * 0.0f);
                tower.Draw(gameTime);
                GraphicsDevice.SetRenderTarget(null);

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);

                if (world.IsBuildable(new Vector3(tower.Position.X + tower.Width / 2.0f, 0, tower.Position.Z + tower.Depth / 2.0f)) &&
                    world.IsBuildable(new Vector3(tower.Position.X + tower.Width / 2.0f, 0, tower.Position.Z - tower.Depth / 2.0f)) &&
                    world.IsBuildable(new Vector3(tower.Position.X - tower.Width / 2.0f, 0, tower.Position.Z + tower.Depth / 2.0f)) &&
                    world.IsBuildable(new Vector3(tower.Position.X - tower.Width / 2.0f, 0, tower.Position.Z - tower.Depth / 2.0f)) &&
                    world.IsBuildable(new Vector3(tower.Position.X, 0, tower.Position.Z)))
                {
                    spriteBatch.Draw(renderTarget, Vector2.Zero, Color.Lime);
                    spriteBatch.Draw(renderTarget, Vector2.Zero, Color.Lime);
                    spriteBatch.Draw(renderTarget, Vector2.Zero, Color.Lime);
                }
                else
                {
                    spriteBatch.Draw(renderTarget, Vector2.Zero, Color.Red);
                    spriteBatch.Draw(renderTarget, Vector2.Zero, Color.Red);
                    spriteBatch.Draw(renderTarget, Vector2.Zero, Color.Red);
                }
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
