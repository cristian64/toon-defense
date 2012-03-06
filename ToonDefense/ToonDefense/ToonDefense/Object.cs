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
    public class Object : DrawableGameComponent
    {
        public Camera Camera;
        public Vector3 Position;
        public Vector3 Scale;
        public Vector3 Rotation;

        protected Model model;
        protected Texture2D texture;
        protected Effect effect;
        private BoundingBox boundingBox;
        private bool boundingBoxCalculated;

        public Model shadowModel;
        public Texture2D shadowTexture;
        public BasicEffect shadowEffect;

        private bool selected;
        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                if (selected)
                {
                    float[] lineColor = new float[4];
                    lineColor[0] = 173 / 255.0f;
                    lineColor[1] = 1.0f;
                    lineColor[2] = 47 / 255.0f;
                    lineColor[3] = 1.0f;
                    effect.Parameters["LineColor"].SetValue(lineColor);
                    effect.Parameters["DiffuseIntensity"].SetValue(10.0f);
                    effect.Parameters["LineThickness"].SetValue(0.04f);
                }
                else
                {
                    float[] lineColor = new float[4];
                    lineColor[0] = 0.0f;
                    lineColor[1] = 0.0f;
                    lineColor[2] = 0.0f;
                    lineColor[3] = 1.0f;
                    effect.Parameters["LineColor"].SetValue(lineColor);
                    effect.Parameters["DiffuseIntensity"].SetValue(3.0f);
                    effect.Parameters["LineThickness"].SetValue(0.02f);
                }
            }
        }

        public Object(Game game, Camera camera)
            : base(game)
        {
            Camera = camera;
            Position = Vector3.Zero;
            Scale = Vector3.One;
            Rotation = Vector3.Zero;
        }

        protected override void LoadContent()
        {
            shadowModel = Game.Content.Load<Model>("models\\shadow");
            shadowTexture = Game.Content.Load<Texture2D>("models\\shadowtexture");
            shadowEffect = (BasicEffect)shadowModel.Meshes[0].Effects[0];
            shadowEffect.Texture = Game.Content.Load<Texture2D>("models\\shadowtexture");
            shadowEffect.TextureEnabled = true;

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            if (ToonDefense.Debug)
            {
                Matrix world = Matrix.CreateScale(Scale) * Matrix.CreateTranslation(Position);
                PrimitiveDrawings.DrawBoundingBox(Game.GraphicsDevice, Camera, world, BoundingBox, Color.White);
            }
            base.Draw(gameTime);
        }

        public void DrawShadow()
        {
            Vector3 position = Position;
            Vector3 scale = Scale * Math.Max(0.5f, (1 - (Position.Y / 3.0f))) * (float)Math.Sqrt(Width * Depth);
            position.Y = 0;

            Matrix world = Matrix.CreateScale(scale) * Matrix.CreateRotationY(Rotation.Y) * Matrix.CreateTranslation(position);
            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            foreach (ModelMesh mesh in shadowModel.Meshes)
            {
                foreach (BasicEffect e in mesh.Effects)
                {
                    e.Projection = Camera.Projection;
                    e.View = Camera.View;
                    e.World = world;
                    e.Alpha = Math.Max(0.5f, (1 - (Position.Y / 3.0f))) * Math.Max(0.5f, (1 - (Position.Y / 3.0f)));
                }
                mesh.Draw();
            }
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }

        public float Height
        {
            get
            {
                return BoundingBox.Max.Y - BoundingBox.Min.Y;
            }
        }

        public float Width
        {
            get
            {
                return BoundingBox.Max.X - BoundingBox.Min.X;
            }
        }

        public float Depth
        {
            get
            {
                return BoundingBox.Max.Z - BoundingBox.Min.Z;
            }
        }

        public BoundingBox BoundingBox
        {
            get
            {
                if (boundingBoxCalculated == false)
                {
                    boundingBox = new BoundingBox(Vector3.One, Vector3.Zero);
                    if (model != null)
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
                    }
                    boundingBoxCalculated = true;
                }

                return boundingBox;
            }
        }

        public float? IntersectsBoundingSphere(Ray ray)
        {
            foreach (ModelMesh i in model.Meshes)
            {
                BoundingSphere boundingSphere = i.BoundingSphere;
                boundingSphere.Center += Position;
                float? intersection = boundingSphere.Intersects(ray);
                if (intersection != null)
                    return intersection;
            }
            return null;
        }

        public float? Intersects(Ray ray)
        {
            bool insideBoundingSphere;
            Vector3 vertex1, vertex2, vertex3;
            Matrix world = Matrix.CreateScale(Scale) * Matrix.CreateRotationX(Rotation.X) * Matrix.CreateRotationY(Rotation.Y) * Matrix.CreateTranslation(Position);
            float? intersection = RayIntersectsModel(ray, model,
                                                     world,
                                                     out insideBoundingSphere,
                                                     out vertex1, out vertex2,
                                                     out vertex3);

            return intersection;
        }

        /// <summary>
        /// Checks whether a ray intersects a model. This method needs to access
        /// the model vertex data, so the model must have been built using the
        /// custom TrianglePickingProcessor provided as part of this sample.
        /// Returns the distance along the ray to the point of intersection, or null
        /// if there is no intersection.
        /// </summary>
        static float? RayIntersectsModel(Ray ray, Model model, Matrix modelTransform,
                                         out bool insideBoundingSphere,
                                         out Vector3 vertex1, out Vector3 vertex2,
                                         out Vector3 vertex3)
        {
            vertex1 = vertex2 = vertex3 = Vector3.Zero;

            // The input ray is in world space, but our model data is stored in object
            // space. We would normally have to transform all the model data by the
            // modelTransform matrix, moving it into world space before we test it
            // against the ray. That transform can be slow if there are a lot of
            // triangles in the model, however, so instead we do the opposite.
            // Transforming our ray by the inverse modelTransform moves it into object
            // space, where we can test it directly against our model data. Since there
            // is only one ray but typically many triangles, doing things this way
            // around can be much faster.

            Matrix inverseTransform = Matrix.Invert(modelTransform);

            ray.Position = Vector3.Transform(ray.Position, inverseTransform);
            ray.Direction = Vector3.TransformNormal(ray.Direction, inverseTransform);

            // Look up our custom collision data from the Tag property of the model.
            Dictionary<string, object> tagData = (Dictionary<string, object>)model.Tag;

            if (tagData == null)
            {
                throw new InvalidOperationException(
                    "Model.Tag is not set correctly. Make sure your model " +
                    "was built using the custom TrianglePickingProcessor.");
            }

            // Start off with a fast bounding sphere test.
            BoundingSphere boundingSphere = (BoundingSphere)tagData["BoundingSphere"];

            if (boundingSphere.Intersects(ray) == null)
            {
                // If the ray does not intersect the bounding sphere, we cannot
                // possibly have picked this model, so there is no need to even
                // bother looking at the individual triangle data.
                insideBoundingSphere = false;

                return null;
            }
            else
            {
                // The bounding sphere test passed, so we need to do a full
                // triangle picking test.
                insideBoundingSphere = true;

                // Keep track of the closest triangle we found so far,
                // so we can always return the closest one.
                float? closestIntersection = null;

                // Loop over the vertex data, 3 at a time (3 vertices = 1 triangle).
                Vector3[] vertices = (Vector3[])tagData["Vertices"];

                for (int i = 0; i < vertices.Length; i += 3)
                {
                    // Perform a ray to triangle intersection test.
                    float? intersection;

                    RayIntersectsTriangle(ref ray,
                                          ref vertices[i],
                                          ref vertices[i + 1],
                                          ref vertices[i + 2],
                                          out intersection);

                    // Does the ray intersect this triangle?
                    if (intersection != null)
                    {
                        // If so, is it closer than any other previous triangle?
                        if ((closestIntersection == null) ||
                            (intersection < closestIntersection))
                        {
                            // Store the distance to this triangle.
                            closestIntersection = intersection;

                            // Transform the three vertex positions into world space,
                            // and store them into the output vertex parameters.
                            Vector3.Transform(ref vertices[i],
                                              ref modelTransform, out vertex1);

                            Vector3.Transform(ref vertices[i + 1],
                                              ref modelTransform, out vertex2);

                            Vector3.Transform(ref vertices[i + 2],
                                              ref modelTransform, out vertex3);
                        }
                    }
                }

                return closestIntersection;
            }
        }


        /// <summary>
        /// Checks whether a ray intersects a triangle. This uses the algorithm
        /// developed by Tomas Moller and Ben Trumbore, which was published in the
        /// Journal of Graphics Tools, volume 2, "Fast, Minimum Storage Ray-Triangle
        /// Intersection".
        /// 
        /// This method is implemented using the pass-by-reference versions of the
        /// XNA math functions. Using these overloads is generally not recommended,
        /// because they make the code less readable than the normal pass-by-value
        /// versions. This method can be called very frequently in a tight inner loop,
        /// however, so in this particular case the performance benefits from passing
        /// everything by reference outweigh the loss of readability.
        /// </summary>
        static void RayIntersectsTriangle(ref Ray ray,
                                          ref Vector3 vertex1,
                                          ref Vector3 vertex2,
                                          ref Vector3 vertex3, out float? result)
        {
            // Compute vectors along two edges of the triangle.
            Vector3 edge1, edge2;

            Vector3.Subtract(ref vertex2, ref vertex1, out edge1);
            Vector3.Subtract(ref vertex3, ref vertex1, out edge2);

            // Compute the determinant.
            Vector3 directionCrossEdge2;
            Vector3.Cross(ref ray.Direction, ref edge2, out directionCrossEdge2);

            float determinant;
            Vector3.Dot(ref edge1, ref directionCrossEdge2, out determinant);

            // If the ray is parallel to the triangle plane, there is no collision.
            if (determinant > -float.Epsilon && determinant < float.Epsilon)
            {
                result = null;
                return;
            }

            float inverseDeterminant = 1.0f / determinant;

            // Calculate the U parameter of the intersection point.
            Vector3 distanceVector;
            Vector3.Subtract(ref ray.Position, ref vertex1, out distanceVector);

            float triangleU;
            Vector3.Dot(ref distanceVector, ref directionCrossEdge2, out triangleU);
            triangleU *= inverseDeterminant;

            // Make sure it is inside the triangle.
            if (triangleU < 0 || triangleU > 1)
            {
                result = null;
                return;
            }

            // Calculate the V parameter of the intersection point.
            Vector3 distanceCrossEdge1;
            Vector3.Cross(ref distanceVector, ref edge1, out distanceCrossEdge1);

            float triangleV;
            Vector3.Dot(ref ray.Direction, ref distanceCrossEdge1, out triangleV);
            triangleV *= inverseDeterminant;

            // Make sure it is inside the triangle.
            if (triangleV < 0 || triangleU + triangleV > 1)
            {
                result = null;
                return;
            }

            // Compute the distance along the ray to the triangle.
            float rayDistance;
            Vector3.Dot(ref edge2, ref distanceCrossEdge1, out rayDistance);
            rayDistance *= inverseDeterminant;

            // Is the triangle behind the ray origin?
            if (rayDistance < 0)
            {
                result = null;
                return;
            }

            result = rayDistance;
        }

    }
}
