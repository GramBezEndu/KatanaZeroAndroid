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
            //int rectangleSize = 18;
            //var newRectangle = new Rectangle(player.Rectangle.Center.X - rectangleSize/2, (int)player.Position.Y, rectangleSize, (int)player.Size.Y);
            if (/*newRectangle.Contains(middleOfRectangle)*/
                /*player.Rectangle.Left < middleOfRectangle.X && player.Rectangle.Right > middleOfRectangle.X*/
                /*player.Rectangle.Left > Rectangle.Left && player.Rectangle.Right < Rectangle.Right*/
                player.Rectangle.Contains(middleOfRectangle))
            {
                Finished = true;
                OnFinished?.Invoke(this, new EventArgs());
            }

            //if(player.Position.X >= Rectangle.X && player.Position.X <= Rectangle.X)
            //{
            //    Finished = true;
            //    OnFinished?.Invoke(this, new EventArgs());
            //}
            //if(playerAproachingFromLeft)
            //{
            //    if(player.Rectangle.Center.X > middleOfRectangle.X)
            //    {
            //        Finished = true;
            //        OnFinished?.Invoke(this, new EventArgs());
            //    }
            //}
            //else
            //{
            //    if (player.Rectangle.Center.X < middleOfRectangle.X)
            //    {
            //        Finished = true;
            //        OnFinished?.Invoke(this, new EventArgs());
            //    }
            //}

            ////More accurate, but won't work if intent rectangle is not wide enough
            //if(Rectangle.Width > player.Rectangle.Width)
            //{
            //    if (Rectangle.Contains(player.Rectangle))
            //    {
            //        Finished = true;
            //        OnFinished?.Invoke(this, new EventArgs());
            //    }
            //}
            //else
            //{
            //    if (player.Rectangle.Contains(middleOfRectangle))
            //    {
            //        Finished = true;
            //        OnFinished?.Invoke(this, new EventArgs());
            //    }
            //}
        }

        public override void UpdateIntent(GameTime gameTime)
        {
            IntentFinished();
            if (!Finished)
            {
                if (middleOfRectangle.X >= player.Rectangle.X)
                {
                    player.MoveRight();
                }
                else
                {
                    player.MoveLeft();
                }
            }
            //if (!Finished)
            //{
            //    if (player.Position.X < Rectangle.X)
            //    {
            //        player.MoveRight();
            //    }
            //    else
            //    {
            //        player.MoveLeft();
            //    }
            //}
        }
    }
}
