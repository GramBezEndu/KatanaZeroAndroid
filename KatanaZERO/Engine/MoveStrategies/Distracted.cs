namespace Engine.MoveStrategies
{
    using System;
    using Engine.Sprites;
    using Microsoft.Xna.Framework;
    using PlatformerEngine.Timers;

    public class Distracted : Strategy
    {
        private readonly GameTimer strategyTimer;
        private readonly Strategy previousStrategy;
        private Vector2 distractionPosition;
        private readonly bool distractionOnLeftSide;

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

            // If enemy is not nearby where the sound was made
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
