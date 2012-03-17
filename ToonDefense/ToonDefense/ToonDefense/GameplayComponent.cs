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
using ToonDefense.Spaceships;
using ToonDefense.Towers;
using ToonDefense.ParticleSystem;
using ToonDefense.Projectiles;

namespace ToonDefense
{
    public enum SpeedLevel { NORMAL, FAST, PAUSED }

    public class GameplayComponent : DrawableGameComponent
    {
        public SpriteBatch SpriteBatch;
        public List<DrawableGameComponent> SpawnComponents;
        public List<DrawableGameComponent> DrawableComponents;
        public List<DrawableGameComponent> GuiComponents;
        List<DrawableGameComponent> particleSystems;
        Player player;
        Camera camera;
        World world;
        public SpeedLevel SpeedLevel;
        public String map;
        Texture2D background;

        FireParticleSystem fireParticleSystem;
        ExplosionParticleSystem explosionParticleSystem;
        ExplosionSmokeParticleSystem explosionSmokeParticleSystem;
        ProjectileTrailParticleSystem projectileTrailParticleSystem;
        WhiteTrailParticleSystem whiteTrailParticleSystem;
        SmokePlumeParticleSystem smokePlumeParticleSystem;
        VortexParticleSystem vortexParticleSystem;
        PlasmaExplosionParticleSystem plasmaExplosionParticleSystem;
        LaserParticleSystem laserParticleSystem;
        ForceFieldParticleSystem forceFieldParticleSystem;
        PlasmaParticleSystem plasmaParticleSystem;

        SoundEffect explosion;
        SoundEffect sold;
        SoundEffect portal;
        public SoundEffectInstance Ost;

        public static GameplayComponent LastInstance = null;

        public GameplayComponent(Game game, String map)
            : base(game)
        {
            this.map = map;
            SpawnComponents = new List<DrawableGameComponent>();
            DrawableComponents = new List<DrawableGameComponent>();
            GuiComponents = new List<DrawableGameComponent>();
            particleSystems = new List<DrawableGameComponent>();

            LastInstance = this;
        }

