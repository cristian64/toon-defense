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
        public static BuildingPanel LastInstance = null;

        public Tower Tower;
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
        SpriteFont font;

        public BuildingPanel(Game game, Camera camera, World world, Player player)
            : base(game)
        {
            this.camera = camera;
            this.world = world;
            this.player = player;

            LastInstance = this;
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
            font = Game.Content.Load<SpriteFont>("fonts\\price");
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (renderTarget.Width != GraphicsDevice.Viewport.Width || renderTarget.Height != GraphicsDevice.Viewport.Height)
                renderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);

            if (!camera.Grabbing && Mouse.GetState().LeftButton == ButtonState.Pressed && Tower == null && GameplayComponent.LastInstance.SpeedLevel != SpeedLevel.PAUSED)
            {
                if (isOnButton(Mouse.GetState(), 5) && forceExtractor.Price <= player.Money)
                    Tower = new TeslaCoil(Game, camera);
                else if (isOnButton(Mouse.GetState(), 4) && plasmaGenerator.Price <= player.Money)
                    Tower = new PlasmaGenerator(Game, camera);
                else if (isOnButton(Mouse.GetState(), 3) && missileLauncher.Price <= player.Money)
                    Tower = new MissileLauncher(Game, camera);
                else if (isOnButton(Mouse.GetState(), 2) && flamethrower.Price <= player.Money)
                    Tower = new Flamethower(Game, camera);
                else if (isOnButton(Mouse.GetState(), 1) && laserCannon.Price <= player.Money)
                    Tower = new LaserCannon(Game, camera);

                if (Tower != null)
                    Tower.Initialize();
            }

            if (Mouse.GetState().LeftButton == ButtonState.Pressed && Tower != null)
            {
                Vector3 floorPosition = camera.RayFromScreenToFloor(Mouse.GetState().X, Mouse.GetState().Y);
                Tower.Position.X = floorPosition.X;
                Tower.Position.Z = floorPosition.Z;
            }
            else if (Mouse.GetState().LeftButton == ButtonState.Released && Tower != null)
            {
                if (!isOnPanel(Mouse.GetState()) &&
                    world.IsBuildable(new Vector3(Tower.Position.X + Tower.Width / 2.0f, 0, Tower.Position.Z + Tower.Depth / 2.0f)) &&
                    world.IsBuildable(new Vector3(Tower.Position.X + Tower.Width / 2.0f, 0, Tower.Position.Z - Tower.Depth / 2.0f)) &&
                    world.IsBuildable(new Vector3(Tower.Position.X - Tower.Width / 2.0f, 0, Tower.Position.Z + Tower.Depth / 2.0f)) &&
                    world.IsBuildable(new Vector3(Tower.Position.X - Tower.Width / 2.0f, 0, Tower.Position.Z - Tower.Depth / 2.0f)) &&
                    world.IsBuildable(new Vector3(Tower.Position.X + Tower.Width / 2.0f, 0, Tower.Position.Z)) &&
                    world.IsBuildable(new Vector3(Tower.Position.X, 0, Tower.Position.Z - Tower.Depth / 2.0f)) &&
                    world.IsBuildable(new Vector3(Tower.Position.X - Tower.Width / 2.0f, 0, Tower.Position.Z)) &&
                    world.IsBuildable(new Vector3(Tower.Position.X, 0, Tower.Position.Z - Tower.Depth / 2.0f)) &&
                    world.IsBuildable(new Vector3(Tower.Position.X, 0, Tower.Position.Z)))
                {
                    GameplayComponent.LastInstance.SpawnComponents.Add(Tower);
                    world.SetNotBuildable(new Vector3(Tower.Position.X - Tower.Width / 2.0f, 0, Tower.Position.Z - Tower.Depth / 2.0f), new Vector3(Tower.Position.X + Tower.Width / 2.0f, 0, Tower.Position.Z + Tower.Depth / 2.0f));
                    player.Money -= Tower.Price;
                }
                Tower = null;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (Tower != null)
            {
                GraphicsDevice.SetRenderTarget(renderTarget);
                GraphicsDevice.Clear(Color.White * 0.0f);
                Tower.Draw(gameTime);
                GraphicsDevice.SetRenderTarget(null);
                Vector3 position = Tower.Position;
                position.Y = 0;
                PrimitiveDrawings.DrawSphere(Game, GraphicsDevice, camera, position, Tower.Sight);

                if (!isOnPanel(Mouse.GetState()) &&
                    world.IsBuildable(new Vector3(Tower.Position.X + Tower.Width / 2.0f, 0, Tower.Position.Z + Tower.Depth / 2.0f)) &&
                    world.IsBuildable(new Vector3(Tower.Position.X + Tower.Width / 2.0f, 0, Tower.Position.Z - Tower.Depth / 2.0f)) &&
                    world.IsBuildable(new Vector3(Tower.Position.X - Tower.Width / 2.0f, 0, Tower.Position.Z + Tower.Depth / 2.0f)) &&
                    world.IsBuildable(new Vector3(Tower.Position.X - Tower.Width / 2.0f, 0, Tower.Position.Z - Tower.Depth / 2.0f)) &&
                    world.IsBuildable(new Vector3(Tower.Position.X + Tower.Width / 2.0f, 0, Tower.Position.Z)) &&
                    world.IsBuildable(new Vector3(Tower.Position.X, 0, Tower.Position.Z - Tower.Depth / 2.0f)) &&
                    world.IsBuildable(new Vector3(Tower.Position.X - Tower.Width / 2.0f, 0, Tower.Position.Z)) &&
                    world.IsBuildable(new Vector3(Tower.Position.X, 0, Tower.Position.Z - Tower.Depth / 2.0f)) &&
                    world.IsBuildable(new Vector3(Tower.Position.X, 0, Tower.Position.Z)))
                {
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
                    spriteBatch.Draw(renderTarget, Vector2.Zero, Color.Lime);
                    spriteBatch.Draw(renderTarget, Vector2.Zero, Color.Lime);
                    spriteBatch.Draw(renderTarget, Vector2.Zero, Color.Lime);
                }
                else
                {
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    spriteBatch.Draw(renderTarget, Vector2.Zero, Color.Red * 0.4f);
                    spriteBatch.Draw(renderTarget, Vector2.Zero, Color.Red * 0.4f);
                    spriteBatch.Draw(renderTarget, Vector2.Zero, Color.Red * 0.4f);
                }
                spriteBatch.End();
            }

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            drawButton(forceExtractorThumbnail, 5, forceExtractor.Price);
            drawButton(plasmaGeneratorThumbnail, 4, plasmaGenerator.Price);
            drawButton(missileLauncherThumbnail, 3, missileLauncher.Price);
            drawButton(flameThrowerThumbnail, 2, flamethrower.Price);
            drawButton(laserCannonThumbnail, 1, laserCannon.Price);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void drawButton(Texture2D thumbnail, int shift, int price)
        {
            Color color = (price <= player.Money) ? Color.White : Color.White * 0.3f;
            if (GameplayComponent.LastInstance.SpeedLevel == SpeedLevel.PAUSED)
                color = Color.White * 0.3f;
            Vector2 position = new Vector2(GraphicsDevice.Viewport.Width - button.Width * shift, GraphicsDevice.Viewport.Height - button.Height);
            spriteBatch.Draw(button, position, color);
            spriteBatch.Draw(thumbnail, new Vector2(GraphicsDevice.Viewport.Width - button.Width * shift, GraphicsDevice.Viewport.Height - button.Height), color);

            Vector2 pricePosition = position + new Vector2(40, 70) - font.MeasureString(price.ToString()) / 2;
            spriteBatch.DrawString(font, price.ToString(), pricePosition + Vector2.UnitX, Color.Black);
            spriteBatch.DrawString(font, price.ToString(), pricePosition - Vector2.UnitX, Color.Black);
            spriteBatch.DrawString(font, price.ToString(), pricePosition + Vector2.UnitY, Color.Black);
            spriteBatch.DrawString(font, price.ToString(), pricePosition - Vector2.UnitY, Color.Black);
            spriteBatch.DrawString(font, price.ToString(), pricePosition, price <= player.Money ? Color.White : Color.Tomato);

        }

        private bool isOnButton(MouseState mouseState, int shift)
        {
            Vector2 position = new Vector2(GraphicsDevice.Viewport.Width - button.Width * shift, GraphicsDevice.Viewport.Height - button.Height);
            position.X += (80 - 74) / 2;
            position.Y += (80 - 74) / 2;

            return position.X <= mouseState.X && mouseState.X <= position.X + 74 &&
                position.Y <= mouseState.Y && mouseState.Y <= position.Y + 74;
        }

        private bool isOnPanel(MouseState mouseState)
        {
            return isOnButton(mouseState, 1) || isOnButton(mouseState, 2) || isOnButton(mouseState, 3) || isOnButton(mouseState, 4) || isOnButton(mouseState, 5);
        }
    }
}