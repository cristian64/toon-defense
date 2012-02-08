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

namespace ToonDefense.Spaceships
{
    public class Unidentified : Spaceship
    {
        Model model;
        Texture2D texture;
        Effect effect;
        float extraRotationY;

        public Unidentified(Game game, Camera camera)
            : base(game, camera)
        {
            extraRotationY = 0;
        }

        protected override void LoadContent()
        {
            model = Game.Content.Load<Model>("models\\unidentified");
            texture = Game.Content.Load<Texture2D>("models\\unidentifiedtexture");
            effect = Game.Content.Load<Effect>("effects\\Toon").Clone();
            effect.Parameters["Texture"].SetValue(texture);
            effect.Parameters["LineThickness"].SetValue(0.01f);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            extraRotationY += MathHelper.Pi / 0.5f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Vector3 shadowPosition = Position;
            shadowPosition.Y = 0;
            DrawShadow(shadowPosition, 1);
            Matrix world = Matrix.CreateScale(1.5f) * Matrix.CreateRotationY(extraRotationY) * Matrix.CreateTranslation(Position);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effect;
                    effect.Parameters["World"].SetValue(world);
                    effect.Parameters["View"].SetValue(Camera.View);
                    effect.Parameters["Projection"].SetValue(Camera.Projection);
                    effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(world)));
                }
                mesh.Draw();
            }

            base.Draw(gameTime);
        }
    }
}
