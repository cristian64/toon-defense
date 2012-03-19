using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ToonDefense.Spaceships;
using ToonDefense.ParticleSystem;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace ToonDefense
{
    public class RoundManager : DrawableGameComponent
    {
        public static RoundManager LastInstance = null;
        Camera camera;
        World world;
        double roundDelay;
        double roundDelayCounter;
        double generationDelay;
        double generationDelayCounter;
        List<Spaceship> pendentShips;
        public int RoundNumber;
        int roundCount;
        Random random;
        List<MethodInfo> rounds;

        double displayTimeCounter;
        static double displayTime = 3000;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        SpriteFont spriteFont2;

        public RoundManager(Game game, Camera camera, World world)
            : base(game)
        {
            LastInstance = this;
            this.camera = camera;
            this.world = world;
            roundDelay = 5000;
            roundDelayCounter = 2000;
            generationDelay = 1000;
            generationDelayCounter = 0;
            pendentShips = new List<Spaceship>();
            RoundNumber = 0;
            roundCount = 0;
            rounds = new List<MethodInfo>();
            foreach (MethodInfo i in GetType().GetMethods())
                if (i.Name.Contains("Round"))
                    roundCount++;
            for (int i = 1; i <= roundCount; i++)
                rounds.Add(GetType().GetMethod("Round" + i));
            random = new Random();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = Game.Content.Load<SpriteFont>("fonts\\round");
            spriteFont2 = Game.Content.Load<SpriteFont>("fonts\\roundsubtitle");
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (GameplayComponent.LastInstance.SpeedLevel != SpeedLevel.PAUSED)
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
                            MethodInfo methodInfo = rounds[Math.Min(roundCount - 1, RoundNumber++)];
                            if (methodInfo != null)
                            {
                                methodInfo.Invoke(this, null);
                                displayTimeCounter = displayTime;
                                Game.Content.Load<SoundEffect>("sounds\\round").Play();
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
                                Vector3 direction = i.Destinations[0] - i.Position;
                                i.Rotation.Y = (float)Math.Atan2(-direction.Z, direction.X);
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
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (displayTimeCounter > 0)
            {
                float alpha = (displayTimeCounter <= 300) ? (float)(displayTimeCounter / 300) : 1.0f;
                alpha = (displayTime - displayTimeCounter <= 300) ? (float)((displayTime - displayTimeCounter) / 300) : alpha;
                displayTimeCounter = Math.Max(0, displayTimeCounter - gameTime.ElapsedGameTime.TotalMilliseconds);
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                String text = "Round " + RoundNumber;
                String text2 = roundCount - RoundNumber + " rounds more to go";
                if (roundCount - RoundNumber == 0)
                    text2 = "Last round";
                else if (roundCount < RoundNumber)
                    text2 = "How many rounds can you still fuck, bitch?";
                Vector2 position = new Vector2(GraphicsDevice.Viewport.Width / 2 - spriteFont.MeasureString(text).X / 2, GraphicsDevice.Viewport.Height / 2 - spriteFont.MeasureString(text).Y / 2);
                Vector2 position2 = new Vector2(GraphicsDevice.Viewport.Width / 2 - spriteFont2.MeasureString(text2).X / 2, GraphicsDevice.Viewport.Height / 2 - spriteFont2.MeasureString(text2).Y / 2 + spriteFont.MeasureString(text).Y / 2);

                spriteBatch.DrawString(spriteFont, text, position + 3 * Vector2.UnitX, Color.Black * 0.25f * alpha);
                spriteBatch.DrawString(spriteFont, text, position - 3 * Vector2.UnitX, Color.Black * 0.25f * alpha);
                spriteBatch.DrawString(spriteFont, text, position + 3 * Vector2.UnitY, Color.Black * 0.25f * alpha);
                spriteBatch.DrawString(spriteFont, text, position - 3 * Vector2.UnitY, Color.Black * 0.25f * alpha);
                spriteBatch.DrawString(spriteFont, text, position + Vector2.UnitX, Color.White * alpha * alpha * alpha * alpha * alpha);
                spriteBatch.DrawString(spriteFont, text, position - Vector2.UnitX, Color.White * alpha * alpha * alpha * alpha * alpha);
                spriteBatch.DrawString(spriteFont, text, position + Vector2.UnitY, Color.White * alpha * alpha * alpha * alpha * alpha);
                spriteBatch.DrawString(spriteFont, text, position - Vector2.UnitY, Color.White * alpha * alpha * alpha * alpha * alpha);
                spriteBatch.DrawString(spriteFont, text, position, Color.Black * alpha);

                spriteBatch.DrawString(spriteFont2, text2, position2 + 3 * Vector2.UnitX, Color.Black * 0.25f * alpha);
                spriteBatch.DrawString(spriteFont2, text2, position2 - 3 * Vector2.UnitX, Color.Black * 0.25f * alpha);
                spriteBatch.DrawString(spriteFont2, text2, position2 + 3 * Vector2.UnitY, Color.Black * 0.25f * alpha);
                spriteBatch.DrawString(spriteFont2, text2, position2 - 3 * Vector2.UnitY, Color.Black * 0.25f * alpha);
                spriteBatch.DrawString(spriteFont2, text2, position2 + Vector2.UnitX, Color.White * 0.40f * alpha * alpha * alpha * alpha);
                spriteBatch.DrawString(spriteFont2, text2, position2 - Vector2.UnitX, Color.White * 0.40f * alpha * alpha * alpha * alpha);
                spriteBatch.DrawString(spriteFont2, text2, position2 + Vector2.UnitY, Color.White * 0.40f * alpha * alpha * alpha * alpha);
                spriteBatch.DrawString(spriteFont2, text2, position2 - Vector2.UnitY, Color.White * 0.40f * alpha * alpha * alpha * alpha);
                spriteBatch.DrawString(spriteFont2, text2, position2, Color.Black * alpha);

                spriteBatch.End();
            }
            base.Draw(gameTime);
        }

        public void Round1()
        {
			generationDelay = 100;
            pendentShips.Add(new PaperAirplane(Game, camera));
        }

        public void Round2()
        {
            generationDelay = 1500;
            for (int i = 0; i < 6; i++)
                pendentShips.Add(new PaperAirplane(Game, camera));
        }

        public void Round3()
        {
            generationDelay = 2000;
            for (int i = 0; i < 10; i++)
                pendentShips.Add(new PaperAirplane(Game, camera));
        }

        public void Round4()
        {
            generationDelay = 1000;
            for (int i = 0; i < 15; i++)
                pendentShips.Add(new PaperAirplane(Game, camera));
        }

        public void Round5()
        {
            generationDelay = 1000;
            for (int i = 0; i < 20; i++)
                pendentShips.Add(new PaperAirplane(Game, camera));
        }

        public void Round6()
        {
            generationDelay = 400;
            for (int i = 0; i < 30; i++)
                pendentShips.Add(new PaperAirplane(Game, camera));
        }

        public void Round7()
        {
            generationDelay = 300;
            for (int i = 0; i < 1; i++)
                pendentShips.Add(new HoverBoard(Game, camera));
        }

        public void Round8()
        {
            generationDelay = 500;
            for (int i = 0; i < 10; i++)
                pendentShips.Add(new HoverBoard(Game, camera));
        }

        public void Round9()
        {
            generationDelay = 700;
            for (int i = 0; i < 20; i++)
                pendentShips.Add(new HoverBoard(Game, camera));
        }

        public void Round10()
        {
            generationDelay = 500;
            for (int i = 0; i < 40; i++)
                pendentShips.Add(new HoverBoard(Game, camera));
        }

        public void Round11()
        {
            generationDelay = 400;
            for (int i = 0; i < 50; i++)
                pendentShips.Add(new HoverBoard(Game, camera));
        }

        public void Round12()
        {
            generationDelay = 300;
            for (int i = 0; i < 50; i++)
                pendentShips.Add(new HoverBoard(Game, camera));
        }

        public void Round13()
        {
            generationDelay = 400;
            for (int i = 0; i < 1; i++)
                pendentShips.Add(new Explorer(Game, camera));
        }

        public void Round14()
        {
            generationDelay = 500;
            for (int i = 0; i < 10; i++)
                pendentShips.Add(new Explorer(Game, camera));
        }

        public void Round15()
        {
            generationDelay = 300;
            for (int i = 0; i < 15; i++)
                pendentShips.Add(new Explorer(Game, camera));
        }

        public void Round16()
        {
            generationDelay = 300;
            for (int i = 0; i < 20; i++)
                pendentShips.Add(new Explorer(Game, camera));
        }

        public void Round17()
        {
            generationDelay = 300;
            for (int i = 0; i < 30; i++)
                pendentShips.Add(new Explorer(Game, camera));
        }

        public void Round18()
        {
            generationDelay = 300;
            for (int i = 0; i < 50; i++)
                pendentShips.Add(new Explorer(Game, camera));
        }

        public void Round19()
        {
            generationDelay = 200;
            for (int i = 0; i < 30; i++)
                pendentShips.Add(new Explorer(Game, camera));
        }

        public void Round20()
        {
            generationDelay = 200;
            for (int i = 0; i < 45; i++)
                pendentShips.Add(new Explorer(Game, camera));
        }

        public void Round21()
        {
            generationDelay = 100;
            for (int i = 0; i < 1; i++)
                pendentShips.Add(new Interceptor(Game, camera));
        }

        public void Round22()
        {
            generationDelay = 1000;
            for (int i = 0; i < 10; i++)
                pendentShips.Add(new Interceptor(Game, camera));
        }

        public void Round23()
        {
            generationDelay = 100;
            for (int i = 0; i < 1; i++)
                pendentShips.Add(new Interceptor(Game, camera));
        }

        public void Round24()
        {
            generationDelay = 500;
            for (int i = 0; i < 10; i++)
                pendentShips.Add(new Interceptor(Game, camera));
        }

        public void Round25()
        {
            generationDelay = 500;
            for (int i = 0; i < 20; i++)
                pendentShips.Add(new Interceptor(Game, camera));
        }

        public void Round26()
        {
            generationDelay = 300;
            for (int i = 0; i < 20; i++)
                pendentShips.Add(new Interceptor(Game, camera));
        }

        public void Round27()
        {
            generationDelay = 400;
            for (int i = 0; i < 30; i++)
                pendentShips.Add(new Interceptor(Game, camera));
        }

        public void Round28()
        {
            generationDelay = 200;
            for (int i = 0; i < 20; i++)
                pendentShips.Add(new Interceptor(Game, camera));
        }

        public void Round29()
        {
            generationDelay = 100;
            for (int i = 0; i < 15; i++)
                pendentShips.Add(new Interceptor(Game, camera));
        }

        public void Round30()
        {
            generationDelay = 100;
            for (int i = 0; i < 20; i++)
                pendentShips.Add(new Interceptor(Game, camera));
        }

        public void Round31()
        {
            generationDelay = 50;
            for (int i = 0; i < 1; i++)
                pendentShips.Add(new Helicopter(Game, camera));
        }

        public void Round32()
        {
            generationDelay = 1500;
            for (int i = 0; i < 10; i++)
                pendentShips.Add(new Helicopter(Game, camera));
        }

        public void Round33()
        {
            generationDelay = 1500;
            for (int i = 0; i < 30; i++)
                pendentShips.Add(new Helicopter(Game, camera));
        }

        public void Round34()
        {
            generationDelay = 1000;
            for (int i = 0; i < 30; i++)
                pendentShips.Add(new Helicopter(Game, camera));
        }

        public void Round35()
        {
            generationDelay = 1000;
            for (int i = 0; i < 50; i++)
                pendentShips.Add(new Helicopter(Game, camera));
        }

        public void Round36()
        {
            generationDelay = 500;
            for (int i = 0; i < 25; i++)
                pendentShips.Add(new Helicopter(Game, camera));
        }

        public void Round37()
        {
            generationDelay = 500;
            for (int i = 0; i < 50; i++)
                pendentShips.Add(new Helicopter(Game, camera));
        }

        public void Round38()
        {
            generationDelay = 400;
            for (int i = 0; i < 25; i++)
                pendentShips.Add(new Helicopter(Game, camera));
        }

        public void Round39()
        {
            generationDelay = 400;
            for (int i = 0; i < 40; i++)
                pendentShips.Add(new Helicopter(Game, camera));
        }

        public void Round40()
        {
            generationDelay = 300;
            for (int i = 0; i < 30; i++)
                pendentShips.Add(new Helicopter(Game, camera));
        }

        public void Round41()
        {
            generationDelay = 100;
            for (int i = 0; i < 1; i++)
                pendentShips.Add(new DeltaDart(Game, camera));
        }

        public void Round42()
        {
            generationDelay = 500;
            for (int i = 0; i < 5; i++)
                pendentShips.Add(new DeltaDart(Game, camera));
        }

        public void Round43()
        {
            generationDelay = 1000;
            for (int i = 0; i < 10; i++)
                pendentShips.Add(new DeltaDart(Game, camera));
        }

        public void Round44()
        {
            generationDelay = 750;
            for (int i = 0; i < 15; i++)
                pendentShips.Add(new DeltaDart(Game, camera));
        }

        public void Round45()
        {
            generationDelay = 500;
            for (int i = 0; i < 15; i++)
                pendentShips.Add(new DeltaDart(Game, camera));
        }

        public void Round46()
        {
            generationDelay = 500;
            for (int i = 0; i < 30; i++)
                pendentShips.Add(new DeltaDart(Game, camera));
        }

        public void Round47()
        {
            generationDelay = 500;
            for (int i = 0; i < 40; i++)
                pendentShips.Add(new DeltaDart(Game, camera));
        }

        public void Round48()
        {
            generationDelay = 100;
            for (int i = 0; i < 20; i++)
                pendentShips.Add(new DeltaDart(Game, camera));
        }

        public void Round49()
        {
            generationDelay = 150;
            for (int i = 0; i < 30; i++)
                pendentShips.Add(new DeltaDart(Game, camera));
        }

        public void Round50()
        {
            generationDelay = 75;
            for (int i = 0; i < 20; i++)
                pendentShips.Add(new DeltaDart(Game, camera));
        }

        public void Round51()
        {
            generationDelay = 50;
            for (int i = 0; i < 1; i++)
                pendentShips.Add(new Gunner(Game, camera));
        }

        public void Round52()
        {
            generationDelay = 1000;
            for (int i = 0; i < 20; i++)
                pendentShips.Add(new Gunner(Game, camera));
        }

        public void Round53()
        {
            generationDelay = 1000;
            for (int i = 0; i < 30; i++)
                pendentShips.Add(new Gunner(Game, camera));
        }

        public void Round54()
        {
            generationDelay = 1000;
            for (int i = 0; i < 40; i++)
                pendentShips.Add(new Gunner(Game, camera));
        }

        public void Round55()
        {
            generationDelay = 500;
            for (int i = 0; i < 40; i++)
                pendentShips.Add(new Gunner(Game, camera));
        }

        public void Round56()
        {
            generationDelay = 500;
            for (int i = 0; i < 40; i++)
                pendentShips.Add(new Gunner(Game, camera));
        }

        public void Round57()
        {
            generationDelay = 500;
            for (int i = 0; i < 50; i++)
                pendentShips.Add(new Gunner(Game, camera));
        }

        public void Round58()
        {
            generationDelay = 400;
            for (int i = 0; i < 40; i++)
                pendentShips.Add(new Gunner(Game, camera));
        }

        public void Round59()
        {
            generationDelay = 300;
            for (int i = 0; i < 40; i++)
                pendentShips.Add(new Gunner(Game, camera));
        }

        public void Round60()
        {
            generationDelay = 350;
            for (int i = 0; i < 50; i++)
                pendentShips.Add(new Gunner(Game, camera));
        }

        public void Round61()
        {
            generationDelay = 250;
            for (int i = 0; i < 1; i++)
                pendentShips.Add(new Unidentified(Game, camera));
        }

        public void Round62()
        {
            generationDelay = 1000;
            for (int i = 0; i < 10; i++)
                pendentShips.Add(new Unidentified(Game, camera));
        }

        public void Round63()
        {
            generationDelay = 2000;
            for (int i = 0; i < 25; i++)
                pendentShips.Add(new Unidentified(Game, camera));
        }

        public void Round64()
        {
            generationDelay = 800;
            for (int i = 0; i < 10; i++)
                pendentShips.Add(new Unidentified(Game, camera));
        }

        public void Round65()
        {
            generationDelay = 500;
            for (int i = 0; i < 10; i++)
                pendentShips.Add(new Unidentified(Game, camera));
        }

        public void Round66()
        {
            generationDelay = 500;
            for (int i = 0; i < 15; i++)
                pendentShips.Add(new Unidentified(Game, camera));
        }

        public void Round67()
        {
            generationDelay = 1100;
            for (int i = 0; i < 50; i++)
                pendentShips.Add(new Unidentified(Game, camera));
        }

        public void Round68()
        {
            generationDelay = 1000;
            for (int i = 0; i < 50; i++)
                pendentShips.Add(new Unidentified(Game, camera));
        }

        public void Round69()
        {
            generationDelay = 900;
            for (int i = 0; i < 50; i++)
                pendentShips.Add(new Unidentified(Game, camera));
        }

        public void Round70()
        {
            generationDelay = 800;
            for (int i = 0; i < 50; i++)
                pendentShips.Add(new Unidentified(Game, camera));
        }

        public void Round71()
        {
            generationDelay = 500;
            for (int i = 0; i < 1; i++)
                pendentShips.Add(new Zeppelin(Game, camera));
        }

        public void Round72()
        {
            generationDelay = 4000;
            for (int i = 0; i < 5; i++)
                pendentShips.Add(new Zeppelin(Game, camera));
        }

        public void Round73()
        {
            generationDelay = 3000;
            for (int i = 0; i < 5; i++)
                pendentShips.Add(new Zeppelin(Game, camera));
        }

        public void Round74()
        {
            generationDelay = 4000;
            for (int i = 0; i < 10; i++)
                pendentShips.Add(new Zeppelin(Game, camera));
        }

        public void Round75()
        {
            generationDelay = 3000;
            for (int i = 0; i < 15; i++)
                pendentShips.Add(new Zeppelin(Game, camera));
        }

        public void Round76()
        {
            generationDelay = 5000;
            for (int i = 0; i < 40; i++)
                pendentShips.Add(new Zeppelin(Game, camera));
        }

        public void Round77()
        {
            generationDelay = 3000;
            for (int i = 0; i < 40; i++)
                pendentShips.Add(new Zeppelin(Game, camera));
        }

        public void Round78()
        {
            generationDelay = 3000;
            for (int i = 0; i < 50; i++)
                pendentShips.Add(new Zeppelin(Game, camera));
        }

        public void Round79()
        {
            generationDelay = 2500;
            for (int i = 0; i < 50; i++)
                pendentShips.Add(new Zeppelin(Game, camera));
        }

        public void Round80()
        {
            generationDelay = 2500;
            for (int i = 0; i < 60; i++)
                pendentShips.Add(new Zeppelin(Game, camera));
        }

        public void Round81()
        {
            generationDelay = 100;
            for (int i = 0; i < 1; i++)
                pendentShips.Add(new BattleCruiser(Game, camera));
        }

        public void Round82()
        {
            generationDelay = 1000;
            for (int i = 0; i < 5; i++)
                pendentShips.Add(new BattleCruiser(Game, camera));
        }

        public void Round83()
        {
            generationDelay = 3000;
            for (int i = 0; i < 15; i++)
                pendentShips.Add(new BattleCruiser(Game, camera));
        }

        public void Round84()
        {
            generationDelay = 3000;
            for (int i = 0; i < 20; i++)
                pendentShips.Add(new BattleCruiser(Game, camera));
        }

        public void Round85()
        {
            generationDelay = 4000;
            for (int i = 0; i < 30; i++)
                pendentShips.Add(new BattleCruiser(Game, camera));
        }

        public void Round86()
        {
            generationDelay = 2000;
            for (int i = 0; i < 30; i++)
                pendentShips.Add(new BattleCruiser(Game, camera));
        }

        public void Round87()
        {
            generationDelay = 1000;
            for (int i = 0; i < 15; i++)
                pendentShips.Add(new BattleCruiser(Game, camera));
        }

        public void Round88()
        {
            generationDelay = 2000;
            for (int i = 0; i < 45; i++)
                pendentShips.Add(new BattleCruiser(Game, camera));
        }

        public void Round89()
        {
            generationDelay = 2000;
            for (int i = 0; i < 50; i++)
                pendentShips.Add(new BattleCruiser(Game, camera));
        }

        public void Round90()
        {
            generationDelay = 500;
            for (int i = 0; i < 15; i++)
                pendentShips.Add(new BattleCruiser(Game, camera));
        }

        public void Round91()
        {
            generationDelay = 500;
            for (int i = 0; i < 1; i++)
                pendentShips.Add(new Spectrum(Game, camera));
        }

        public void Round92()
        {
            generationDelay = 2000;
            for (int i = 0; i < 10; i++)
                pendentShips.Add(new Spectrum(Game, camera));
        }

        public void Round93()
        {
            generationDelay = 2000;
            for (int i = 0; i < 15; i++)
                pendentShips.Add(new Spectrum(Game, camera));
        }

        public void Round94()
        {
            generationDelay = 2000;
            for (int i = 0; i < 20; i++)
                pendentShips.Add(new Spectrum(Game, camera));
        }

        public void Round95()
        {
            generationDelay = 2000;
            for (int i = 0; i < 25; i++)
                pendentShips.Add(new Spectrum(Game, camera));
        }

        public void Round96()
        {
            generationDelay = 1000;
            for (int i = 0; i < 10; i++)
                pendentShips.Add(new Spectrum(Game, camera));
        }

        public void Round97()
        {
            generationDelay = 1000;
            for (int i = 0; i < 15; i++)
                pendentShips.Add(new Spectrum(Game, camera));
        }

        public void Round98()
        {
            generationDelay = 500;
            for (int i = 0; i < 10; i++)
                pendentShips.Add(new Spectrum(Game, camera));
        }

        public void Round99()
        {
            generationDelay = 400;
            for (int i = 0; i < 10; i++)
                pendentShips.Add(new Spectrum(Game, camera));
        }

        public void Round100()
        {
            generationDelay = 300;
            for (int i = 0; i < 10; i++)
                pendentShips.Add(new Spectrum(Game, camera));
        }

        public void Round101()
        {
            generationDelay = 300;
            for (int i = 0; i < 1; i++)
                pendentShips.Add(new ScienceVessel(Game, camera));
        }

        public void Round102()
        {
            generationDelay = 2000;
            for (int i = 0; i < 10; i++)
                pendentShips.Add(new ScienceVessel(Game, camera));
        }

        public void Round103()
        {
            generationDelay = 2000;
            for (int i = 0; i < 15; i++)
                pendentShips.Add(new ScienceVessel(Game, camera));
        }

        public void Round104()
        {
            generationDelay = 2000;
            for (int i = 0; i < 20; i++)
                pendentShips.Add(new ScienceVessel(Game, camera));
        }

        public void Round105()
        {
            generationDelay = 2000;
            for (int i = 0; i < 30; i++)
                pendentShips.Add(new ScienceVessel(Game, camera));
        }

        public void Round106()
        {
            generationDelay = 1500;
            for (int i = 0; i < 20; i++)
                pendentShips.Add(new ScienceVessel(Game, camera));
        }

        public void Round107()
        {
            generationDelay = 1000;
            for (int i = 0; i < 15; i++)
                pendentShips.Add(new ScienceVessel(Game, camera));
        }

        public void Round108()
        {
            generationDelay = 1000;
            for (int i = 0; i < 15; i++)
                pendentShips.Add(new ScienceVessel(Game, camera));
        }

        public void Round109()
        {
            generationDelay = 500;
            for (int i = 0; i < 10; i++)
                pendentShips.Add(new ScienceVessel(Game, camera));
        }

        public void Round110()
        {
            generationDelay = 500;
            for (int i = 0; i < 20; i++)
                pendentShips.Add(new ScienceVessel(Game, camera));
        }

        public void Round111()
        {
            generationDelay = 500;
            for (int i = 0; i < 1; i++)
                pendentShips.Add(new ElectricityWorm(Game, camera));
        }

        public void Round112()
        {
            generationDelay = 2000;
            for (int i = 0; i < 5; i++)
                pendentShips.Add(new ElectricityWorm(Game, camera));
        }

        public void Round113()
        {
            generationDelay = 2000;
            for (int i = 0; i < 10; i++)
                pendentShips.Add(new ElectricityWorm(Game, camera));
        }

        public void Round114()
        {
            generationDelay = 3000;
            for (int i = 0; i < 15; i++)
                pendentShips.Add(new ElectricityWorm(Game, camera));
        }

        public void Round115()
        {
            generationDelay = 2000;
            for (int i = 0; i < 15; i++)
                pendentShips.Add(new ElectricityWorm(Game, camera));
        }

        public void Round116()
        {
            generationDelay = 1000;
            for (int i = 0; i < 15; i++)
                pendentShips.Add(new ElectricityWorm(Game, camera));
        }

        public void Round117()
        {
            generationDelay = 1000;
            for (int i = 0; i < 20; i++)
                pendentShips.Add(new ElectricityWorm(Game, camera));
        }

        public void Round118()
        {
            generationDelay = 1000;
            for (int i = 0; i < 25; i++)
                pendentShips.Add(new ElectricityWorm(Game, camera));
        }

        public void Round119()
        {
            generationDelay = 1000;
            for (int i = 0; i < 30; i++)
                pendentShips.Add(new ElectricityWorm(Game, camera));
        }

        public void Round120()
        {
            generationDelay = 500;
            for (int i = 0; i < 20; i++)
                pendentShips.Add(new ElectricityWorm(Game, camera));
        }

        public void Round121()
        {
            generationDelay = 500;
            for (int i = 0; i < 1; i++)
                pendentShips.Add(new EquipoME(Game, camera));
        }

        public void Round122()
        {
            generationDelay = 500;
            for (int i = 0; i < 5; i++)
                pendentShips.Add(new EquipoME(Game, camera));
        }

        public void Round123()
        {
            generationDelay = 1000;
            for (int i = 0; i < 10; i++)
                pendentShips.Add(new EquipoME(Game, camera));
        }

        public void Round124()
        {
            generationDelay = 1000;
            for (int i = 0; i < 15; i++)
                pendentShips.Add(new EquipoME(Game, camera));
        }

        public void Round125()
        {
            generationDelay = 1000;
            for (int i = 0; i < 20; i++)
                pendentShips.Add(new EquipoME(Game, camera));
        }

        public void Round126()
        {
            generationDelay = 500;
            for (int i = 0; i < 10; i++)
                pendentShips.Add(new EquipoME(Game, camera));
        }

        public void Round127()
        {
            generationDelay = 200;
            for (int i = 0; i < 10; i++)
                pendentShips.Add(new EquipoME(Game, camera));
        }

        public void Round128()
        {
            generationDelay = 200;
            for (int i = 0; i < 15; i++)
                pendentShips.Add(new EquipoME(Game, camera));
        }

        public void Round129()
        {
            generationDelay = 500;
            for (int i = 0; i < 25; i++)
                pendentShips.Add(new EquipoME(Game, camera));
        }

        public void Round130()
        {
            generationDelay = 700;
            for (int i = 0; i < 50; i++)
                pendentShips.Add(new EquipoME(Game, camera));
        }

        public void Round131()
        {
            generationDelay = 100;
            int number = 30 + RoundNumber - roundCount;
            for (int i = 0; i < number; i++)
            {
                switch(random.Next(14))
                {
                    case 0:
                        pendentShips.Add(new PaperAirplane(Game, camera));
                        break;
                    case 1:
                        pendentShips.Add(new HoverBoard(Game, camera));
                        break;
                    case 2:
                        pendentShips.Add(new Explorer(Game, camera));
                        break;
                    case 3:
                        pendentShips.Add(new Interceptor(Game, camera));
                        break;
                    case 4:
                        pendentShips.Add(new Helicopter(Game, camera));
                        break;
                    case 5:
                        pendentShips.Add(new DeltaDart(Game, camera));
                        break;
                    case 6:
                        pendentShips.Add(new Gunner(Game, camera));
                        break;
                    case 7:
                        pendentShips.Add(new Unidentified(Game, camera));
                        break;
                    case 8:
                        pendentShips.Add(new Zeppelin(Game, camera));
                        break;
                    case 9:
                        pendentShips.Add(new BattleCruiser(Game, camera));
                        break;
                    case 10:
                        pendentShips.Add(new Spectrum(Game, camera));
                        break;
                    case 11:
                        pendentShips.Add(new ScienceVessel(Game, camera));
                        break;
                    case 12:
                        pendentShips.Add(new ElectricityWorm(Game, camera));
                        break;
                    case 13:
                        pendentShips.Add(new EquipoME(Game, camera));
                        break;
                }
            }
        }

        public void Beta()
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
            EquipoME equipoMe = new EquipoME(Game, camera);
            pendentShips.Add(equipoMe);
            ElectricityWorm electricity = new ElectricityWorm(Game, camera);
            pendentShips.Add(electricity);
        }
    }
}
