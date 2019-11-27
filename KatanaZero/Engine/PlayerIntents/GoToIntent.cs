using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine.PlayerIntents
{
    public class GoToIntent : IPlayerIntent
    {
        private Player player;
        private Rectangle objectRectangle;
        //We are using middle of the given rectangle to check if player touches this point (this way player touch "more" of the given rectangle)
        private Point middleOfRectangle;
        public GoToIntent(Player p, Rectangle rec)
        {
            player = p;
            objectRectangle = rec;
            middleOfRectangle = new Point(objectRectangle.X + objectRectangle.Width/2,
                objectRectangle.Y + objectRectangle.Height/2);
        }
        public bool IntentFinished()
        {
            //if(player.Rectangle.Intersects(objectRectangle))
            if (player.Rectangle.Contains(middleOfRectangle))
                return true;
            return false;
        }

        public void Update(GameTime gameTime)
        {
            if(!IntentFinished())
            {
                if (objectRectangle.X >= player.Rectangle.X)
                    player.MoveRight();
                else
                    player.MoveLeft();
            }
        }
    }
}
