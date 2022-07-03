namespace Engine.PlayerIntents
{
    using Engine.Input;
    using Microsoft.Xna.Framework;

    public abstract class Intent : IPlayerIntent
    {
        protected Player player;

        public bool Finished { get; protected set; }

        protected InputManager inputManager;

        protected Camera camera;

        public Intent(InputManager im, Camera c, Player p)
        {
            inputManager = im;
            camera = c;
            player = p;
        }

        public void ResetIntent()
        {
            Finished = false;
        }

        public abstract void IntentFinished();

        public abstract void Update(GameTime gameTime);
        //public EventHandler OnFinished { get; set; }
    }
}
