namespace KatanaZERO.States
{
    using System;
    using System.Collections.Generic;
    using Engine;
    using Engine.SpecialEffects;
    using KatanaZERO.LightStrategies;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using PlatformerEngine.Timers;

    public class ClubLights : IDrawableComponent
    {
        private readonly GameTimer changeStrategyTimer;

        private LightStrategy currentStrategy;

        private GameTimer speedUpTimer;

        private int changeStrategiesCount;

        public ClubLights()
        {
            currentStrategy = new ToggleEvenLights(this);
            changeStrategyTimer = new GameTimer(15f);
            changeStrategyTimer.OnTimedEvent += (o, e) => ChangeStrategy();
            speedUpTimer = new GameTimer(11f);
            speedUpTimer.OnTimedEvent += (o, e) =>
            {
                SpeedUp();

                // It's just a one time effect, we don't need this timer anymore
                speedUpTimer = null;
            };

            // starting position
            Position = new Vector2(180, 290);
            for (int i = 0; i < 55; i++)
            {
                Lights.Add(new DrawableRectangle(new Rectangle((int)((20 * i) + Position.X), (int)Position.Y, 3, 157))
                {
                    Color = Color.LightGreen * 0.3f,
                    Filled = true,
                });
            }
        }

        public bool Hidden { get; set; }

        public Vector2 Position { get; set; }

        public Vector2 Size => throw new NotImplementedException();

        public Rectangle Rectangle => throw new NotImplementedException();

        public Color Color { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public List<DrawableRectangle> Lights { get; private set; } = new List<DrawableRectangle>();

        public List<SpecialEffect> SpecialEffects { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!Hidden)
            {
                foreach (DrawableRectangle l in Lights)
                {
                    l.Draw(gameTime, spriteBatch);
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            if (!Hidden)
            {
                speedUpTimer?.Update(gameTime);
                changeStrategyTimer.Update(gameTime);
                currentStrategy.Update(gameTime);
                foreach (DrawableRectangle l in Lights)
                {
                    l.Update(gameTime);
                }
            }
        }

        public void AddSpecialEffect(SpecialEffect effect)
        {
            throw new NotImplementedException();
        }

        private void SpeedUp()
        {
            if (currentStrategy is Toggle toggle)
            {
                toggle.TimeToggle = 0.1f;
            }
        }

        private void ChangeStrategy()
        {
            changeStrategiesCount++;
            ChangeLightsColor();
            if (currentStrategy is ToggleAll)
            {
                currentStrategy = new ToggleEvenLights(this);
            }
            else
            {
                currentStrategy = new ToggleAll(this);
            }
        }

        private void ChangeColor(Color c)
        {
            foreach (DrawableRectangle l in Lights)
            {
                l.Color = c;
            }
        }
    }
}