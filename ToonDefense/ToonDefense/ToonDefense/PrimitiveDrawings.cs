using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ToonDefense
{
    public class PrimitiveDrawings
    {
        private static BasicEffect basicEffect;
        private static VertexPositionColor[] vertices = new VertexPositionColor[2];

        public static void DrawLine(GraphicsDevice device, Camera camera, Vector3 source, Vector3 destination, Color color)
        {
            if (basicEffect == null)
            {
                basicEffect = new BasicEffect(device);
                basicEffect.VertexColorEnabled = true;
            }

            basicEffect.Projection = camera.Projection;
            basicEffect.View = camera.View;
            basicEffect.CurrentTechnique.Passes[0].Apply();
            vertices[0].Position = source;
            vertices[0].Color = color;
            vertices[1].Position = destination;
            vertices[1].Color = color;
            basicEffect.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, 1);
        }
    }
}