        public override void Initialize()
        {
            player = new Player(Game);
            GuiComponents.Add(player);

            camera = new Camera(Game);

            world = new World(Game, camera, map);
            DrawableComponents.Add(world);
            camera.World = world;
            GuiComponents.Add(new RoundManager(Game, camera, world));
            GuiComponents.Add(new LabelManager(Game, camera, world));
            GuiComponents.Add(new BuildingPanel(Game, camera, world, player));
            GuiComponents.Add(new SpeedPanel(Game));
            GuiComponents.Add(new SelectingPanel(Game, camera, world));
            GuiComponents.Add(camera);

            fireParticleSystem = new FireParticleSystem(Game, Game.Content, camera);
            explosionParticleSystem = new ExplosionParticleSystem(Game, Game.Content, camera);
            explosionSmokeParticleSystem = new ExplosionSmokeParticleSystem(Game, Game.Content, camera);
            projectileTrailParticleSystem = new ProjectileTrailParticleSystem(Game, Game.Content, camera);
            whiteTrailParticleSystem = new WhiteTrailParticleSystem(Game, Game.Content, camera);
            smokePlumeParticleSystem = new SmokePlumeParticleSystem(Game, Game.Content, camera);
            vortexParticleSystem = new VortexParticleSystem(Game, Game.Content, camera);
            plasmaExplosionParticleSystem = new PlasmaExplosionParticleSystem(Game, Game.Content, camera);
            laserParticleSystem = new LaserParticleSystem(Game, Game.Content, camera);
            forceFieldParticleSystem = new ForceFieldParticleSystem(Game, Game.Content, camera);
            plasmaParticleSystem = new PlasmaParticleSystem(Game, Game.Content, camera);

            particleSystems.Add(explosionSmokeParticleSystem);
            particleSystems.Add(whiteTrailParticleSystem);
            particleSystems.Add(projectileTrailParticleSystem);
            particleSystems.Add(smokePlumeParticleSystem);
            particleSystems.Add(explosionParticleSystem);
            particleSystems.Add(fireParticleSystem);
            particleSystems.Add(vortexParticleSystem);
            particleSystems.Add(plasmaExplosionParticleSystem);
            particleSystems.Add(laserParticleSystem);
            particleSystems.Add(forceFieldParticleSystem);
            particleSystems.Add(plasmaParticleSystem);

            DrawableComponents.Add(new Axis(Game, camera));

            foreach (DrawableGameComponent i in DrawableComponents)
                i.Initialize();
            foreach (DrawableGameComponent i in particleSystems)
                i.Initialize();
            foreach (DrawableGameComponent i in GuiComponents)
                i.Initialize();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            background = Game.Content.Load<Texture2D>("images\\background");
            explosion = Game.Content.Load<SoundEffect>("sounds\\explosion");
            sold = Game.Content.Load<SoundEffect>("sounds\\sold");
            portal = Game.Content.Load<SoundEffect>("sounds\\portal");
            Ost = Game.Content.Load<SoundEffect>("sounds\\ost").CreateInstance();
            Ost.Play();

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (SpeedLevel == SpeedLevel.FAST)
                gameTime = new GameTime(gameTime.TotalGameTime + gameTime.TotalGameTime + gameTime.TotalGameTime + gameTime.TotalGameTime, gameTime.ElapsedGameTime + gameTime.ElapsedGameTime + gameTime.ElapsedGameTime + gameTime.ElapsedGameTime, gameTime.IsRunningSlowly);

            if (SpeedLevel != SpeedLevel.PAUSED)
            {
                for (int i = DrawableComponents.Count - 1; i >= 0; i--)
                {
                    Spaceship spaceship = DrawableComponents[i] as Spaceship;
                    if (spaceship != null)
                    {
                        if (spaceship.Health <= 0)
                        {
                            player.Money += spaceship.Reward;
                            player.Kills++;
                            for (int j = 0; j < 30; j++)
                                explosionSmokeParticleSystem.AddParticle(spaceship.Position, Vector3.Zero);
                            for (int j = 0; j < 30; j++)
                                explosionParticleSystem.AddParticle(spaceship.Position, Vector3.Zero);
                            DrawableComponents.RemoveAt(i);
                            explosion.Play();
                            LabelManager.LastInstance.AddLabel("+" + spaceship.Reward, 2000, spaceship.Position, Color.Lime);
                        }
                        else if (spaceship.Destinations.Count == 0)
                        {
                            player.Lives--;
                            for (int j = 0; j < 10; j++)
                                vortexParticleSystem.AddParticle(spaceship.Position, Vector3.Zero);
                            for (int j = 0; j < 10; j++)
                                plasmaExplosionParticleSystem.AddParticle(spaceship.Position, Vector3.Zero);
                            spaceship.Health = 0;
                            DrawableComponents.RemoveAt(i);
                            portal.Play();
                        }
                    }
                    else
                    {
                        Tower tower = DrawableComponents[i] as Tower;
                        if (tower != null)
                        {
                            if (tower.Sold == true)
                            {
                                player.Money += (tower.Price + (tower.Upgraded ? tower.UpgradePrice : 0)) / 2;
                                for (int j = 0; j < 30; j++)
                                    explosionSmokeParticleSystem.AddParticle(tower.Position, Vector3.Zero);
                                DrawableComponents.RemoveAt(i);
                                sold.Play();
                                world.SetBuildable(new Vector3(tower.Position.X - tower.Width / 2.0f, 0, tower.Position.Z - tower.Depth / 2.0f), new Vector3(tower.Position.X + tower.Width / 2.0f, 0, tower.Position.Z + tower.Depth / 2.0f));
                                LabelManager.LastInstance.AddLabel("+" + (tower.Price + (tower.Upgraded ? tower.UpgradePrice : 0)) / 2, 2000, tower.Position, Color.Lime);
                            }
                        }
                        else
                        {
                            Projectile projectile = DrawableComponents[i] as Projectile;
                            if (projectile != null && projectile.NoTarget)
                                DrawableComponents.RemoveAt(i);
                        }
                    }
                }

                foreach (DrawableGameComponent i in DrawableComponents)
                    i.Update(gameTime);
                foreach (DrawableGameComponent i in SpawnComponents)
                {
                    DrawableComponents.Add(i);
                    if (i as Spaceship != null)
                        portal.Play();
                }
                SpawnComponents.Clear();
                foreach (DrawableGameComponent i in particleSystems)
                    i.Update(gameTime);
            }
            foreach (DrawableGameComponent i in GuiComponents)
                i.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            //gameTime = new GameTime(gameTime.TotalGameTime + gameTime.TotalGameTime + gameTime.TotalGameTime + gameTime.TotalGameTime, gameTime.ElapsedGameTime + gameTime.ElapsedGameTime + gameTime.ElapsedGameTime + gameTime.ElapsedGameTime, gameTime.IsRunningSlowly);

            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            SpriteBatch.Begin();
            SpriteBatch.Draw(background, new Rectangle(0, 0, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferWidth), Color.White);
            SpriteBatch.End();

            foreach (DrawableGameComponent i in DrawableComponents)
                i.Draw(gameTime);
            foreach (DrawableGameComponent i in particleSystems)
                i.Draw(gameTime);
            foreach (DrawableGameComponent i in GuiComponents)
                i.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}
