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
using ToonDefense.ParticleSystem;

namespace ToonDefense.Spaceships
{
    public class ElectricityWorm : Spaceship
    {
        ParticleEmitter particleEmitter;
        public ElectricityWorm(Game game, Camera camera)
            : base(game, camera)
        {
            Name = "Electricity Worm";
            Speed = 1.4f;
            InitialHealth = 45000;
            Reward = 150;
            Position.Y = 1f;
        }

        protected override void LoadContent()
        {
            model = Game.Content.Load<Model>("models\\sciencevessel");
            texture = Game.Content.Load<Texture2D>("models\\sciencevesseltexture");
            effect = Game.Content.Load<Effect>("effects\\Toon").Clone();
            effect.Parameters["Texture"].SetValue(texture);

            particleEmitter = new ParticleEmitter(ElectricityBallParticleSystem.LastInstance, 20, Vector3.Zero);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            //ElectricityBallParticleSystem.LastInstance.AddParticle(Position, Vector3.Zero);
            particleEmitter.Update(gameTime, Position);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            DrawShadow();
            base.Draw(gameTime);
        }
    }
}
