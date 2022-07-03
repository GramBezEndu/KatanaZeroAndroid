namespace Engine.PlayerIntents
{
    using System;
    using Engine.Input;
    using Microsoft.Xna.Framework;

    public class GoToOnScreenIntent : Intent
    {
        private readonly GoToHorizontalOnScreen horizontalIntent;

        private readonly GoToVerticalOnScreen verticalIntent;

        public EventHandler OnFinished { get; set; }

        public GoToOnScreenIntent(InputManager im, Camera c, Player p, Vector2 destination)
            : base(im, c, p)
        {
            horizontalIntent = new GoToHorizontalOnScreen(im, c, p, destination.X);
            verticalIntent = new GoToVerticalOnScreen(im, c, p, destination.Y);
        }

        public override void IntentFinished()
        {
            bool first = false;
            if (horizontalIntent == null)
            {
                first = true;
            }
            else
            {
                first = horizontalIntent.Finished;
            }

            bool second = false;
            if (verticalIntent == null)
            {
                second = true;
            }
            else
            {
                second = verticalIntent.Finished;
            }

            if (first && second)
            {
                Finished = true;
                OnFinished?.Invoke(this, new EventArgs());
            }
        }

        public override void Update(GameTime gameTime)
        {
            IntentFinished();
            if (!Finished)
            {
                horizontalIntent?.Update(gameTime);
                verticalIntent?.Update(gameTime);
            }
        }
    }
}
