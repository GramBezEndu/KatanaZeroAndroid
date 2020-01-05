using System;
using System.Collections.Generic;
using System.Text;
using Engine.Input;
using Microsoft.Xna.Framework;

namespace Engine.PlayerIntents
{
    public class TeleportIntent : Intent
    {
        GoToIntent goToIntent;
        Vector2 positionToTeleport;
        public TeleportIntent(InputManager im, Camera c, Player p, Rectangle r, Vector2 positionToTp) : base(im, c, p, r)
        {
            goToIntent = new GoToIntent(im, c, p, Rectangle);
            positionToTeleport = positionToTp;
        }

        public override void IntentFinished()
        {
            if (player.Position == positionToTeleport)
            {
                Finished = true;
                OnFinished?.Invoke(this, new EventArgs());
            }
        }

        public override void UpdateIntent(GameTime gameTime)
        {
            if (!Finished)
            {
                if (!goToIntent.Finished)
                {
                    goToIntent.UpdateIntent(gameTime);
                }
                else
                {
                    player.Position = positionToTeleport;
                    IntentFinished();
                }
            }
        }
    }
}
