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
using Engine;
using Engine.SpecialEffects;
using KatanaZero.LightStrategies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlatformerEngine.Timers;

namespace KatanaZero.States
{
    public class ClubLights : IDrawableComponent
    {
        public bool Hidden { get; set; }
        public Vector2 Position { get; set; }

        public Vector2 Size => throw new NotImplementedException();

        public Rectangle Rectangle => throw new NotImplementedException();

        public Color Color { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public List<DrawableRectangle> Lights { get; private set; } = new List<DrawableRectangle>();
        public List<SpecialEffect> SpecialEffects { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        LightStrategy currentStrategy;

        GameTimer speedUpTimer;
        GameTimer changeStrategyTimer;
        int changeStrategiesCount;

        public ClubLights()
        {
            currentStrategy = new ToggleEvenLights(this);
            changeStrategyTimer = new GameTimer(15f)
            {
                OnTimedEvent = (o, e) => ChangeStrategy()
            };
            speedUpTimer = new GameTimer(11f)
            {
                OnTimedEvent = (o, e) =>
                {
                    SpeedUp();
                    //It's just a one time effect, we don't need this timer anymore
                    speedUpTimer = null;
                }
            };
            //starting position
            Position = new Vector2(180, 290);
            for(int i=0;i<55;i++)
            {
                Lights.Add(new DrawableRectangle(new Rectangle((int)(20 * i + Position.X), (int)Position.Y, 3, 157))
                {
                    Color = Color.LightGreen * 0.3f,
                    Filled = true,
                });
            }
        }

        private void SpeedUp()
        {
            if (currentStrategy is Toggle toggle)
                toggle.TimeToggle = 0.1f;
        }

        private void ChangeStrategy()
        {
            changeStrategiesCount++;
            ChangeLightsColor();
            if (currentStrategy is ToggleAll)
                currentStrategy = new ToggleEvenLights(this);
            else
                currentStrategy = new ToggleAll(this);
        }

        public void ChangeLightsColor()
        {
            List<Color> colors = new List<Color>
            {
                Color.LightGreen * 0.3f,
                Color.MediumVioletRed * 0.4f,
                Color.Yellow * 0.3f,
                Color.Violet * 0.5f,
            };
            ChangeColor(colors[changeStrategiesCount % colors.Count]);
        }

        private void ChangeColor(Color c)
        {
            foreach (var l in Lights)
                l.Color = c;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if(!Hidden)
            {
                foreach (var l in Lights)
                    l.Draw(gameTime, spriteBatch);
            }
        }


        public void Update(GameTime gameTime)
        {
            if(!Hidden)
            {
                speedUpTimer?.Update(gameTime);
                changeStrategyTimer.Update(gameTime);
                currentStrategy.Update(gameTime);
                foreach (var l in Lights)
                    l.Update(gameTime);
            }
        }

        public void AddSpecialEffect(SpecialEffect effect)
        {
            throw new NotImplementedException();
        }
    }
}