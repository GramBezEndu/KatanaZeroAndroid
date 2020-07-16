using System;
using System.Collections.Generic;
using System.Text;
using Engine.Input;
using Microsoft.Xna.Framework;

namespace Engine.PlayerIntents
{
    public class GoToOnScreenIntent : Intent
    {
        GoToHorizontalOnScreen horizontalIntent;
        GoToVerticalOnScreen verticalIntent;

        public GoToOnScreenIntent(InputManager im, Camera c, Player p, Vector2 destination) : base(im, c, p)
        {
            horizontalIntent = new GoToHorizontalOnScreen(im, c, p, destination.X);
            verticalIntent = new GoToVerticalOnScreen(im, c, p, destination.Y);
        }

        public override void IntentFinished()
        {
            if (horizontalIntent.Finished && verticalIntent.Finished)
                Finished = true;
        }

        public override void Update(GameTime gameTime)
        {
            if(!Finished)
            {
                horizontalIntent.Update(gameTime);
                verticalIntent.Update(gameTime);
            }
        }
    }
}
