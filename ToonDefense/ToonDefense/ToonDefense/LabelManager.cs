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
using ToonDefense.Towers;
using ToonDefense.Spaceships;

namespace ToonDefense
{
    public class LabelManager : DrawableGameComponent
    {
        private class Label
        {
            public string Text;
            public float Duration;
            public Vector3 Position;
            public Color Color;
        }

        public static LabelManager LastInstance = null;
        List<Label> labels;
        Camera camera;
        World world;
        SpriteBatch spriteBatch;
        SpriteFont font;

        public LabelManager(Game game, Camera camera, World world)
            : base(game)
        {
            LastInstance = this;

            this.camera = camera;
            this.world = world;
            labels = new List<Label>();

            Label hey = new Label();
            hey.Text = "pollaca";
            hey.Position = Vector3.Up;
            hey.Color = Color.Indigo;
            labels.Add(hey);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Game.Content.Load<SpriteFont>("fonts\\label");
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (GameplayComponent.LastInstance.SpeedLevel != SpeedLevel.PAUSED)
                for (int i = labels.Count - 1; i >= 0; i--)
                {
                    Label label = labels[i];
                    label.Duration -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    label.Color = label.Color * Math.Min(1.0f, (label.Duration / 1000.0f));
                    label.Position += new Vector3(0, 0.005f, 0);

                    if (label.Duration <= 0)
                        labels.RemoveAt(i);
                }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            foreach (Label i in labels)
            {
                Vector2 position = camera.RayFromWorldToScreen2(i.Position) - font.MeasureString(i.Text) / 2;
                Color shadow = Color.Black * i.Color.ToVector4().W;
                spriteBatch.DrawString(font, i.Text, position + Vector2.UnitX, shadow);
                spriteBatch.DrawString(font, i.Text, position - Vector2.UnitX, shadow);
                spriteBatch.DrawString(font, i.Text, position + Vector2.UnitY, shadow);
                spriteBatch.DrawString(font, i.Text, position - Vector2.UnitY, shadow);
                spriteBatch.DrawString(font, i.Text, position, i.Color);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        public void AddLabel(String text, int duration, Vector3 position, Color color)
        {
            Label label = new Label();
            label.Text = text;
            label.Duration = duration;
            label.Position = position;
            label.Color = color;
            labels.Add(label);
        }
    }
}