using System;
using System.Collections.Generic;
using System.Text;
using Engine.Input;
using Microsoft.Xna.Framework;

namespace Engine.PlayerIntents
{
    public class GoToIntent : Intent
    {
        bool commingFromLeft;
        Vector2 destination;
        public GoToIntent(InputManager im, Camera c, Player p, Vector2 destination) : base(im, c, p)
        {
            this.destination = destination;
            if (player.CollisionRectangle.Center.X < destination.X)
                commingFromLeft = true;
        }
        public override void IntentFinished()
        {
            if (commingFromLeft)
            {
                if (player.CollisionRectangle.Center.X >= destination.X)
                    Finished = true;
            }
            else
            {
                if (player.CollisionRectangle.Center.X <= destination.X)
                    Finished = true;
            }
            if (Finished)
            {
                OnFinished?.Invoke(this, new EventArgs());
            }
        }

        public override void Update(GameTime gameTime)
        {
            IntentFinished();
            if (!Finished)
            {
                if (commingFromLeft)
                {
                    player.MoveRight();
                }
                else
                {
                    player.MoveLeft();
                }
            }
        }
    }
}
