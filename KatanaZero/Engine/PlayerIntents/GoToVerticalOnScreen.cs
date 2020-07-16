using System;
using System.Collections.Generic;
using System.Text;
using Engine.Input;
using Microsoft.Xna.Framework;

namespace Engine.PlayerIntents
{
    public class GoToVerticalOnScreen : Intent
    {
        float destinationY;
        bool commingFromTop;

        public GoToVerticalOnScreen(InputManager im, Camera c, Player p, float onScreenY) : base(im, c, p)
        {
            this.destinationY = onScreenY;
            if (camera.WorldToScreen(new Vector2(0f, player.CollisionRectangle.Center.Y)).Y < onScreenY)
                commingFromTop = true;
        }

        public override void IntentFinished()
        {
            if (commingFromTop)
            {
                if (camera.WorldToScreen(new Vector2(0f, player.CollisionRectangle.Center.Y)).Y >= destinationY)
                    Finished = true;
            }
            else
            {
                if (camera.WorldToScreen(new Vector2(0f, player.CollisionRectangle.Center.Y)).Y <= destinationY)
                    Finished = true;
            }
        }

        public override void Update(GameTime gameTime)
        {
            IntentFinished();
            if (!Finished)
            {
                if (commingFromTop)
                {
                    player.MoveDown();
                }
                else
                {
                    player.MoveUp();
                }
            }
        }
    }
}
