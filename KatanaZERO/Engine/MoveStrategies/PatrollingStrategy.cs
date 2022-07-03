namespace Engine.MoveStrategies
{
    using Engine.Sprites;
    using Microsoft.Xna.Framework;
    using PlatformerEngine.Timers;

    public class PatrollingStrategy : Strategy
    {
        private readonly float positionStartPatrolX;
        private readonly float positionEndPatrolX;
        private GameTimer idleTimer;
        private readonly float idleTime;

        /// <summary>
        /// Determines in which direction (left/right) will go now
        /// Note: If not specified enemy will go left on first move
        /// </summary>
        private bool goingLeft = true;

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
                // On value change
                if (goingLeft != value)
                {
                    CreateIdleTimer();
                    goingLeft = value;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            // Idle
            if (idleTimer != null)
            {
                idleTimer.Update(gameTime);
                return;
            }

            // Move
            if (GoingLeft)
            {
                if (enemy.Position.X < positionStartPatrolX)
                {
                    // End going left -> start going right
                    GoingLeft = false;
                }
                else
                {
                    // Go left still
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
                OnTimedEvent = (o, e) => idleTimer = null,
            };
        }
    }
}
