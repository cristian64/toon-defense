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
using ToonDefense.Spaceships;
using ToonDefense.ParticleSystem;

namespace ToonDefense
{
    public class GameplayComponent : DrawableGameComponent
    {
        List<DrawableGameComponent> drawableComponents;
        List<DrawableGameComponent> particleSystems;
        List<GameComponent> components;
        Camera camera;
        World world;

        FireParticleSystem fireParticleSystem;
        ExplosionParticleSystem explosionParticleSystem;
        ExplosionSmokeParticleSystem explosionSmokeParticleSystem;
        ProjectileTrailParticleSystem projectileTrailParticleSystem;
        SmokePlumeParticleSystem smokePlumeParticleSystem;
        VortexParticleSystem vortexParticleSystem;
        PlasmaExplosionParticleSystem plasmaExplosionParticleSystem;
        LaserParticleSystem laserParticleSystem;
        ForceFieldParticleSystem forceFieldParticleSystem;

        public GameplayComponent(Game game)
            : base(game)
        {
            drawableComponents = new List<DrawableGameComponent>();
            particleSystems = new List<DrawableGameComponent>();
            components = new List<GameComponent>();
        }

        public override void Initialize()
        {
            camera = new Camera(Game);
            camera.Position = new Vector3(5, 3, 10);
            camera.Target = new Vector3(8, 5.5f, 0);
            components.Add(camera);

            world = new World(Game, camera, "map1");
            drawableComponents.Add(world);

            fireParticleSystem = new FireParticleSystem(Game, Game.Content, camera);
            explosionParticleSystem = new ExplosionParticleSystem(Game, Game.Content, camera);
            explosionSmokeParticleSystem = new ExplosionSmokeParticleSystem(Game, Game.Content, camera);
            projectileTrailParticleSystem = new ProjectileTrailParticleSystem(Game, Game.Content, camera);
            smokePlumeParticleSystem = new SmokePlumeParticleSystem(Game, Game.Content, camera);
            vortexParticleSystem = new VortexParticleSystem(Game, Game.Content, camera);
            plasmaExplosionParticleSystem = new PlasmaExplosionParticleSystem(Game, Game.Content, camera);
            laserParticleSystem = new LaserParticleSystem(Game, Game.Content, camera);
            forceFieldParticleSystem = new ForceFieldParticleSystem(Game, Game.Content, camera);

            particleSystems.Add(explosionSmokeParticleSystem);
            particleSystems.Add(projectileTrailParticleSystem);
            particleSystems.Add(smokePlumeParticleSystem);
            particleSystems.Add(explosionParticleSystem);
            particleSystems.Add(fireParticleSystem);
            particleSystems.Add(vortexParticleSystem);
            particleSystems.Add(plasmaExplosionParticleSystem);
            particleSystems.Add(laserParticleSystem);
            particleSystems.Add(forceFieldParticleSystem);

            for (int j = 0; j < 1; j++)
            {
                Interceptor interceptor = new Interceptor(Game, camera);
                interceptor.Position = new Vector3(0, 2, 0);
                drawableComponents.Add(interceptor);
                BattleCruiser battleCruiser = new BattleCruiser(Game, camera);
                battleCruiser.Position = new Vector3(3, 2, 0);
                drawableComponents.Add(battleCruiser);
                Explorer explorer = new Explorer(Game, camera);
                explorer.Position = new Vector3(6, 2, 0);
                drawableComponents.Add(explorer);
                ScienceVessel scienceVessel = new ScienceVessel(Game, camera);
                scienceVessel.Position = new Vector3(9, 2, 0);
                drawableComponents.Add(scienceVessel);
                Unidentified unidentified = new Unidentified(Game, camera);
                unidentified.Position = new Vector3(12, 2, 0);
                drawableComponents.Add(unidentified);
                Zeppelin zeppelin = new Zeppelin(Game, camera);
                zeppelin.Position = new Vector3(15, 2, 0);
                drawableComponents.Add(zeppelin);
                DeltaDart deltaDart = new DeltaDart(Game, camera);
                deltaDart.Position = new Vector3(18, 2, 0);
                drawableComponents.Add(deltaDart);
            }
            drawableComponents.Add(new Axis(Game, camera));

            foreach (DrawableGameComponent i in drawableComponents)
                i.Initialize();
            foreach (DrawableGameComponent i in particleSystems)
                i.Initialize();
            foreach (GameComponent i in components)
                i.Initialize();
            base.Initialize();

            Random random = new Random();
            foreach (DrawableGameComponent i in drawableComponents)
            {
                Spaceship spaceship = i as Spaceship;
                if (spaceship != null)
                {
                    foreach (Vector3 j in world.Waypoints)
                        spaceship.Destinations.Add(j);
                    for (int j = 0; j < 1000; j++)
                        spaceship.Destinations.Add(new Vector3(random.Next() % (int)world.Scale.X - (int)world.Scale.X / 2, 0, random.Next() % (int)world.Scale.Z - (int)world.Scale.Z / 2));
                }
            }
        }

