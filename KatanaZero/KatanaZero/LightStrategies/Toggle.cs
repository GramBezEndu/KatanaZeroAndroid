using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using KatanaZero.States;
using Microsoft.Xna.Framework;
using PlatformerEngine.Timers;

namespace KatanaZero.LightStrategies
{
    public abstract class Toggle : LightStrategy
    {
        protected GameTimer toggleLightsTimer;
        private double _timeToggle = 0.5f;

        public double TimeToggle
        {
            get => _timeToggle;
            set
            {
                if(_timeToggle != value)
                {
                    _timeToggle = value;
                    CreateTimer();
                }
            }
        }
        public Toggle(ClubLights cl) : base(cl)
        {
            CreateTimer();
        }

        private void CreateTimer()
        {
            toggleLightsTimer = new GameTimer(TimeToggle)
            {
                OnTimedEvent = (o, e) =>
                {
                    ToggleLights();
                }
            };
        }

        public abstract void ToggleLights();

        public override void Update(GameTime gameTime)
        {
            toggleLightsTimer?.Update(gameTime);
        }
    }
}