using Engine.Sprites;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.PlayerIntents
{
    public class KillIntent : IComponent, IPlayerIntent
    {
        private Player player;
        private Enemy enemy;
        private GoToIntent goToIntent;
        private bool killAnimationInitialized;
        public KillIntent(Player p, Enemy e)
        {
            player = p;
            enemy = e;
            goToIntent = new GoToIntent(p, e.Rectangle);
        }
        public bool IntentFinished()
        {
            //if (!enemy.IsDead)
            if(!enemy.IsDead)
                return false;
            return true;
        }

        public void Update(GameTime gameTime)
        {
            if(!IntentFinished())
            {
                //We are not in position to kill yet
                if(!goToIntent.IntentFinished())
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
