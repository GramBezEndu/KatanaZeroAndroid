using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Controls.Buttons;
using Engine.Input;

namespace Engine.Controls
{
    public class VerticalNavigationMenu : NavigationMenu
    {
        private Vector2 position;

        public VerticalNavigationMenu(InputManager im, List<IButton> listButtons) : base(im, listButtons)
        {
        }

        public override Vector2 Position
        {
            get => position;
            set
            {
                position = value;
                Vector2 currentPos = position;
                foreach (var button in buttons)
                {
                    button.Position = currentPos;
                    currentPos += new Vector2(0, button.Size.Y);
                }
            }
        }

        public override Vector2 Size
        {
            //Size: X equals the biggest width of buttons; Y equals sum of heights
            get
            {
                Vector2 size = Vector2.Zero;
                foreach (var button in buttons)
                {
                    if (button.Size.X > size.X)
                        size.X = button.Size.X;
                    size.Y += button.Size.Y;
                }
                return size;
            }
        }
    }
}
