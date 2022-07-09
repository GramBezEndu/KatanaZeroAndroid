namespace Engine.PlayerIntents
{
    using Engine.Input;
    using Microsoft.Xna.Framework;

    public class GoToHorizontal : Intent
    {
        private readonly bool commingFromLeft;

        private readonly float destinationX;

        public GoToHorizontal(InputManager im, Camera c, Player p, float destinationX)
            : base(im, c, p)
        {
            this.destinationX = destinationX;
            if (Player.CollisionRectangle.Center.X < destinationX)
            {
                commingFromLeft = true;
            }
        }

        public override void IntentFinished()
        {
            if (commingFromLeft)
            {
                if (Player.CollisionRectangle.Center.X >= destinationX)
                {
                    Finished = true;
                }
            }
            else
            {
                if (Player.CollisionRectangle.Center.X <= destinationX)
                {
                    Finished = true;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            IntentFinished();
            if (!Finished)
            {
                if (commingFromLeft)
                {
                    Player.MoveRight();
                }
                else
                {
                    Player.MoveLeft();
                }
            }
        }
    }
}
