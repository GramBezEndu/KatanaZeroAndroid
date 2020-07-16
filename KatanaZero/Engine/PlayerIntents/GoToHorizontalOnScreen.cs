using System;
using System.Collections.Generic;
using System.Text;
using Engine.Input;
using Microsoft.Xna.Framework;

namespace Engine.PlayerIntents
{
    public class GoToHorizontalOnScreen : Intent
    {
        float destinationX;
        bool commingFromLeft;

        public GoToHorizontalOnScreen(InputManager im, Camera c, Player p, float onScreenX) : base(im, c, p)
        {
            this.destinationX = onScreenX;
            if (camera.WorldToScreen(new Vector2(player.CollisionRectangle.Center.X, 0f)).X < onScreenX)
                commingFromLeft = true;
        }

        public override void IntentFinished()
        {
            if (commingFromLeft)
            {
                if (camera.WorldToScreen(new Vector2(player.CollisionRectangle.Center.X, 0f)).X >= destinationX)
                    Finished = true;
            }
            else
            {
                if (camera.WorldToScreen(new Vector2(player.CollisionRectangle.Center.X, 0f)).X <= destinationX)
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
