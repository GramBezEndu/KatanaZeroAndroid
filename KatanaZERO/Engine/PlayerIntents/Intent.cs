namespace Engine.PlayerIntents
{
    using Engine.Input;
    using Microsoft.Xna.Framework;

    public abstract class Intent : IPlayerIntent
    {
        private InputManager inputManager;

        public Intent(InputManager im, Camera c, Player p)
        {
            inputManager = im;
            Camera = c;
            Player = p;
        }

        public bool Finished { get; protected set; }

        protected Camera Camera { get; private set; }

        protected Player Player { get; private set; }

        public void ResetIntent()
        {
            Finished = false;
        }

        public abstract void IntentFinished();

        public abstract void Update(GameTime gameTime);
    }
}
