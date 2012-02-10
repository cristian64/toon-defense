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

namespace ToonDefense.Towers
{
    public class Tower : Object
    {
        public Spaceship Target;
        public float Sight;
        public int Damage;
        public int Delay;
        public int Price;

        public Tower(Game game, Camera camera)
            :base(game, camera)
        {
            Target = null;
            Sight = 2;
            Damage = 10;
            Delay = 1000;
            Price = 100;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
