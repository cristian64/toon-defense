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

namespace ToonDefense.Spaceships
{
    public class Spaceship : Object
    {
        protected Model model;
        protected Texture2D texture;
        protected Effect effect;
        private BoundingBox boundingBox;
        private bool boundingBoxCalculated;
        public List<Vector3> Destinations;
        public float Speed;
        public int Health;
        public int Reward;

        public Spaceship(Game game, Camera camera)
            :base(game, camera)
        {
            Destinations = new List<Vector3>();
            Speed = 1;
            Health = 100;
            Reward = 100;
        }

        public override void Update(GameTime gameTime)
        {
            if (Destinations.Count > 0 && gameTime.ElapsedGameTime.TotalMilliseconds > 0)
            {
                Vector2 position = new Vector2(Position.X, Position.Z);
                Vector2 destination = new Vector2(Destinations[0].X, Destinations[0].Z);
                Vector2 direction = destination - position;

                if (direction.LengthSquared() > Speed * Speed / gameTime.ElapsedGameTime.TotalMilliseconds / gameTime.ElapsedGameTime.TotalMilliseconds)
                {
                    direction.Normalize();
                    Position.X += direction.X * Speed / (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    Position.Z += direction.Y * Speed / (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    Rotation.Y = (float)Math.Atan2(-direction.Y, direction.X);
                }
                else
                {
                    Position.X = destination.X;
                    Position.Z = destination.Y;
                    Destinations.RemoveAt(0);
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Matrix world = Matrix.CreateScale(Scale) * Matrix.CreateTranslation(Position);
            PrimitiveDrawings.DrawBoundingBox(Game.GraphicsDevice, Camera, world, BoundingBox, Color.White);
            base.Draw(gameTime);
        }

        public BoundingBox BoundingBox
        {
            get
            {
                if (boundingBoxCalculated == false)
                {
                    // Initialize minimum and maximum corners of the bounding box to max and min values
                    Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
                    Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

                    // For each mesh of the model
                    foreach (ModelMesh mesh in model.Meshes)
                    {
                        foreach (ModelMeshPart meshPart in mesh.MeshParts)
                        {
                            // Vertex buffer parameters
                            int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                            int vertexBufferSize = meshPart.NumVertices * vertexStride;

                            // Get vertex data as float
                            float[] vertexData = new float[vertexBufferSize / sizeof(float)];
                            meshPart.VertexBuffer.GetData<float>(vertexData);

                            // Iterate through vertices (possibly) growing bounding box, all calculations are done in world space
                            for (int i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float))
                            {
                                Vector3 transformedPosition = Vector3.Transform(new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]), Matrix.Identity);

                                min = Vector3.Min(min, transformedPosition);
                                max = Vector3.Max(max, transformedPosition);
                            }
                        }
                    }

                    // Create and return bounding box
                    boundingBox = new BoundingBox(min, max);
                    boundingBoxCalculated = true;
                }

                return boundingBox;
            }
        }
    }
}
