using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine.Physics
{
    public class TopDownCollisionManager : CollisionManager
    {
        public TopDownCollisionManager()
        {
        }

        //protected override void CheckVertical(GameTime gameTime, ICollidable c, Rectangle s)
        //{
        //    if (ShareXCoordinate(c.CollisionRectangle, s))
        //    {
        //        if (c.Velocity.Y > 0)
        //        {
        //            float distanceY = GetDistanceBeneath(c.CollisionRectangle, s);
        //            if (distanceY >= 0 && distanceY < c.Velocity.Y)
        //            {
        //                c.Velocity = new Vector2(c.Velocity.X, distanceY);
        //                c.OnMapCollision?.Invoke(this, new EventArgs());
        //            }
        //        }
        //        else if (c.Velocity.Y < 0)
        //        {
        //            float distanceY = GetDistanceAbove(c.CollisionRectangle, s);
        //            if (distanceY >= 0 && distanceY < -c.Velocity.Y)
        //            {
        //                c.Velocity = new Vector2(c.Velocity.X, -distanceY);
        //                c.OnMapCollision?.Invoke(this, new EventArgs());
        //            }
        //        }
        //    }
        //}
    }
}
