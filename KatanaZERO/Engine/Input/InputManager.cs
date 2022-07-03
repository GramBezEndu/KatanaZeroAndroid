namespace Engine.Input
{
    using KatanaZERO;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input.Touch;

    public class InputManager : IComponent
    {
        private readonly AccelerometerManager accelometerManager;

        private readonly Game1 game;

        public TouchCollection CurrentTouchCollection { get; private set; }
        /// <summary>
        /// Initializes new InputManager
        /// </summary>
        public InputManager(Game1 g)
        {
            accelometerManager = new AccelerometerManager();
            game = g;
        }

        public void Update(GameTime gameTime)
        {
            CurrentTouchCollection = TouchPanel.GetState();
            accelometerManager.Update(gameTime);
        }

        public bool RectangleWasJustClicked(Rectangle rec)
        {
            foreach (TouchLocation touchLocation in CurrentTouchCollection)
            {
                Rectangle touchRectangle = new Rectangle((int)(touchLocation.Position.X / (game.WindowSize.X / game.LogicalSize.X)), (int)(touchLocation.Position.Y / (game.WindowSize.Y / game.LogicalSize.Y)), 1, 1);
                if (touchRectangle.Intersects(rec) && touchLocation.State == TouchLocationState.Pressed)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if rectangle placed in world coordinates was clicked
        /// Implementation note: The tochLocation position is translated to world (game) coordinates
        /// </summary>
        /// <param name="rec"></param>
        /// <param name="camera"></param>
        /// <returns></returns>
        //public bool WorldRectnagleWasJustClicked(Rectangle rec, Camera camera)
        //{
        //    foreach (TouchLocation touchLocation in CurrentTouchCollection)
        //    {
        //        Vector2 touchPosition = Vector2.Transform(new Vector2((int)(touchLocation.Position.X / (game.WindowSize.X / game.LogicalSize.X)), (int)(touchLocation.Position.Y / (game.WindowSize.Y / game.LogicalSize.Y))),
        //            Matrix.Invert(camera.ViewMatrix));
        //        var touchRectangle = new Rectangle((int)touchPosition.X, (int)touchPosition.Y, 1, 1);
        //        if (touchRectangle.Intersects(rec) && touchLocation.State == TouchLocationState.Pressed)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        public bool AnyTapDetected()
        {
            foreach (TouchLocation touchLocation in CurrentTouchCollection)
            {
                if (touchLocation.State == TouchLocationState.Pressed)
                {
                    return true;
                }
            }

            return false;
        }

        public bool ShakeDetected()
        {
            return accelometerManager.ShakeDetected();
        }
    }
}
