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
        public Model model;
        public Texture2D texture;
        public Effect effect;
        public Vector3 Position;
        public Vector3 Scale;
        public Vector3 Rotation;

        public Object(Game game)
            : base(game)
        {
        }
    }
}
