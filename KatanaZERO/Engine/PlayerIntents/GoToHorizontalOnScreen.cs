namespace Engine.PlayerIntents
{
    using Engine.Input;
    using Microsoft.Xna.Framework;

    public class GoToHorizontalOnScreen : Intent
    {
        private readonly float destinationX;
        private readonly bool commingFromLeft;

        public GoToHorizontalOnScreen(InputManager im, Camera c, Player p, float onScreenX)
            : base(im, c, p)
        {
            destinationX = onScreenX;
            Rectangle r = new Rectangle((int)c.ScreenToWorld(new Vector2(onScreenX, 0f)).X, 0, 1, 1);
            bool shareX = player.CollisionRectangle.Left <= r.Right && player.CollisionRectangle.Right >= r.Left;
            if (shareX)
            {
                Finished = true;
            }

            if (camera.WorldToScreen(new Vector2(player.CollisionRectangle.Center.X, 0f)).X < onScreenX)
            {
                commingFromLeft = true;
            }
        }

        public override void IntentFinished()
        {
            if (commingFromLeft)
            {
                if (camera.WorldToScreen(new Vector2(player.CollisionRectangle.Center.X, 0f)).X >= destinationX)
                {
                    Finished = true;
                }
            }
            else
            {
                if (camera.WorldToScreen(new Vector2(player.CollisionRectangle.Center.X, 0f)).X <= destinationX)
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
