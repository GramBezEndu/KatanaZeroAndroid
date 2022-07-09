namespace Engine.SpecialEffects
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using PlatformerEngine.Timers;

    public class JumpingEffect : SpecialEffect
    {
        private readonly GameTimer effectTimer;

        private readonly List<Vector2> startingPositions = new List<Vector2>();

        private bool goingDown = true;

        public JumpingEffect()
        {
            effectTimer = new GameTimer(0.25f);
            effectTimer.OnTimedEvent += SwitchDirection;
        }

        public override void AddTarget(IDrawableComponent target)
        {
            base.AddTarget(target);
            startingPositions.Add(target.Position);
        }

        public override void Update(GameTime gameTime)
        {
            if (Enabled)
            {
                effectTimer.Update(gameTime);
                float movementPerFrame = 0.2f;
                if (goingDown)
                {
                    foreach (IDrawableComponent target in Targets)
                    {
                        target.Position = new Vector2(target.Position.X, target.Position.Y + movementPerFrame);
                    }
                }
                else
                {
                    foreach (IDrawableComponent target in Targets)
                    {
                        target.Position = new Vector2(target.Position.X, target.Position.Y - movementPerFrame);
                    }
                }
            }
        }

        private void SwitchDirection(object sender, EventArgs e)
        {
            if (Enabled)
            {
                goingDown = !goingDown;

                // If we changed to going down
                if (goingDown)
                {
                    // Reset position to original position (will avoid position changes in long term)
                    for (int i = 0; i < Targets.Count; i++)
                    {
                        Targets[i].Position = startingPositions[i];
                    }
                }
            }
        }
    }
}
