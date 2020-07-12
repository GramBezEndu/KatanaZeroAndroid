using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using PlatformerEngine.Timers;

namespace Engine.SpecialEffects
{
    public class JumpingEffect : SpecialEffect
    {
        private GameTimer effectTimer;
        List<Vector2> startingPositions = new List<Vector2>();
        bool goingDown = true;
        public JumpingEffect()
        {
            effectTimer = new GameTimer(0.25f)
            {
                OnTimedEvent = SwitchDirection
            };
        }

        private void SwitchDirection(object sender, EventArgs e)
        {
            goingDown = !goingDown;
            //If we changed to going down
            if (goingDown)
            {
                //Reset position to original position (will avoid position changes in long term)
                for(int i = 0; i < targets.Count; i++)
                {
                    targets[i].Position = startingPositions[i];
                }
            }
        }

        public override void AddTarget(IDrawableComponent target)
        {
            base.AddTarget(target);
            startingPositions.Add(target.Position);
        }

        public override void Update(GameTime gameTime)
        {
            effectTimer.Update(gameTime);
            float movementPerFrame = 0.2f;
            if (goingDown)
            {
                foreach(var target in targets)
                {
                    target.Position = new Vector2(target.Position.X, target.Position.Y + movementPerFrame);
                }
            }
            else
            {
                foreach (var target in targets)
                {
                    target.Position = new Vector2(target.Position.X, target.Position.Y - movementPerFrame);
                }
            }
        }
    }
}
