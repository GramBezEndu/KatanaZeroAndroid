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
        public Toggle(ClubLights cl) : base(cl)
        {
            toggleLightsTimer = new GameTimer(0.5f)
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