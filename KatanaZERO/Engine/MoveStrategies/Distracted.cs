using System;
using System.Collections.Generic;
using System.Text;
using Engine.Sprites;
using Microsoft.Xna.Framework;
using PlatformerEngine.Timers;

namespace Engine.MoveStrategies
{
    public class Distracted : Strategy
    {
        private GameTimer strategyTimer;
        private Strategy previousStrategy;
        private Vector2 distractionPosition;
        private bool distractionOnLeftSide;

        public Distracted(Enemy e, Strategy previousStrategy, Vector2 distractionPosition) : base(e)
        {
            this.previousStrategy = previousStrategy;
            this.distractionPosition = distractionPosition;
            if (distractionPosition.X < e.Position.X)
            {
                distractionOnLeftSide = true;
            }
            strategyTimer = new GameTimer(10f)
            {
                OnTimedEvent = RestorePreviousStrategy,
            };
            e.QuestionMark.Hidden = false;
        }

        private void RestorePreviousStrategy(object sender, EventArgs e)
        {
            enemy.QuestionMark.Hidden = true;
            enemy.CurrentStrategy = previousStrategy;
        }

        public override void Update(GameTime gameTime)
        {
            strategyTimer.Update(gameTime);
            //If enemy is not nearby where the sound was made
            if (!enemy.CollisionRectangle.Contains(distractionPosition))
            {
                if (distractionOnLeftSide)
                {
                    enemy.Velocity = new Vector2(-0.8f, enemy.Velocity.Y);
                }
                else
                {
                    enemy.Velocity = new Vector2(0.8f, enemy.Velocity.Y);
                }
            }
        }
    }
}
