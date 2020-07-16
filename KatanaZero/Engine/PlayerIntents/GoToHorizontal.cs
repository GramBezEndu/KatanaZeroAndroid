using System;
using System.Collections.Generic;
using System.Text;
using Engine.Input;
using Microsoft.Xna.Framework;

namespace Engine.PlayerIntents
{
    public class GoToHorizontal : Intent
    {
        bool commingFromLeft;
        float destinationX;
        public GoToHorizontal(InputManager im, Camera c, Player p, float destinationX) : base(im, c, p)
        {
            this.destinationX = destinationX;
            if (player.CollisionRectangle.Center.X < destinationX)
                commingFromLeft = true;
        }
        public override void IntentFinished()
        {
            if (commingFromLeft)
            {
                if (player.CollisionRectangle.Center.X >= destinationX)
                    Finished = true;
            }
            else
            {
                if (player.CollisionRectangle.Center.X <= destinationX)
                    Finished = true;
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
