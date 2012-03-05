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
        private static VertexPositionColor[] verticesTriangle = new VertexPositionColor[9];

        public static void DrawLine(GraphicsDevice device, Camera camera, Matrix world, Vector3 source, Vector3 destination, Color color)
        {
            if (basicEffect == null)
            {
                basicEffect = new BasicEffect(device);
                basicEffect.LightingEnabled = false;
                basicEffect.VertexColorEnabled = true;
            }

            basicEffect.Projection = camera.Projection;
            basicEffect.View = camera.View;
            basicEffect.World = world;
            basicEffect.CurrentTechnique.Passes[0].Apply();
            vertices[0].Position = source;
            vertices[0].Color = color;
            vertices[1].Position = destination;
            vertices[1].Color = color;
            basicEffect.GraphicsDevice.BlendState = BlendState.Additive;
            basicEffect.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            basicEffect.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, 1);
            basicEffect.GraphicsDevice.BlendState = BlendState.AlphaBlend;
        }

        public static void DrawLine(GraphicsDevice device, Camera camera, Vector3 source, Vector3 destination, Color color)
        {
            DrawLine(device, camera, Matrix.Identity, source, destination, color);
        }

        public static void DrawPyramid(GraphicsDevice device, Camera camera, Vector3 source, Vector3 destination, Color color)
        {
            if (basicEffect == null)
            {
                basicEffect = new BasicEffect(device);
                basicEffect.LightingEnabled = false;
                basicEffect.VertexColorEnabled = true;
            }

            basicEffect.Projection = camera.Projection;
            basicEffect.View = camera.View;
            basicEffect.World = Matrix.Identity;
            basicEffect.CurrentTechnique.Passes[0].Apply();

            Vector3 direction = Vector3.Normalize(destination - source);
            Vector3 normal1 = new Vector3(direction.Z, direction.Y, -direction.X);
            Vector3 normal2 = new Vector3(direction.X, direction.Z, -direction.Y);

            verticesTriangle[0].Position = source;
            verticesTriangle[0].Color = color;
            verticesTriangle[1].Position = destination + 0.1f * normal1;
            verticesTriangle[1].Color = color;
            verticesTriangle[2].Position = destination - 0.1f * normal2;
            verticesTriangle[2].Color = color;
            verticesTriangle[3].Position = source;
            verticesTriangle[3].Color = color;
            verticesTriangle[4].Position = destination - 0.1f * normal2;
            verticesTriangle[4].Color = color;
            verticesTriangle[5].Position = destination - 0.1f * normal1;
            verticesTriangle[5].Color = color;
            verticesTriangle[6].Position = destination + 0.1f * normal1;
            verticesTriangle[6].Color = color;
            verticesTriangle[7].Position = source;
            verticesTriangle[7].Color = color;
            verticesTriangle[8].Position = destination - 0.1f * normal1;
            verticesTriangle[8].Color = color;

            basicEffect.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            basicEffect.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            device.DrawUserPrimitives(PrimitiveType.TriangleList, verticesTriangle, 0, 3, VertexPositionColor.VertexDeclaration);
        }

        public static void DrawBoundingBox(GraphicsDevice device, Camera camera, Matrix world, BoundingBox boundingBox, Color color)
        {
            Vector3 vertex1 = boundingBox.Min;
            Vector3 vertex2 = boundingBox.Min;
            vertex2.X = boundingBox.Max.X;
            Vector3 vertex3 = boundingBox.Min;
            vertex3.X = boundingBox.Max.X;
            vertex3.Z = boundingBox.Max.Z;
            Vector3 vertex4 = boundingBox.Min;
            vertex4.Z = boundingBox.Max.Z;

            Vector3 vertex5 = boundingBox.Max;
            Vector3 vertex6 = boundingBox.Max;
            vertex6.X = boundingBox.Min.X;
            Vector3 vertex7 = boundingBox.Max;
            vertex7.X = boundingBox.Min.X;
            vertex7.Z = boundingBox.Min.Z;
            Vector3 vertex8 = boundingBox.Max;
            vertex8.Z = boundingBox.Min.Z;

            DrawLine(device, camera, world, vertex1, vertex2, color);
            DrawLine(device, camera, world, vertex2, vertex3, color);
            DrawLine(device, camera, world, vertex3, vertex4, color);
            DrawLine(device, camera, world, vertex4, vertex1, color);
            DrawLine(device, camera, world, vertex5, vertex6, color);
            DrawLine(device, camera, world, vertex6, vertex7, color);
            DrawLine(device, camera, world, vertex7, vertex8, color);
            DrawLine(device, camera, world, vertex8, vertex5, color);
            DrawLine(device, camera, world, vertex1, vertex7, color);
            DrawLine(device, camera, world, vertex2, vertex8, color);
            DrawLine(device, camera, world, vertex3, vertex5, color);
            DrawLine(device, camera, world, vertex4, vertex6, color);
        }

        public static void DrawSphere(Game game, GraphicsDevice device, Camera camera, Vector3 position, float scale)
        {
            device.DepthStencilState = DepthStencilState.DepthRead;
            device.BlendState = BlendState.Additive;
            Matrix world = Matrix.CreateScale(scale) * Matrix.CreateScale(2) * Matrix.CreateTranslation(position);

            foreach (ModelMesh mm in game.Content.Load<Model>("models\\sphere").Meshes)
            {
                foreach (BasicEffect e in mm.Effects)
                {
                    e.Projection = camera.Projection;
                    e.View = camera.View;
                    e.World = world;
                    e.Alpha = 0.25f;
                }
                mm.Draw();
            }
            device.BlendState = BlendState.AlphaBlend;
            device.DepthStencilState = DepthStencilState.Default;
        }
    }
}