        Vector3 lastPosition;
        Vector3 lastFloor;
        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.D1))
                smokePlumeParticleSystem.AddParticle(new Vector3(-1, 1, 0), new Vector3(0, 0, 0));
            if (Keyboard.GetState().IsKeyDown(Keys.D2))
                explosionSmokeParticleSystem.AddParticle(new Vector3(-1, 1, 0), new Vector3(0, 0, 0));
            if (Keyboard.GetState().IsKeyDown(Keys.D3))
                projectileTrailParticleSystem.AddParticle(new Vector3(-1, 1, 0), new Vector3(0, 0, 0));
            if (Keyboard.GetState().IsKeyDown(Keys.D4))
                explosionParticleSystem.AddParticle(new Vector3(-1, 1, 0), new Vector3(0, 0, 0));
            if (Keyboard.GetState().IsKeyDown(Keys.D5))
                fireParticleSystem.AddParticle(new Vector3(-1, 1, 0), new Vector3(0, 0, 0));
            if (Keyboard.GetState().IsKeyDown(Keys.D6))
                vortexParticleSystem.AddParticle(new Vector3(-1, 1, 0), new Vector3(0, 0, 0));
            if (Keyboard.GetState().IsKeyDown(Keys.D7))
                plasmaExplosionParticleSystem.AddParticle(new Vector3(-1, 1, 0), new Vector3(0, 0, 0));
            if (Keyboard.GetState().IsKeyDown(Keys.D8))
                laserParticleSystem.AddParticle(new Vector3(-1, 1, 0), new Vector3(-10, 5, 0));
            if (Keyboard.GetState().IsKeyDown(Keys.D9))
                forceFieldParticleSystem.AddParticle(new Vector3(-1, 1, 0), new Vector3(-10, 5, 0));

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                lastPosition = camera.Position;
                Vector3 floor = camera.RayFromScreenToFloor(Mouse.GetState().X, Mouse.GetState().Y);
                if (lastFloor == null || !floor.Equals(lastFloor))
                {
                    lastFloor = floor;
                    Console.WriteLine(world.WorldUnitsToTextureUnits(lastFloor).X + " " + world.WorldUnitsToTextureUnits(lastFloor).Y);
                }
            }

            foreach (DrawableGameComponent i in drawableComponents)
                i.Update(gameTime);
            foreach (DrawableGameComponent i in particleSystems)
                i.Update(gameTime);
            foreach (GameComponent i in components)
                i.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (lastFloor != null)
                PrimitiveDrawings.DrawLine(GraphicsDevice, camera, lastPosition, lastFloor, Color.Pink);

            foreach (DrawableGameComponent i in drawableComponents)
                i.Draw(gameTime);
            foreach (DrawableGameComponent i in particleSystems)
                i.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}
