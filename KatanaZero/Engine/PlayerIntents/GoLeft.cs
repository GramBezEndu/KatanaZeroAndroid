using System;
using System.Collections.Generic;
using System.Text;
using Engine.Input;
using Microsoft.Xna.Framework;

namespace Engine.PlayerIntents
{
    public class GoLeft : Intent
    {
        public GoLeft(InputManager im, Camera c, Player p) : base(im, c, p)
        {
        }

        public override void IntentFinished()
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            player.StepLeft();
        }
    }
}
