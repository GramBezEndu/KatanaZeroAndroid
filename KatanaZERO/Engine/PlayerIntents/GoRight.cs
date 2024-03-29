﻿namespace Engine.PlayerIntents
{
    using Engine.Input;
    using Microsoft.Xna.Framework;

    public class GoRight : Intent
    {
        public GoRight(InputManager im, Camera c, Player p)
            : base(im, c, p)
        {
        }

        public override void IntentFinished()
        {
        }

        public override void Update(GameTime gameTime)
        {
            Player.MoveRight();
        }
    }
}
