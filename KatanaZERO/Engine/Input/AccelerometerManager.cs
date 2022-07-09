namespace Engine.Input
{
    using System;
    using Microsoft.Devices.Sensors;
    using Microsoft.Xna.Framework;
    using PlatformerEngine.Timers;

    public class AccelerometerManager : IComponent
    {
        private readonly Accelerometer accelerometer;

        private readonly GameTimer accelerometerValueCheckInterval;

        private readonly float shakeMinimalForce = 0.9f;

        private Vector3 previousAccelometerValues = Vector3.Zero;

        private Vector3 currentAccelometerValues = Vector3.Zero;

        public AccelerometerManager()
        {
            if (Accelerometer.IsSupported)
            {
                accelerometer = new Accelerometer();
                accelerometer.Start();
                accelerometerValueCheckInterval = new GameTimer(0.2f);
                accelerometerValueCheckInterval.OnTimedEvent += (o, e) => UpdateValues();
            }
        }

        /// <summary>
        /// Note: If accelerometer is not supported or failed to initialize then method will always return false.
        /// </summary>
        /// <returns>True if shake was detected.</returns>
        public bool ShakeDetected()
        {
            Vector3 result = currentAccelometerValues - previousAccelometerValues;
            if (Math.Abs(result.X + result.Y + result.Z) > shakeMinimalForce)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Update(GameTime gameTime)
        {
            accelerometerValueCheckInterval?.Update(gameTime);
        }

        private void UpdateValues()
        {
            previousAccelometerValues = currentAccelometerValues;
            currentAccelometerValues = accelerometer.CurrentValue.Acceleration;
        }
    }
}
