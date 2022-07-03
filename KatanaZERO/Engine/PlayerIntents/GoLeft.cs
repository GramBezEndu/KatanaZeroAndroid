namespace Engine.PlayerIntents
{
    using Engine.Input;
    using Microsoft.Xna.Framework;

    public class GoLeft : Intent
    {
        public GoLeft(InputManager im, Camera c, Player p)
            : base(im, c, p)
        {
        }

        public override void IntentFinished()
        {
        }

        public override void Update(GameTime gameTime)
        {
            player.MoveLeft();
        }
    }
}
