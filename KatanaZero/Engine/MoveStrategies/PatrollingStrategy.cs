using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Engine.Sprites;
using Microsoft.Xna.Framework;

namespace Engine.MoveStrategies
{
    public class PatrollingStrategy : Strategy
    {
        float positionStartPatrolX;
        float positionEndPatrolX;
        /// <summary>
        /// Going left on first move
        /// </summary>
        bool goingLeft = true;
        public PatrollingStrategy(Enemy e, float startX, float endX) : base(e)
        {
            positionStartPatrolX = startX;
            positionEndPatrolX = endX;
        }
        public override void Update(GameTime gameTime)
        {
            if(goingLeft)
            {
                if (enemy.Position.X < positionStartPatrolX)
                {
                    //End going left -> start going right
                    goingLeft = false;
                }
                else
                {
                    //Go left still
                    enemy.Velocity = new Vector2(-0.8f, enemy.Velocity.Y);
                    Debug.WriteLine("enemy velocity" + enemy.Velocity);
                }
            }
            else
            {
                if (enemy.Position.X > positionEndPatrolX)
                {
                    goingLeft = true;
                }
                else
                {
                    enemy.Velocity = new Vector2(0.8f, enemy.Velocity.Y);
                    Debug.WriteLine("enemy velocity" + enemy.Velocity);
                }
            }

            if (enemy.Velocity.Y < 0)
            {
                int x = 5;
            }


            //if(enemy.Position.X > positionStartPatrolX)
            //{
            //    //Left
            //    enemy.Velocity = new Vector2(-2f, enemy.Velocity.Y);
            //}
            //else
            //{
            //    enemy.Velocity = new Vector2(2f, enemy.Velocity.Y);
            //}
        }
    }
}
