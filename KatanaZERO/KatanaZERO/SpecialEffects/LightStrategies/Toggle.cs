namespace KatanaZERO.LightStrategies
{
    using KatanaZERO.States;
    using Microsoft.Xna.Framework;
    using PlatformerEngine.Timers;

    public abstract class Toggle : LightStrategy
    {
        private GameTimer toggleLightsTimer;

        private double timeToggle = 0.5f;

        public Toggle(ClubLights cl)
            : base(cl)
        {
            CreateTimer();
        }

        public double TimeToggle
        {
            get => timeToggle;
            set
            {
                if (timeToggle != value)
                {
                    timeToggle = value;
                    CreateTimer();
                }
            }
        }

        public abstract void ToggleLights();

        public override void Update(GameTime gameTime)
        {
            toggleLightsTimer?.Update(gameTime);
        }

        private void CreateTimer()
        {
            toggleLightsTimer = new GameTimer(TimeToggle);
            toggleLightsTimer.OnTimedEvent += (o, e) => ToggleLights();
        }
    }
}