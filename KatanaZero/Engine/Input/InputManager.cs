using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Engine.Input
{
    public class InputManager : IComponent
    {
        public TouchCollection CurrentTouchCollection { get; private set; }
        //public TouchCollection PreviousTouchCollection { get; private set; }
        /// <summary>
        /// Initializes new InputManager
        /// </summary>
        public InputManager()
        {

        }
        public void Update(GameTime gameTime)
        {
            //PreviousTouchCollection = CurrentTouchCollection;
            CurrentTouchCollection = TouchPanel.GetState();
            //Debug
            foreach (TouchLocation touchLocation in CurrentTouchCollection)
            {
                //Debug.WriteLine(String.Format("{0} Touch: {1} {2}", gameTime.TotalGameTime.TotalSeconds, touchLocation.Position.X, touchLocation.Position.Y));
            }
        }
        public bool RectangleWasJustClicked(Rectangle rec)
        {
            foreach(TouchLocation touchLocation in CurrentTouchCollection)
            {
                var touchRectangle = new Rectangle((int)touchLocation.Position.X, (int)touchLocation.Position.Y, 1, 1);
                if(touchRectangle.Intersects(rec) && touchLocation.State == TouchLocationState.Pressed)
                {
                    return true;
                }
                //touchLocation.State == TouchLocationState.
            }
            return false;
        }

        /// <summary>
        /// The tochLocation position needs to be translated to world space
        /// </summary>
        /// <param name="rec"></param>
        /// <param name="camera"></param>
        /// <returns></returns>
        public bool WorldRectnagleWasJustClicked(Rectangle rec, Camera camera)
        {
            //var position = new Vector2(rec.X, rec.Y);
            //var positionTranslated = Vector2.Transform(position, Matrix.Invert(camera.ViewMatrix));
            ////Zoom too?
            //var rectangleTranslated = new Rectangle((int)positionTranslated.X, (int)positionTranslated.Y, (int)(rec.Width * camera.Zoom), (int)(rec.Height * camera.Zoom));

            //Debug
            //Debug.WriteLine(rectangleTranslated);
            foreach (TouchLocation touchLocation in CurrentTouchCollection)
            {
                Vector2 touchPosition = Vector2.Transform(new Vector2(touchLocation.Position.X, touchLocation.Position.Y), Matrix.Invert(camera.ViewMatrix));
                var touchRectangle = new Rectangle((int)touchPosition.X, (int)touchPosition.Y, 1, 1);
                //Debug.WriteLine(String.Format("Touch location in worlds space: {0}", touchPosition));
                if (touchRectangle.Intersects(rec) && touchLocation.State == TouchLocationState.Pressed)
                {
                    return true;
                }
                //Vector2 mouseTranslated = new Vector2(touchLocation.Position.X, touchLocation.Position.Y);
                //mouseTranslated = Vector2.Transform(mouseTranslated, Matrix.Invert(camera.ViewMatrix));
                //Debug.WriteLine(String.Format("Touch location in worlds space: {0}", mouseTranslated));
                //touchLocation.State == TouchLocationState.
            }
            return false;
        }

        public bool AnyTapDetected()
        {
            foreach (TouchLocation touchLocation in CurrentTouchCollection)
            {
                if (touchLocation.State == TouchLocationState.Pressed)
                    return true;
            }
            return false;
        }
    }
}
