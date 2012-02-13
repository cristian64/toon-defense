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
using ToonDefense.Spaceships;
using ToonDefense.ParticleSystem;

namespace ToonDefense.Projectiles
{
    public class Projectile : Object
    {
        public int Damage;
        public Spaceship Target;
        public Vector3 Speed;
        public float Acceleration;
        public float Friction;
        public bool NoTarget;

        public Projectile(Game game, Camera camera)
            :base(game, camera)
        {
        }
    }
}
