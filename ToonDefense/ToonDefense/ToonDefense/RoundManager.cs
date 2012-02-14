using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ToonDefense.Spaceships;
using ToonDefense.ParticleSystem;
using System.Reflection;

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

        public RoundManager(Game game, Camera camera, World world)
            : base(game)
        {
            this.camera = camera;
            this.world = world;
            roundDelay = 5000;
            roundDelayCounter = 5000;
            generationDelay = 1000;
            generationDelayCounter = 0;
            pendentShips = new List<Spaceship>();
            roundNumber = 0;
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
                        MethodInfo methodInfo = GetType().GetMethod("Round" + 1);
                        if (methodInfo != null)
                            methodInfo.Invoke(this, null);

                        foreach (Spaceship i in pendentShips)
                        {
                            foreach (Vector3 j in world.Waypoints)
                                i.Destinations.Add(j);
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

        public void Round1()
        {
            Console.WriteLine("round 1");
            Interceptor interceptor = new Interceptor(Game, camera);
            interceptor.Position = new Vector3(0, 2, 0);
            pendentShips.Add(interceptor);
            BattleCruiser battleCruiser = new BattleCruiser(Game, camera);
            battleCruiser.Position = new Vector3(3, 2, 0);
            pendentShips.Add(battleCruiser);
            Explorer explorer = new Explorer(Game, camera);
            explorer.Position = new Vector3(6, 2, 0);
            pendentShips.Add(explorer);
            ScienceVessel scienceVessel = new ScienceVessel(Game, camera);
            scienceVessel.Position = new Vector3(9, 2, 0);
            pendentShips.Add(scienceVessel);
            Unidentified unidentified = new Unidentified(Game, camera);
            unidentified.Position = new Vector3(12, 2, 0);
            pendentShips.Add(unidentified);
            Zeppelin zeppelin = new Zeppelin(Game, camera);
            zeppelin.Position = new Vector3(15, 2, 0);
            pendentShips.Add(zeppelin);
            DeltaDart deltaDart = new DeltaDart(Game, camera);
            deltaDart.Position = new Vector3(18, 2, 0);
            pendentShips.Add(deltaDart);
            Gunner gunner = new Gunner(Game, camera);
            gunner.Position = new Vector3(19, 2, 0);
            pendentShips.Add(gunner);
            HoverBoard hoverBoard = new HoverBoard(Game, camera);
            hoverBoard.Position = new Vector3(22, 2, 0);
            pendentShips.Add(hoverBoard);
            PaperAirplane paperAirplane = new PaperAirplane(Game, camera);
            paperAirplane.Position = new Vector3(24, 2, 0);
            pendentShips.Add(paperAirplane);
            Spectrum spectrum = new Spectrum(Game, camera);
            spectrum.Position = new Vector3(27, 2, 0);
            pendentShips.Add(spectrum);
            Helicopter helicopter = new Helicopter(Game, camera);
            helicopter.Position = new Vector3(-16, 2, 0);
            pendentShips.Add(helicopter);
        }
    }
}
