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

namespace ToonDefense
{
    public class Axis : Object
    {
        VertexPositionColor[] vertices;
        BasicEffect basicEffect;
        float length;

        public Axis(Game game, Camera camera)
            : base(game, camera)
        {
            vertices = new VertexPositionColor[2];
            length = 10.0f;
        }

        protected override void LoadContent()
        {
            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.VertexColorEnabled = true;
            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            basicEffect.Projection = Camera.Projection;
            basicEffect.View = Camera.View;
            basicEffect.CurrentTechnique.Passes[0].Apply();
            drawLine(new Vector3(0, 0, 0), new Vector3(length, 0, 0), Color.Red);
            drawLine(new Vector3(length - 0.5f, 0.5f * 0.4f, 0), new Vector3(length, 0, 0), Color.Red);
            drawLine(new Vector3(length - 0.5f, -0.5f * 0.4f, 0), new Vector3(length, 0, 0), Color.Red);

            drawLine(new Vector3(0, 0, 0), new Vector3(0, length, 0), Color.Green);
            drawLine(new Vector3(0.5f * 0.4f, length - 0.5f, 0), new Vector3(0, length, 0), Color.Green);
            drawLine(new Vector3(-0.5f * 0.4f, length - 0.5f, 0), new Vector3(0, length, 0), Color.Green);

            drawLine(new Vector3(0, 0, 0), new Vector3(0, 0, length), Color.Blue);
            drawLine(new Vector3(0, 0.5f * 0.4f, length - 0.5f), new Vector3(0, 0, length), Color.Blue);
            drawLine(new Vector3(0, -0.5f * 0.4f, length - 0.5f), new Vector3(0, 0, length), Color.Blue);
            base.Draw(gameTime);
        }

        private void drawLine(Vector3 source, Vector3 destination, Color color)
        {
            vertices[0].Position = source;
            vertices[0].Color = color;
            vertices[1].Position = destination;
            vertices[1].Color = color;
            Game.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, 1);
        }
    }
}
