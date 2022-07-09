namespace Engine
{
    using KatanaZERO;
    using Microsoft.Xna.Framework;

    public enum CameraModes
    {
        FollowPlayer,
        Static,
        ConstantVelocity,
    }

    public class Camera : IComponent
    {
        private readonly Game1 game;

        private readonly Player player;

        private Vector2 mapSize;

        private Vector2 position = Vector2.Zero;

        public Camera(Game1 gameReference, Vector2 mapSize, Player p)
        {
            game = gameReference;
            this.mapSize = mapSize;
            player = p;
        }

        public float Zoom { get; private set; } = 2.5f;

        public float MultiplierOriginX { get; set; } = 0.25f;

        public Vector2 Origin { get; private set; } = Vector2.Zero;

        public Vector2 ConstantVelocity
        {
            get
            {
                if (player.NitroActive)
                {
                    return Player.BikeVelocity + Player.NitroBonus;
                }
                else
                {
                    return Player.BikeVelocity;
                }
            }
        }

        public CameraModes CameraMode { get; set; } = CameraModes.FollowPlayer;

        public bool DeadZonesEnabled
        {
            get
            {
                if (CameraMode == CameraModes.Static)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public Vector2 TopLeftPosition
        {
            get
            {
                Matrix invertedMatrix = Matrix.Invert(ViewMatrix);
                return new Vector2(invertedMatrix.Translation.X, invertedMatrix.Translation.Y);
            }
        }

        public Matrix ViewMatrix { get; private set; } = Matrix.CreateTranslation(0, 0, 0);

        public Vector2 Position
        {
            get => position;
            private set
            {
                // Dead zones
                float positionX = value.X;
                float positionY = value.Y;
                if (DeadZonesEnabled)
                {
                    positionX = CheckOutOfBounceLeft(positionX);
                    positionX = CheckOutOfBounceRight(positionX);
                    positionY = CheckOutOfBounceBottom(positionY);
                }

                position = new Vector2(positionX, positionY);
            }
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

        public Vector2 ScreenToWorld(Vector2 position)
        {
            return Vector2.Transform(
                new Vector2((int)(position.X / (game.WindowSize.X / game.LogicalSize.X)), (int)(position.Y / (game.WindowSize.Y / game.LogicalSize.Y))),
                Matrix.Invert(ViewMatrix));
        }

        public Vector2 WorldToScreen(Vector2 position)
        {
            Vector2 pos = new Vector2(
                (position.X - TopLeftPosition.X) * (game.WindowSize.X / game.LogicalSize.X),
                (position.Y - TopLeftPosition.Y) * (game.WindowSize.Y / game.LogicalSize.Y));

            // Scale with zoom ratio
            return new Vector2(pos.X * Zoom, pos.Y * Zoom);
        }

        private void ConstVelocity()
        {
            Position = new Vector2(Position.X + ConstantVelocity.X, Position.Y + ConstantVelocity.Y);
            CalculateViewMatrix();
        }

        private void FollowPlayer()
        {
            Origin = new Vector2(
                (game.LogicalSize.X * MultiplierOriginX) - (player.Size.X / 2),
                (game.LogicalSize.Y * 0.75f) - (player.Size.Y / 2));
            Position = new Vector2(player.Position.X, player.Position.Y);

            // After updating position we and origin can calculate view matrix
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

        private float CheckOutOfBounceTop(float positionY)
        {
            if (positionY - (Origin.Y * (1 / Zoom)) < 0f)
            {
                // how much camera is too much to the top
                float distance = positionY - (Origin.Y * (1 / Zoom));
                return positionY - distance;
            }

            return positionY;
        }

        private float CheckOutOfBounceBottom(float positionY)
        {
            if (positionY + ((game.LogicalSize.Y * (1 / Zoom)) - (Origin.Y * (1 / Zoom))) > mapSize.Y)
            {
                // how much camera is too much to the bottom
                float distance = (positionY + ((game.LogicalSize.Y * (1 / Zoom)) - (Origin.Y * (1 / Zoom)))) - mapSize.Y;
                return positionY - distance;
            }

            return positionY;
        }

        private float CheckOutOfBounceRight(float x)
        {
            if (x + ((game.LogicalSize.X * (1 / Zoom)) - (Origin.X * (1 / Zoom))) > mapSize.X)
            {
                float distance = (x + ((game.LogicalSize.X * (1 / Zoom)) - (Origin.X * (1 / Zoom)))) - mapSize.X;
                return x - distance;
            }

            return x;
        }

        private float CheckOutOfBounceLeft(float positionX)
        {
            if (positionX - (Origin.X * (1 / Zoom)) < 0)
            {
                float distance = positionX - (Origin.X * (1 / Zoom));
                return positionX - distance;
            }

            return positionX;
        }
    }
}
