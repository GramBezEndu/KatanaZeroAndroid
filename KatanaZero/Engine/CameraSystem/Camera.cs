using System;
using System.Collections.Generic;
using System.Text;
using Engine.Input;
using KatanaZero;
using Microsoft.Xna.Framework;

namespace Engine
{
    public class Camera : IComponent
    {
        public enum CameraModes
        {
            FollowPlayer,
            Static,
            ConstantVelocity
        }
        public CameraModes CameraMode = CameraModes.FollowPlayer;
        private bool deadZonesEnabled
        {
            get
            {
                if (CameraMode == CameraModes.Static)
                    return false;
                else
                    return true;
            }
        }
        readonly Game1 game;
        readonly Player player;
        public Matrix ViewMatrix { get; private set; } = Matrix.CreateTranslation(0, 0, 0);
        public Vector2 Position
        {
            get => _position;
            private set
            {
                //Dead zones
                float positionX = value.X;
                float positionY = value.Y;
                if (deadZonesEnabled)
                {
                    positionX = CheckOutOfBounceLeft(positionX);
                    positionX = CheckOutOfBounceRight(positionX);
                    positionY = CheckOutOfBounceBottom(positionY);
                }

                _position = new Vector2(positionX, positionY);
            }
        }

        private float CheckOutOfBounceBottom(float positionY)
        {
            if (positionY + (game.LogicalSize.Y * (1 / Zoom) - Origin.Y * (1 / Zoom)) > mapSize.Y)
            {
                //how much camera is too much to the bottom
                float distance = (positionY + (game.LogicalSize.Y * (1 / Zoom) - Origin.Y * (1 / Zoom))) - mapSize.Y;
                return positionY - distance;
            }
            return positionY;
        }

        private float CheckOutOfBounceRight(float x)
        {
            if (x + ((game.LogicalSize.X * (1 / Zoom)) - Origin.X * (1 / Zoom)) > mapSize.X)
            {
                float distance = (x + ((game.LogicalSize.X * (1 / Zoom)) - Origin.X * (1 / Zoom))) - mapSize.X;
                return x - distance;
            }
            return x;
        }

        private float CheckOutOfBounceLeft(float positionX)
        {
            if (positionX - Origin.X * (1 / Zoom) < 0)
            {
                float distance = positionX - Origin.X * (1 / Zoom);
                return positionX - distance;
            }
            return positionX;
        }

        public float Zoom { get; private set; } = 2.5f;
        public float MultiplierOriginX { get; set; } = 0.25f;
        public Vector2 Origin { get; private set; } = Vector2.Zero;
        private Vector2 constantVelocity = new Vector2(10f, 0f);
        private Vector2 mapSize;
        private Vector2 _position = Vector2.Zero;

        public Camera(Game1 gameReference, Vector2 mapSize, Player p)
        {
            game = gameReference;
            this.mapSize = mapSize;
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
                case CameraModes.ConstantVelocity:
                    ConstVelocity();
                    break;
            }
        }

        private void ConstVelocity()
        {
            Position = new Vector2(Position.X + constantVelocity.X, Position.Y + constantVelocity.Y);
            CalculateViewMatrix();
        }

        private void FollowPlayer()
        {
            Origin = new Vector2(game.LogicalSize.X * MultiplierOriginX - player.Size.X / 2, game.LogicalSize.Y * (3 / 4f) - player.Size.Y / 2);
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
            ViewMatrix = Matrix.CreateTranslation(-Position.X, -Position.Y, 0) * Matrix.CreateTranslation(Origin.X * (1 / Zoom), Origin.Y * (1 / Zoom), 0);
            ViewMatrix *= Matrix.CreateScale(new Vector3(Zoom, Zoom, 1));
        }
    }
}
