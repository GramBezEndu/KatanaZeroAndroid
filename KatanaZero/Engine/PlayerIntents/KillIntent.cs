using Engine.Input;
using Engine.Sprites;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.PlayerIntents
{
    public class KillIntent : Intent
    {
        private Enemy enemy;
        private GoToIntent goToIntent;
        private bool killAnimationInitialized;
        public KillIntent(InputManager inputManager, Camera c, Player p, Enemy e) : base(inputManager, c, p, e.Rectangle)
        {
            enemy = e;
            goToIntent = new GoToIntent(inputManager, c, p, e.Rectangle);
        }
        public override void IntentFinished()
        {
            //TODO: Fix after rework
            //if (!enemy.IsDead)
            //if (enemy.IsDead)
                Finished = true;
        }

        public override void UpdateIntent(GameTime gameTime)
        {
            IntentFinished();
            if(!Finished)
            {
                //We are not in position to kill yet
                if(!goToIntent.Finished)
                {
                    goToIntent.Update(gameTime);
                }
                //We are in position to kill
                else
                {
                    if(!killAnimationInitialized)
                    {
                        player.Kill(enemy);
                        killAnimationInitialized = true;
                    }
                }
            }
        }
    }
}
