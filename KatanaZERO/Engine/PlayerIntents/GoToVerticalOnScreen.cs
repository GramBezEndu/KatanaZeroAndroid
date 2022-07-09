namespace Engine.PlayerIntents
{
    using Engine.Input;
    using Microsoft.Xna.Framework;

    public class GoToVerticalOnScreen : Intent
    {
        private readonly float destinationY;
        private readonly bool commingFromTop;

        public GoToVerticalOnScreen(InputManager im, Camera c, Player p, float onScreenY)
            : base(im, c, p)
        {
            destinationY = onScreenY;
            Rectangle r = new Rectangle(0, (int)c.ScreenToWorld(new Vector2(0f, onScreenY)).Y, 1, 1);
            bool shareY = Player.CollisionRectangle.Top < r.Bottom && Player.CollisionRectangle.Bottom > r.Top;
            if (shareY)
            {
                Finished = true;
            }

            if (Camera.WorldToScreen(new Vector2(0f, Player.CollisionRectangle.Center.Y)).Y < onScreenY)
            {
                commingFromTop = true;
            }
        }

        public override void IntentFinished()
        {
            if (commingFromTop)
            {
                if (Camera.WorldToScreen(new Vector2(0f, Player.CollisionRectangle.Center.Y)).Y >= destinationY)
                {
                    Finished = true;
                }
            }
            else
            {
                if (Camera.WorldToScreen(new Vector2(0f, Player.CollisionRectangle.Center.Y)).Y <= destinationY)
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
                if (commingFromTop)
                {
                    Player.MoveDown();
                }
                else
                {
                    Player.MoveUp();
                }
            }
        }
    }
}
