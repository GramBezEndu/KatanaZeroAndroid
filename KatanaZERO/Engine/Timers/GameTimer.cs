﻿namespace PlatformerEngine.Timers
{
    using System;
    using Engine;
    using Microsoft.Xna.Framework;

    public class GameTimer : IComponent
    {
        public GameTimer(double actionInterval)
        {
            Interval = actionInterval;
            CurrentInterval = Interval;
        }

        public event EventHandler OnTimedEvent;

        public bool Enabled { get; set; } = true;

        public double Interval { get; set; }

        public double CurrentInterval { get; set; }

        public void Update(GameTime gameTime)
        {
            if (Enabled)
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
