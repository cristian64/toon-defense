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
        Texture2D button;
        Texture2D laserCannonThumbnail;
        LaserCannon laserCannon;
        Texture2D forceExtractorThumbnail;
        TeslaCoil forceExtractor;
        Texture2D flameThrowerThumbnail;
        Flamethower flamethrower;
        Texture2D missileLauncherThumbnail;
        MissileLauncher missileLauncher;
        Texture2D plasmaGeneratorThumbnail;
        PlasmaGenerator plasmaGenerator;

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
            renderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            button = Game.Content.Load<Texture2D>("images\\button");
            laserCannonThumbnail = Game.Content.Load<Texture2D>("images\\lasercannonthumbnail");
            laserCannon = new LaserCannon(Game, camera);
            forceExtractorThumbnail = Game.Content.Load<Texture2D>("images\\forceextractorthumbnail");
            forceExtractor = new TeslaCoil(Game, camera);
            flameThrowerThumbnail = Game.Content.Load<Texture2D>("images\\flamethrowerthumbnail");
            flamethrower = new Flamethower(Game, camera);
            missileLauncherThumbnail = Game.Content.Load<Texture2D>("images\\missilelauncherthumbnail");
            missileLauncher = new MissileLauncher(Game, camera);
            plasmaGeneratorThumbnail = Game.Content.Load<Texture2D>("images\\plasmageneratorthumbnail");
            plasmaGenerator = new PlasmaGenerator(Game, camera);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (renderTarget.Width != GraphicsDevice.Viewport.Width || renderTarget.Height != GraphicsDevice.Viewport.Height)
                renderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);

            if (Mouse.GetState().LeftButton == ButtonState.Pressed && tower == null && GameplayComponent.LastInstance.SpeedLevel != SpeedLevel.PAUSED)
            {
                if (isOnButton(Mouse.GetState(), 1) && laserCannon.Price <= player.Money)
                    tower = new LaserCannon(Game, camera);
                else if (isOnButton(Mouse.GetState(), 2) && forceExtractor.Price <= player.Money)
                    tower = new TeslaCoil(Game, camera);
                else if (isOnButton(Mouse.GetState(), 3) && flamethrower.Price <= player.Money)
                    tower = new Flamethower(Game, camera);
                else if (isOnButton(Mouse.GetState(), 4) && missileLauncher.Price <= player.Money)
                    tower = new MissileLauncher(Game, camera);
                else if (isOnButton(Mouse.GetState(), 5) && plasmaGenerator.Price <= player.Money)
                    tower = new PlasmaGenerator(Game, camera);

                if (tower != null)
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
                    world.IsBuildable(new Vector3(tower.Position.X + tower.Width / 2.0f, 0, tower.Position.Z)) &&
                    world.IsBuildable(new Vector3(tower.Position.X, 0, tower.Position.Z - tower.Depth / 2.0f)) &&
                    world.IsBuildable(new Vector3(tower.Position.X - tower.Width / 2.0f, 0, tower.Position.Z)) &&
                    world.IsBuildable(new Vector3(tower.Position.X, 0, tower.Position.Z - tower.Depth / 2.0f)) &&
                    world.IsBuildable(new Vector3(tower.Position.X, 0, tower.Position.Z)))
                {
                    GameplayComponent.LastInstance.SpawnComponents.Add(tower);
                    world.SetNotBuildable(new Vector3(tower.Position.X - tower.Width / 2.0f, 0, tower.Position.Z - tower.Depth / 2.0f), new Vector3(tower.Position.X + tower.Width / 2.0f, 0, tower.Position.Z + tower.Depth / 2.0f));
                    player.Money -= tower.Price;
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
                PrimitiveDrawings.DrawSphere(Game, GraphicsDevice, camera, tower.Position, tower.Sight);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);

                if (world.IsBuildable(new Vector3(tower.Position.X + tower.Width / 2.0f, 0, tower.Position.Z + tower.Depth / 2.0f)) &&
                    world.IsBuildable(new Vector3(tower.Position.X + tower.Width / 2.0f, 0, tower.Position.Z - tower.Depth / 2.0f)) &&
                    world.IsBuildable(new Vector3(tower.Position.X - tower.Width / 2.0f, 0, tower.Position.Z + tower.Depth / 2.0f)) &&
                    world.IsBuildable(new Vector3(tower.Position.X - tower.Width / 2.0f, 0, tower.Position.Z - tower.Depth / 2.0f)) &&
                    world.IsBuildable(new Vector3(tower.Position.X + tower.Width / 2.0f, 0, tower.Position.Z)) &&
                    world.IsBuildable(new Vector3(tower.Position.X, 0, tower.Position.Z - tower.Depth / 2.0f)) &&
                    world.IsBuildable(new Vector3(tower.Position.X - tower.Width / 2.0f, 0, tower.Position.Z)) &&
                    world.IsBuildable(new Vector3(tower.Position.X, 0, tower.Position.Z - tower.Depth / 2.0f)) &&
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

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            drawButton(laserCannonThumbnail, 1, laserCannon.Price);
            drawButton(forceExtractorThumbnail, 2, forceExtractor.Price);
            drawButton(flameThrowerThumbnail, 3, flamethrower.Price);
            drawButton(missileLauncherThumbnail, 4, missileLauncher.Price);
            drawButton(plasmaGeneratorThumbnail, 5, plasmaGenerator.Price);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void drawButton(Texture2D thumbnail, int shift, int price)
        {
            Color color = (price <= player.Money) ? Color.White : Color.White * 0.3f;
            Vector2 position = new Vector2(GraphicsDevice.Viewport.Width - button.Width * shift, GraphicsDevice.Viewport.Height - button.Height);
            spriteBatch.Draw(button, position, color);
            spriteBatch.Draw(button, position, color);
            spriteBatch.Draw(thumbnail, new Vector2(GraphicsDevice.Viewport.Width - button.Width * shift, GraphicsDevice.Viewport.Height - button.Height), color);
        }

        private bool isOnButton(MouseState mouseState, int shift)
        {
            Vector2 position = new Vector2(GraphicsDevice.Viewport.Width - button.Width * shift, GraphicsDevice.Viewport.Height - button.Height);
            position.X += (80 - 64) / 2;
            position.Y += (80 - 64) / 2;

            return position.X <= mouseState.X && mouseState.X <= position.X + 64 &&
                position.Y <= mouseState.Y && mouseState.Y <= position.Y + 64;
        }
    }
}