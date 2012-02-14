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
                }
                else
                {
                    float[] lineColor = new float[4];
                    lineColor[0] = 0.0f;
                    lineColor[1] = 0.0f;
                    lineColor[2] = 0.0f;
                    lineColor[3] = 1.0f;
                    effect.Parameters["LineColor"].SetValue(lineColor);
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
            #if DEBUG
            Matrix world = Matrix.CreateScale(Scale) * Matrix.CreateTranslation(Position);
            PrimitiveDrawings.DrawBoundingBox(Game.GraphicsDevice, Camera, world, BoundingBox, Color.White);
            #endif
            base.Draw(gameTime);
        }

        public void DrawShadow(Vector3 position, float scale)
        {
            Matrix world = Matrix.CreateScale(scale * 0.5f) * Matrix.CreateTranslation(position);
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            foreach (ModelMesh mesh in shadowModel.Meshes)
            {
                foreach (BasicEffect e in mesh.Effects)
                {
                    e.Projection = Camera.Projection;
                    e.View = Camera.View;
                    e.World = world;
                }
                mesh.Draw();
            }
            GraphicsDevice.BlendState = BlendState.Opaque;
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
    }
}
