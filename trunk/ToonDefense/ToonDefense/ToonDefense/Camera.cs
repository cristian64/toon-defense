using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ToonDefense
{
    /// <summary>
    /// First person camera component for the demos, rotated by mouse.
    /// </summary>
    public class Camera : DrawableGameComponent
    {
        public World World;

        private Matrix view;
        private Matrix projection;

        private float height = 40;
        private float differenceZ = 50;
        private Vector3 target = Vector3.Zero;
        private Vector3 position = new Vector3(0, 0, 10);
        private Vector2 angles = Vector2.Zero;

        private int widthOver2;
        private int heightOver2;

        private float fieldOfView = Microsoft.Xna.Framework.MathHelper.PiOver4;
        private float aspectRatio;
        private float nearPlaneDistance = 0.1f;
        private float farPlaneDistance = 10000.0f;

        public int GrabbingX = 0;
        public int GrabbingY = 0;
        public bool Grabbing = false;
        private MouseState prevMouseState = new MouseState();

        /// <summary>
        /// Initializes new camera component.
        /// </summary>
        /// <param name="game">Game to which attach this camera.</param>
        public Camera(Game game)
            : base(game)
        {
            widthOver2 = Game.GraphicsDevice.PresentationParameters.BackBufferWidth / 2;
            heightOver2 = Game.GraphicsDevice.PresentationParameters.BackBufferHeight / 2;
            aspectRatio = Game.GraphicsDevice.PresentationParameters.BackBufferWidth / (float)Game.GraphicsDevice.PresentationParameters.BackBufferHeight;
            UpdateProjection();
            FreeCamera = false;
        }

        /// <summary>
        /// Gets camera view matrix.
        /// </summary>
        public Matrix View { get { return view; } set { view = value; } }
        /// <summary>
        /// Gets or sets camera projection matrix.
        /// </summary>
        public Matrix Projection { get { return projection; } set { projection = value; } }
        /// <summary>
        /// Gets camera view matrix multiplied by projection matrix.
        /// </summary>
        public Matrix ViewProjection { get { return view * projection; } }

        /// <summary>
        /// Gets or sets camera position.
        /// </summary>
        public Vector3 Position { get { return position; } set { position = value; } }

        /// <summary>
        /// Gets or sets camera field of view.
        /// </summary>
        public float FieldOfView { get { return fieldOfView; } set { fieldOfView = value; UpdateProjection(); } }
        /// <summary>
        /// Gets or sets camera aspect ratio.
        /// </summary>
        public float AspectRatio { get { return aspectRatio; } set { aspectRatio = value; UpdateProjection(); } }
        /// <summary>
        /// Gets or sets camera near plane distance.
        /// </summary>
        public float NearPlaneDistance { get { return nearPlaneDistance; } set { nearPlaneDistance = value; UpdateProjection(); } }
        /// <summary>
        /// Gets or sets camera far plane distance.
        /// </summary>
        public float FarPlaneDistance { get { return farPlaneDistance; } set { farPlaneDistance = value; UpdateProjection(); } }

        /// <summary>
        /// Gets or sets camera's target.
        /// </summary>
        public Vector3 Target2
        {
            get
            {
                Matrix cameraRotation = Matrix.CreateRotationX(angles.X) * Matrix.CreateRotationY(angles.Y);
                return position + Vector3.Transform(Vector3.Forward, cameraRotation);
            }
            set
            {
                Vector3 forward = Vector3.Normalize(position - value);
                Vector3 right = Vector3.Normalize(Vector3.Cross(forward, Vector3.Up));
                Vector3 up = Vector3.Normalize(Vector3.Cross(right, forward));

                Matrix test = Matrix.Identity;
                test.Forward = forward;
                test.Right = right;
                test.Up = up;
                angles.X = -(float)Math.Asin(test.M32);
                angles.Y = -(float)Math.Asin(test.M13);
            }
        }

        /// <summary>
        /// Updates camera with input and updates view matrix.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            MouseState currentMouseState = Mouse.GetState();
            if (Game.IsActive && Enabled)
            {
                aspectRatio = Game.GraphicsDevice.PresentationParameters.BackBufferWidth / (float)Game.GraphicsDevice.PresentationParameters.BackBufferHeight;
                if (freeCamera)
                {
                    double elapsedTime = (double)gameTime.ElapsedGameTime.Ticks / (double)TimeSpan.TicksPerSecond;
                    ProcessInput((float)elapsedTime * 10.0f);
                    UpdateProjection();
                    UpdateView();
                }
                else
                {
                    float speed = 12;
                    if (GameplayComponent.LastInstance.SpeedLevel == SpeedLevel.FAST)
                        speed = 12/4.0f;
                    if (Keyboard.GetState().IsKeyDown(Keys.A) || Keyboard.GetState().IsKeyDown(Keys.Left) || currentMouseState.X == 0)
                    {
                        position.X -= speed * ((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000);
                        target.X -= speed * ((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000);
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.D) || Keyboard.GetState().IsKeyDown(Keys.Right) || currentMouseState.X == Game.GraphicsDevice.PresentationParameters.BackBufferWidth - 1)
                    {
                        position.X += speed * ((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000);
                        target.X += speed * ((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000);
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.W) || Keyboard.GetState().IsKeyDown(Keys.Up) || currentMouseState.Y == 0)
                    {
                        position.Z -= speed * ((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000);
                        target.Z -= speed * ((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000);
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.S) || Keyboard.GetState().IsKeyDown(Keys.Down) || currentMouseState.Y == Game.GraphicsDevice.PresentationParameters.BackBufferHeight - 1)
                    {
                        position.Z += speed * ((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000);
                        target.Z += speed * ((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000);
                    }

                    if (!Grabbing && BuildingPanel.LastInstance.Tower == null && currentMouseState.LeftButton == ButtonState.Pressed && (currentMouseState.X != prevMouseState.X || currentMouseState.Y != prevMouseState.Y))
                    {
                        Grabbing = true;
                        GrabbingX = currentMouseState.X;
                        GrabbingY = currentMouseState.Y;
                    }
                    else if (currentMouseState.LeftButton == ButtonState.Released)
                    {
                        Grabbing = false;
                    }
                    if (Grabbing && (currentMouseState.X != prevMouseState.X || currentMouseState.Y != prevMouseState.Y))
                    {
                        Vector3 currentPosition = RayFromScreenToFloor(currentMouseState.X, currentMouseState.Y);
                        Vector3 previousPosition = RayFromScreenToFloor(prevMouseState.X, prevMouseState.Y);

                        position.X -= currentPosition.X - previousPosition.X;
                        position.Z -= currentPosition.Z - previousPosition.Z;
                        target.X -= currentPosition.X - previousPosition.X;
                        target.Z -= currentPosition.Z - previousPosition.Z;
                    }

                    if (target.X > World.Scale.X / 2.0f)
                    {
                        target.X = World.Scale.X / 2.0f;
                        position.X = World.Scale.X / 2.0f;
                    }
                    if (target.X < -World.Scale.X / 2.0f)
                    {
                        target.X = -World.Scale.X / 2.0f;
                        position.X = -World.Scale.X / 2.0f;
                    }
                    if (target.Z > World.Scale.Z / 2.0f)
                    {
                        target.Z = World.Scale.Z / 2.0f;
                        position.Z = World.Scale.Z / 2.0f + differenceZ;
                    }
                    if (target.Z < -World.Scale.Z / 2.0f)
                    {
                        target.Z = -World.Scale.Z / 2.0f;
                        position.Z = -World.Scale.Z / 2.0f + differenceZ;
                    }

                    float increment = (prevMouseState.ScrollWheelValue - currentMouseState.ScrollWheelValue) / 100.0f;
                    if (increment > 0)
                    {
                        position = position - 5 * (Vector3.Normalize(target - position));
                        differenceZ = position.Z - target.Z;
                    }
                    else if (increment < 0)
                    {
                        if ((position + 5 * (Vector3.Normalize(target - position))).Y > 0)
                        {
                            position = position + 5 * (Vector3.Normalize(target - position));
                            differenceZ = position.Z - target.Z;
                        }
                    }

                    projection = Matrix.CreatePerspectiveFieldOfView(0.2f, aspectRatio, nearPlaneDistance, farPlaneDistance);
                    view = Matrix.CreateLookAt(position, target, Vector3.Up);

                    prevMouseState = Mouse.GetState();
                }

                base.Update(gameTime);
            }
        }

        public Vector3 Target
        {
            get
            {
                return target;
            }

            set
            {
                if (!freeCamera)
                {
                    target = value;
                    position.X = target.X;
                    position.Z = target.Z + differenceZ;
                }
            }
        }

        private bool freeCamera;
        public bool FreeCamera
        {
            get { return freeCamera; }
            set
            {
                freeCamera = value;
                if (freeCamera)
                {
                    position = new Vector3(5, 3, 10);
                    Target2 = new Vector3(8, 5.5f, 0);
                }
                else
                {
                    position = new Vector3(0, height, differenceZ);
                    target = new Vector3(0, 0, 0);
                }
            }
        }

        private void ProcessInput(float amountOfMovement)
        {
            Vector3 moveVector = new Vector3();

            KeyboardState keys = Keyboard.GetState();
            if (keys.IsKeyDown(Keys.D))
                moveVector.X += amountOfMovement;
            if (keys.IsKeyDown(Keys.A))
                moveVector.X -= amountOfMovement;
            if (keys.IsKeyDown(Keys.S))
                moveVector.Z += amountOfMovement;
            if (keys.IsKeyDown(Keys.W))
                moveVector.Z -= amountOfMovement;

            Matrix cameraRotation = Matrix.CreateRotationX(angles.X) * Matrix.CreateRotationY(angles.Y);
            position += Vector3.Transform(moveVector, cameraRotation);

            MouseState currentMouseState = Mouse.GetState();

            if (currentMouseState.RightButton == ButtonState.Pressed && prevMouseState.RightButton == ButtonState.Released)
            {
                Mouse.SetPosition(widthOver2, heightOver2);
            }
            else if (currentMouseState.RightButton == ButtonState.Pressed)
            {
                if (currentMouseState.X != widthOver2)
                    angles.Y -= amountOfMovement / 80.0f * (currentMouseState.X - widthOver2);
                if (currentMouseState.Y != heightOver2)
                    angles.X -= amountOfMovement / 80.0f * (currentMouseState.Y - heightOver2);

                if (angles.X > 1.4) angles.X = 1.4f;
                if (angles.X < -1.4) angles.X = -1.4f;
                if (angles.Y > Math.PI) angles.Y -= 2 * (float)Math.PI;
                if (angles.Y < -Math.PI) angles.Y += 2 * (float)Math.PI;

                Mouse.SetPosition(widthOver2, heightOver2);
            }

            prevMouseState = currentMouseState;
        }

        private void UpdateProjection()
        {
            projection = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance);
        }

        private void UpdateView()
        {
            Matrix cameraRotation = Matrix.CreateRotationX(angles.X) * Matrix.CreateRotationY(angles.Y);
            Vector3 targetPos = position + Vector3.Transform(Vector3.Forward, cameraRotation);

            Vector3 upVector = Vector3.Transform(Vector3.Up, cameraRotation);

            view = Matrix.CreateLookAt(position, targetPos, upVector);
        }

        /// <summary>
        /// Gets a direction from the screen/camera to the world, according to the position (x, y) of the screen.
        /// </summary>
        /// <param name="x">Position X on the screen</param>
        /// <param name="y">Position Y on the screen</param>
        /// <returns>Direction from the screen to the world.</returns>
        public Vector3 RayFromScreenToWorld(int x, int y)
        {
            Vector3 nearSource = new Vector3(x, y, 0);
            Vector3 farSource = new Vector3(x, y, 1);

            Vector3 nearPoint = Game.GraphicsDevice.Viewport.Unproject(nearSource, Projection, View, Matrix.Identity);
            Vector3 farPoint = Game.GraphicsDevice.Viewport.Unproject(farSource, Projection, View, Matrix.Identity);

            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            return direction;
        }

        public Ray RayFromScreenToWorldRay(int x, int y)
        {
            Vector3 nearSource = new Vector3(x, y, 0);
            Vector3 farSource = new Vector3(x, y, 1);

            Vector3 nearPoint = Game.GraphicsDevice.Viewport.Unproject(nearSource, Projection, View, Matrix.Identity);
            Vector3 farPoint = Game.GraphicsDevice.Viewport.Unproject(farSource, Projection, View, Matrix.Identity);

            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            Ray pickRay = new Ray(nearPoint, direction);
            return pickRay;
        }

        public Vector3 RayFromScreenToFloor(int x, int y)
        {
            Vector3 direction = RayFromScreenToWorld(x, y);
            Vector3 lineStart = direction + position;
            Vector3 lineEnd = position;
            Vector3 result = new Vector3();
            result.X = lineStart.X + (lineStart.X - lineEnd.X) * lineStart.Y / (lineEnd.Y - lineStart.Y);
            result.Z = lineStart.Z + (lineStart.Z - lineEnd.Z) * lineStart.Y / (lineEnd.Y - lineStart.Y);
            return result;
        }

        public Vector3 RayFromWorldToScreen(Vector3 position)
        {
            return Game.GraphicsDevice.Viewport.Project(position, Projection, View, Matrix.Identity);
        }

        public Vector2 RayFromWorldToScreen2(Vector3 position)
        {
            Vector3 aux = Game.GraphicsDevice.Viewport.Project(position, Projection, View, Matrix.Identity);
            return new Vector2(aux.X, aux.Y);
        }
    }
}
