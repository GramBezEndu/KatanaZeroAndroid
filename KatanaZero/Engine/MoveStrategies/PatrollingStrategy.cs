using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Engine.Sprites;
using Microsoft.Xna.Framework;
using PlatformerEngine.Timers;

namespace Engine.MoveStrategies
{
    public class PatrollingStrategy : Strategy
    {
        float positionStartPatrolX;
        float positionEndPatrolX;
        GameTimer idleTimer;
        float idleTime;
        /// <summary>
        /// Going left on first move
        /// </summary>
        bool goingLeft = true;
        public PatrollingStrategy(Enemy e, float startX, float endX, float idleTimeSeconds = 3.5f, bool startingLeft = true) : base(e)
        {
            positionStartPatrolX = startX;
            positionEndPatrolX = endX;
            idleTime = idleTimeSeconds;
            GoingLeft = startingLeft;
        }

        public bool GoingLeft
        {
            get => goingLeft;
            private set
            {
                //On value change
                if(goingLeft != value)
                {
                    CreateIdleTimer();
                    goingLeft = value;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            //Idle
            if(idleTimer != null)
            {
                idleTimer.Update(gameTime);
                return;
            }
            //Move
            if(GoingLeft)
            {
                if (enemy.Position.X < positionStartPatrolX)
                {
                    //End going left -> start going right
                    GoingLeft = false;
                }
                else
                {
                    //Go left still
                    enemy.Velocity = new Vector2(-0.8f, enemy.Velocity.Y);
                }
            }
            else
            {
                if (enemy.Position.X > positionEndPatrolX)
                {
                    GoingLeft = true;
                }
                else
                {
                    enemy.Velocity = new Vector2(0.8f, enemy.Velocity.Y);
                }
            }
        }
        private void CreateIdleTimer()
        {
            idleTimer = new GameTimer(idleTime)
            {
                OnTimedEvent = (o, e) => idleTimer = null
            };
        }
    }
}
