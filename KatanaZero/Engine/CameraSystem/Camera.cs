using System;
using System.Collections.Generic;
using System.Text;
using Engine.Input;
using KatanaZero;
using Microsoft.Xna.Framework;

namespace Engine
{
    /// <summary>
    /// Camera is controlled now by player (for display purposes), it will be locked in the future
    /// </summary>
    public class Camera : IComponent
    {
        public enum CameraModes
        {
            FollowPlayer,
            Static
        }
        public CameraModes CameraMode = CameraModes.FollowPlayer;
        readonly Game1 game;
        readonly Player player;
        public Matrix ViewMatrix { get; private set; } = Matrix.CreateTranslation(0, 0, 0);
        public Vector2 Position { get; private set; } = Vector2.Zero;
        public float Zoom { get; private set; } = 4f;

        public Vector2 Origin { get; private set; } = Vector2.Zero;

        private Vector2 freeroamVelocity = new Vector2(10f, 10f);
        public Camera(Game1 gameReference, Player p)
        {
            game = gameReference;
            player = p;
        }
        public void Update(GameTime gameTime)
        {
            switch (CameraMode)
            {
                case CameraModes.FollowPlayer:
                    FollowPlayer();
                    break;
                case CameraModes.Static:
                    StaticCamera();
                    break;
            }
        }

        private void FollowPlayer()
        {
            Origin = new Vector2(game.LogicalSize.X / 2 - player.Size.X / 2, game.LogicalSize.Y * (3 / 4f) - player.Size.Y / 2);
            Position = new Vector2(player.Position.X, player.Position.Y);

            //After updating position we and origin can calculate view matrix
            CalculateViewMatrix();
        }

        private void StaticCamera()
        {
            CalculateViewMatrix();
        }

        private void CalculateViewMatrix()
        {
            ViewMatrix = Matrix.CreateTranslation(-Position.X, -Position.Y, 0) * Matrix.CreateTranslation(Origin.X * (1/Zoom), Origin.Y * (1/ Zoom), 0);
            ViewMatrix *= Matrix.CreateScale(new Vector3(Zoom, Zoom, 1));
        }
    }
}
