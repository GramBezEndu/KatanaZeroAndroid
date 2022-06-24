using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Devices.Sensors;
using Microsoft.Xna.Framework;
using PlatformerEngine.Timers;

namespace Engine.Input
{
    public class AccelerometerManager : IComponent
    {
        private Accelerometer accelerometer;
        private Vector3 previousAccelometerValues = new Vector3();
        private Vector3 currentAccelometerValues = new Vector3();
        private GameTimer accelerometerValueCheckInterval;
        private const float shakeMinimumForce = 0.9f;
        public AccelerometerManager()
        {
            if (Accelerometer.IsSupported)
            {
                accelerometer = new Accelerometer();
                accelerometer.Start();
                accelerometerValueCheckInterval = new GameTimer(0.2f)
                {
                    OnTimedEvent = (o, e) => UpdateValues()
                };
            }
        }

        private void UpdateValues()
        {
            previousAccelometerValues = currentAccelometerValues;
            currentAccelometerValues = accelerometer.CurrentValue.Acceleration;
        }

        /// <summary>
        /// Note: If accelerometer is not supported or failed to initialize then method will always return false
        /// </summary>
        /// <returns></returns>
        public bool ShakeDetected()
        {
            Vector3 result = currentAccelometerValues - previousAccelometerValues;
            if (Math.Abs(result.X + result.Y + result.Z) > shakeMinimumForce)
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
    }
}
