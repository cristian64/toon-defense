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

        public Object(Game game, Camera camera)
            : base(game)
        {
            Camera = camera;
            Position = new Vector3();
            Scale = new Vector3();
            Rotation = new Vector3();
        }
    }
}
