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
    public class GameplayComponent : DrawableGameComponent
    {
        public List<DrawableGameComponent> SpawnComponents;
        public List<DrawableGameComponent> DrawableComponents;
        public List<DrawableGameComponent> GuiComponents;
        List<DrawableGameComponent> particleSystems;
        List<GameComponent> components;
        Player player;
        Camera camera;
        World world;

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

        public static GameplayComponent LastInstance = null;

        public GameplayComponent(Game game)
            : base(game)
        {
            SpawnComponents = new List<DrawableGameComponent>();
            DrawableComponents = new List<DrawableGameComponent>();
            GuiComponents = new List<DrawableGameComponent>();
            particleSystems = new List<DrawableGameComponent>();
            components = new List<GameComponent>();

            LastInstance = this;
        }

        public override void Initialize()
        {
            player = new Player(Game);
            GuiComponents.Add(player);

            camera = new Camera(Game);
            components.Add(camera);

            world = new World(Game, camera, "map1");
            DrawableComponents.Add(world);
            GuiComponents.Add(new RoundManager(Game, camera, world));
            GuiComponents.Add(new BuildingPanel(Game, camera, world, player);

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

            for (int j = 0; j < 1; j++)
            {
                TeslaCoil teslaCoil = new TeslaCoil(Game, camera);
                teslaCoil.Position = new Vector3(3, 2, -3f);
                DrawableComponents.Add(teslaCoil);
                TeslaCoil teslaCoil2 = new TeslaCoil(Game, camera);
                teslaCoil2.Position = new Vector3(3, 2, 2f);
                DrawableComponents.Add(teslaCoil2);
                MissileLauncher missileLauncher = new MissileLauncher(Game, camera);
                missileLauncher.Position = new Vector3(5, 2, 2f);
                DrawableComponents.Add(missileLauncher);
                PlasmaGenerator plasmaGenerator = new PlasmaGenerator(Game, camera);
                plasmaGenerator.Position = new Vector3(7, 2, 2f);
                DrawableComponents.Add(plasmaGenerator);
                LaserCannon laserCannon = new LaserCannon(Game, camera);
                laserCannon.Position = new Vector3(-7, 2, 2f);
                DrawableComponents.Add(laserCannon);
                Flamethower flamethower = new Flamethower(Game, camera);
                flamethower.Position = new Vector3(-3, 2, -2.4f);
                DrawableComponents.Add(flamethower);
            }
            #if DEBUG
            DrawableComponents.Add(new Axis(Game, camera));
            #endif

            foreach (DrawableGameComponent i in DrawableComponents)
                i.Initialize();
            foreach (DrawableGameComponent i in particleSystems)
                i.Initialize();
            foreach (GameComponent i in components)
                i.Initialize();
            foreach (DrawableGameComponent i in GuiComponents)
                i.Initialize();
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            //gameTime = new GameTime(gameTime.TotalGameTime + gameTime.TotalGameTime + gameTime.TotalGameTime + gameTime.TotalGameTime, gameTime.ElapsedGameTime + gameTime.ElapsedGameTime + gameTime.ElapsedGameTime + gameTime.ElapsedGameTime, gameTime.IsRunningSlowly);

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
                    }
                }
                else
                {
                    Projectile projectile = DrawableComponents[i] as Projectile;
                    if (projectile != null && projectile.NoTarget)
                        DrawableComponents.RemoveAt(i);
                }
            }

            foreach (DrawableGameComponent i in DrawableComponents)
                i.Update(gameTime);
            foreach (DrawableGameComponent i in SpawnComponents)
                DrawableComponents.Add(i);
            SpawnComponents.Clear();
            foreach (DrawableGameComponent i in particleSystems)
                i.Update(gameTime);
            foreach (GameComponent i in components)
                i.Update(gameTime);
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
            GraphicsDevice.Clear(Color.CornflowerBlue);

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
