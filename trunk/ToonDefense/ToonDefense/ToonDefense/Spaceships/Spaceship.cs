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
        public List<Vector3> Destinations;
        public float Speed;
        public int Health;
        public int Reward;

        public Spaceship(Game game, Camera camera)
            :base(game, camera)
        {

        }
    }
}
