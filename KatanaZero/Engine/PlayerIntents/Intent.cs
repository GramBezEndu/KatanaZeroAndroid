using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Engine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Engine.PlayerIntents
{
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
