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

        private readonly bool distractionOnLeftSide;

        private Vector2 distractionPosition;

        public Distracted(Enemy enemy, Strategy previousStrategy, Vector2 distractionPosition)
            : base(enemy)
        {
            this.previousStrategy = previousStrategy;
            this.distractionPosition = distractionPosition;
            if (distractionPosition.X < enemy.Position.X)
            {
                distractionOnLeftSide = true;
            }

            strategyTimer = new GameTimer(10f);
            strategyTimer.OnTimedEvent += RestorePreviousStrategy;
            enemy.QuestionMark.Hidden = false;
        }

        public override void Update(GameTime gameTime)
        {
            strategyTimer.Update(gameTime);

            // If enemy is not nearby where the sound was made
            if (!Enemy.CollisionRectangle.Contains(distractionPosition))
            {
                if (distractionOnLeftSide)
                {
                    Enemy.Velocity = new Vector2(-0.8f, Enemy.Velocity.Y);
                }
                else
                {
                    Enemy.Velocity = new Vector2(0.8f, Enemy.Velocity.Y);
                }
            }
        }

        private void RestorePreviousStrategy(object sender, EventArgs e)
        {
            Enemy.QuestionMark.Hidden = true;
            Enemy.CurrentStrategy = previousStrategy;
        }
    }
}
