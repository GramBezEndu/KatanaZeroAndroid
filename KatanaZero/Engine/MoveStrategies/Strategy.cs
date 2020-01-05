using System;
using System.Collections.Generic;
using System.Text;
using Engine.Sprites;
using Microsoft.Xna.Framework;

namespace Engine.MoveStrategies
{
    public abstract class Strategy : IComponent
    {
        protected Enemy enemy;
        public Strategy(Enemy e)
        {
            enemy = e;
        }

        public abstract void Update(GameTime gameTime);
    }
}
