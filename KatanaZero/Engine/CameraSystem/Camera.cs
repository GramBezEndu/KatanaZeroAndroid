using System;
using System.Collections.Generic;
using System.Text;
using KatanaZero;
using Microsoft.Xna.Framework;

namespace Engine
{
    public class Camera : IComponent
    {
        public enum CameraModes
        {
            Static
        }
        public CameraModes CameraMode = CameraModes.Static;
        readonly Game1 game;
        public Matrix ViewMatrix { get; private set; } = Matrix.CreateTranslation(0, 0, 0);
        public Vector2 Position { get; set; } = Vector2.Zero;
        public Camera(Game1 gameReference)
        {
            game = gameReference;
        }
        public void Update(GameTime gameTime)
        {
            switch (CameraMode)
            {
                case CameraModes.Static:
                    StaticCamera();
                    break;
            }
        }

        private void StaticCamera()
        {
            CalculateViewMatrix();
        }

        private void CalculateViewMatrix()
        {
            ViewMatrix = Matrix.CreateTranslation(-Position.X, -Position.Y, 0);
        }
    }
}
