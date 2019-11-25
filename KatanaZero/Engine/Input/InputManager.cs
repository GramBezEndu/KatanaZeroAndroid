using System;
using System.Collections.Generic;
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
    }
}
