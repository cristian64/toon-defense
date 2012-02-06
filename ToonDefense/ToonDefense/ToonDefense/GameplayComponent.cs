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

namespace ToonDefense
{
    public class GameplayComponent : DrawableGameComponent
    {
        List<DrawableGameComponent> drawableComponents;
        List<GameComponent> components;
        Camera camera;

        public GameplayComponent(Game game)
            : base(game)
        {
            drawableComponents = new List<DrawableGameComponent>();
            components = new List<GameComponent>();
        }

        public override void Initialize()
        {
            camera = new Camera(Game);
            camera.Position = new Vector3(5, 3, 10);
            camera.Target = new Vector3(8, 5.5f, 0);
            components.Add(camera);

            World world = new World(Game, camera);
            drawableComponents.Add(world);

            Interceptor interceptor = new Interceptor(Game, camera);
            interceptor.Position = new Vector3(3, 1, -4);
            drawableComponents.Add(interceptor);
            BattleCruiser battleCruiser = new BattleCruiser(Game, camera);
            battleCruiser.Position = new Vector3(-5, 1, 0);
            drawableComponents.Add(battleCruiser);
            Explorer explorer = new Explorer(Game, camera);
            explorer.Position = new Vector3(0, 1, -3);
            drawableComponents.Add(explorer);

            drawableComponents.Add(new Axis(Game, camera));
            Box box = new Box(Game, camera);
            box.Position.X = 3;
            box.Position.Y = 1;
            drawableComponents.Add(box);

            foreach (DrawableGameComponent i in drawableComponents)
                i.Initialize();
            foreach (GameComponent i in components)
                i.Initialize();
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (DrawableGameComponent i in drawableComponents)
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
            GraphicsDevice.Clear(Color.RosyBrown);

            foreach (DrawableGameComponent i in drawableComponents)
                i.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}
