namespace Engine.Controls
{
    using System.Collections.Generic;
    using Engine.Controls.Buttons;
    using Engine.Input;
    using Microsoft.Xna.Framework;

    public class VerticalNavigationMenu : NavigationMenu
    {
        private const int margin = 20;
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
                foreach (IButton button in buttons)
                {
                    button.Position = currentPos;
                    currentPos += new Vector2(0, button.Size.Y + margin);
                }
            }
        }

        public override Vector2 Size
        {
            // Size: X equals the biggest width of buttons; Y equals sum of heights + margins
            get
            {
                Vector2 size = Vector2.Zero;
                for (int i = 0; i < buttons.Count; i++)
                {
                    if (buttons[i].Size.X > size.X)
                    {
                        size.X = buttons[i].Size.X;
                    }

                    size.Y += buttons[i].Size.Y;
                }

                for (int i = 0; i < buttons.Count - 1; i++)
                {
                    size.Y += margin;
                }

                return size;
            }
        }
    }
}
