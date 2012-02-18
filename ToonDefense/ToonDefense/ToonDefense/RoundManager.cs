using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ToonDefense.Spaceships;
using ToonDefense.ParticleSystem;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;

namespace ToonDefense
{
    public class RoundManager : DrawableGameComponent
    {
        Camera camera;
        World world;
        double roundDelay;
        double roundDelayCounter;
        double generationDelay;
        double generationDelayCounter;
        List<Spaceship> pendentShips;
        int roundNumber;
        int roundCount;
        Random random;

        double displayTimeCounter;
        static double displayTime = 5000;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;

        public RoundManager(Game game, Camera camera, World world)
            : base(game)
        {
            this.camera = camera;
            this.world = world;
            roundDelay = 5000;
            roundDelayCounter = 2000;
            generationDelay = 1000;
            generationDelayCounter = 0;
            pendentShips = new List<Spaceship>();
            roundNumber = 0;
            roundCount = 0;
            foreach (MethodInfo i in GetType().GetMethods())
                if (i.Name.Contains("Round"))
                    roundCount++;
            random = new Random();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = Game.Content.Load<SpriteFont>("fonts\\round");
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (pendentShips.Count == 0)
            {
                roundDelayCounter += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (roundDelayCounter > roundDelay)
                {
                    roundDelayCounter = 0;

                    int spaceshipCounter = 0;
                    foreach (DrawableGameComponent i in GameplayComponent.LastInstance.DrawableComponents)
                        if (i as Spaceship != null)
                            spaceshipCounter++;

                    if (spaceshipCounter == 0)
                    {
                        if (roundNumber > roundCount)
                            roundNumber = 0;
                        MethodInfo methodInfo = GetType().GetMethod("Round" + ++roundNumber);
                        if (methodInfo != null)
                        {
                            methodInfo.Invoke(this, null);
                            displayTimeCounter = displayTime;
                        }

                        foreach (Spaceship i in pendentShips)
                        {
                            i.Initialize();
                            Vector3 noise = new Vector3((float)random.NextDouble() - 0.5f, 0, (float)random.NextDouble() - 0.5f);
                            foreach (Vector3 j in world.Waypoints)
                                i.Destinations.Add(j + noise);
                            i.Position.X = i.Destinations[0].X;
                            i.Position.Z = i.Destinations[0].Z;
                            i.Destinations.RemoveAt(0);
                        }
                    }
                }
            }
            else
            {
                generationDelayCounter += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (generationDelayCounter > generationDelay)
                {
                    generationDelayCounter -= generationDelay;
                    GameplayComponent.LastInstance.SpawnComponents.Add(pendentShips[0]);
                    VortexParticleSystem.LastInstance.AddParticle(pendentShips[0].Position, Vector3.Up * 0.1f);
                    VortexParticleSystem.LastInstance.AddParticle(pendentShips[0].Position, Vector3.Down * 0.1f);
                    VortexParticleSystem.LastInstance.AddParticle(pendentShips[0].Position, Vector3.Left * 0.1f);
                    VortexParticleSystem.LastInstance.AddParticle(pendentShips[0].Position, Vector3.Right * 0.1f);
                    VortexParticleSystem.LastInstance.AddParticle(pendentShips[0].Position, Vector3.Forward * 0.1f);
                    VortexParticleSystem.LastInstance.AddParticle(pendentShips[0].Position, Vector3.Backward * 0.1f);
                    pendentShips.RemoveAt(0);
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (displayTimeCounter > 0)
            {
                float alpha = (displayTimeCounter <= 300) ? (float)(displayTimeCounter / 300) : 1.0f;
                displayTimeCounter = Math.Max(0, displayTimeCounter - gameTime.ElapsedGameTime.TotalMilliseconds);
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                String text = "Round " + roundNumber;
                Vector2 position = new Vector2(GraphicsDevice.Viewport.Width / 2 - spriteFont.MeasureString(text).X / 2, GraphicsDevice.Viewport.Height / 2 - spriteFont.MeasureString(text).Y / 2);
                spriteBatch.DrawString(spriteFont, text, position, Color.Black * 0.7f * alpha * alpha);
                spriteBatch.DrawString(spriteFont, text, position + Vector2.UnitY, Color.Black * 0.2f * alpha * alpha);
                spriteBatch.DrawString(spriteFont, text, position - Vector2.One, Color.White * 0.9f * alpha);
                spriteBatch.End();
            }
            base.Draw(gameTime);
        }

        public void Round1()
        {
			generationDelay = 200;
            pendentShips.Add(new Interceptor(Game, camera));
        }

        public void Round2()
        {
			generationDelay = 200;
            for (int i = 0; i < 5; i++)
                pendentShips.Add(new Interceptor(Game, camera));
        }

        public void Round3()
        {
			generationDelay = 1000;
            Interceptor interceptor = new Interceptor(Game, camera);
            pendentShips.Add(interceptor);
            BattleCruiser battleCruiser = new BattleCruiser(Game, camera);
            pendentShips.Add(battleCruiser);
            Explorer explorer = new Explorer(Game, camera);
            pendentShips.Add(explorer);
            ScienceVessel scienceVessel = new ScienceVessel(Game, camera);
            pendentShips.Add(scienceVessel);
            Unidentified unidentified = new Unidentified(Game, camera);
            pendentShips.Add(unidentified);
            Zeppelin zeppelin = new Zeppelin(Game, camera);
            pendentShips.Add(zeppelin);
            DeltaDart deltaDart = new DeltaDart(Game, camera);
            pendentShips.Add(deltaDart);
            Gunner gunner = new Gunner(Game, camera);
            pendentShips.Add(gunner);
            HoverBoard hoverBoard = new HoverBoard(Game, camera);
            pendentShips.Add(hoverBoard);
            PaperAirplane paperAirplane = new PaperAirplane(Game, camera);
            pendentShips.Add(paperAirplane);
            Spectrum spectrum = new Spectrum(Game, camera);
            pendentShips.Add(spectrum);
            Helicopter helicopter = new Helicopter(Game, camera);
            pendentShips.Add(helicopter);
        }
    }
}
