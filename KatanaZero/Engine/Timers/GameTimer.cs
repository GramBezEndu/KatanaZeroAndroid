using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Engine;
using Microsoft.Xna.Framework;

namespace PlatformerEngine.Timers
{
    public class GameTimer : IComponent
    {
        public bool Enabled { get; set; } = true;
        public double Interval { get; private set; }
        public double CurrentInterval { get; private set; }

        public EventHandler OnTimedEvent;

        public GameTimer(double actionInterval)
        {
            Interval = actionInterval;
            CurrentInterval = Interval;
        }
        public void Update(GameTime gameTime)
        {
            if(Enabled)
            {
                CurrentInterval -= gameTime.ElapsedGameTime.TotalSeconds;
                if (CurrentInterval <= 0)
                {
                    OnTimedEvent?.Invoke(this, new EventArgs());
                    CurrentInterval = Interval;
                }
            }
        }
    }
}
