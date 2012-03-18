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
    public class MissileLauncher : Tower
    {
        SoundEffect sound;
        public MissileLauncher(Game game, Camera camera)
            : base(game, camera)
        {
            Name = "Missile Launcher";
            Sight = 5;
            Damage = 70;
            Delay = 700;
            Price = 500;
            UpgradePrice = 650;
            Upgraded = false;
        }

        public override void Upgrade()
        {
            Sight = 8;
            Damage = 200;
            Delay = 1000;
            base.Upgrade();
        }

        protected override void LoadContent()
        {
            model = Game.Content.Load<Model>("models\\missilelauncher");
            texture = Game.Content.Load<Texture2D>("models\\missilelaunchertexture");
            effect = Game.Content.Load<Effect>("effects\\Toon").Clone();
            effect.Parameters["Texture"].SetValue(texture);
            sound = Game.Content.Load<SoundEffect>("sounds\\missileout");

            Position.Y = Height / 2.0f;

            base.LoadContent();
        }

        public override void Shoot()
        {
            Missile missile = new Missile(Game, Camera);
            missile.Initialize();
            missile.Target = Target;
            missile.Damage = Damage;
            Vector3 direction = Vector3.Normalize(Target.Position - Position);
            direction.Y = 0;
            missile.Position = Position + new Vector3(0, Height - 0.5f, 0) + 0.3f * direction;
            GameplayComponent.LastInstance.SpawnComponents.Add(missile);
            sound.Play();
            base.Shoot();
        }

        public override void Update(GameTime gameTime)
        {
            if (Target != null)
            {
                Vector3 direction = Target.Position - Position;
                Rotation.Y = (float)Math.Atan2(-direction.Z, direction.X);
            }
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

            world = Matrix.CreateScale(Scale) * Matrix.CreateRotationY(Rotation.Y) * Matrix.CreateTranslation(Position);
            foreach (ModelMeshPart part in model.Meshes[1].MeshParts)
            {
                part.Effect = effect;
                effect.Parameters["World"].SetValue(world);
                effect.Parameters["View"].SetValue(Camera.View);
                effect.Parameters["Projection"].SetValue(Camera.Projection);
                effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(world)));
            }
            model.Meshes[1].Draw();

            base.Draw(gameTime);
        }
    }
}
