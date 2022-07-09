namespace Engine.MoveStrategies
{
    using Engine.Sprites;
    using Microsoft.Xna.Framework;
    using PlatformerEngine.Timers;

    public class PatrollingStrategy : Strategy
    {
        private readonly float positionStartPatrolX;

        private readonly float positionEndPatrolX;

        private readonly float idleTime;

        private GameTimer idleTimer;

        /// <summary>
        /// Determines in which direction (left/right) will go now
        /// Note: If not specified enemy will go left on first move
        /// </summary>
        private bool goingLeft = true;

        public PatrollingStrategy(Enemy enemy, float startX, float endX, float idleTimeSeconds = 3.5f, bool startingLeft = true)
            : base(enemy)
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
                if (Enemy.Position.X < positionStartPatrolX)
                {
                    // End going left -> start going right
                    GoingLeft = false;
                }
                else
                {
                    // Go left still
                    Enemy.Velocity = new Vector2(-0.8f, Enemy.Velocity.Y);
                }
            }
            else
            {
                if (Enemy.Position.X > positionEndPatrolX)
                {
                    GoingLeft = true;
                }
                else
                {
                    Enemy.Velocity = new Vector2(0.8f, Enemy.Velocity.Y);
                }
            }
        }

        private void CreateIdleTimer()
        {
            idleTimer = new GameTimer(idleTime);
            idleTimer.OnTimedEvent += (o, e) => idleTimer = null;
        }
    }
}
