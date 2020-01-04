using System;
using System.Collections.Generic;
using System.Text;
using Engine.Input;
using Microsoft.Xna.Framework;

namespace Engine.PlayerIntents
{
    public class GoToIntent : Intent
    {
        //We are using middle of the given rectangle to check if player touches this point
        private Point middleOfRectangle;
        public GoToIntent(InputManager im, Camera c, Player p, Rectangle rec) : base(im, c, p, rec)
        {
            middleOfRectangle = new Point(rec.X + rec.Width/2,
                rec.Y + rec.Height/2);
        }
        public override void IntentFinished()
        {
            //if(player.Rectangle.Intersects(objectRectangle))
            if (player.Rectangle.Contains(middleOfRectangle))
                Finished = true;
        }

        public override void UpdateIntent(GameTime gameTime)
        {
            IntentFinished();
            if(!Finished)
            {
                if (middleOfRectangle.X >= player.Rectangle.X)
                    player.MoveRight();
                else
                    player.MoveLeft();
            }
        }
    }
}
