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
using ToonDefense.Projectiles;

namespace ToonDefense.Towers
{
    public class PlasmaGenerator : Tower
    {
        SoundEffect sound;
        public PlasmaGenerator(Game game, Camera camera)
            : base(game, camera)
        {
            Name = "Plasma Generator";
            Sight = 5;
            Damage = 100;
            Delay = 1000;
            Price = 500;
        }

        protected override void LoadContent()
        {
            model = Game.Content.Load<Model>("models\\plasmagenerator");
            texture = Game.Content.Load<Texture2D>("models\\plasmageneratortexture");
            effect = Game.Content.Load<Effect>("effects\\Toon").Clone();
            effect.Parameters["Texture"].SetValue(texture);
            sound = Game.Content.Load<SoundEffect>("sounds\\plasmaout");

            Position.Y = Height / 2.0f;

            base.LoadContent();
        }

        public override void Shoot()
        {
            Plasma plasma = new Plasma(Game, Camera);
            plasma.Initialize();
            plasma.Target = Target;
            plasma.Damage = Damage;
            plasma.Position = Position + new Vector3(-0.55f, Height - 0.4f, 0);
            GameplayComponent.LastInstance.SpawnComponents.Add(plasma);
            sound.Play();
            base.Shoot();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Matrix world = Matrix.CreateScale(Scale) * Matrix.CreateTranslation(Position);
            foreach (ModelMeshPart part in model.Meshes[0].MeshParts)
            {
                part.Effect = effect;
                effect.Parameters["World"].SetValue(world);
                effect.Parameters["View"].SetValue(Camera.View);
                effect.Parameters["Projection"].SetValue(Camera.Projection);
                effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(world)));
            }
            model.Meshes[0].Draw();

            base.Draw(gameTime);
        }
    }
}
