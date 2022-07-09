namespace Engine.Controls
{
    using System.Collections.Generic;
    using Engine.Controls.Buttons;
    using Engine.Input;
    using Microsoft.Xna.Framework;

    public class VerticalNavigationMenu : NavigationMenu
    {
        private const int MarginY = 20;

        private Vector2 position;

        public VerticalNavigationMenu(InputManager im, List<IButton> listButtons)
            : base(im, listButtons)
        {
        }

        public override Vector2 Position
        {
            get => position;
            set
            {
                position = value;
                Vector2 currentPos = position;
                foreach (IButton button in Buttons)
                {
                    button.Position = currentPos;
                    currentPos += new Vector2(0, button.Size.Y + MarginY);
                }
            }
        }

        public override Vector2 Size
        {
            // Size: X equals the biggest width of buttons; Y equals sum of heights + margins
            get
            {
                Vector2 size = Vector2.Zero;
                for (int i = 0; i < Buttons.Count; i++)
                {
                    if (Buttons[i].Size.X > size.X)
                    {
                        size.X = Buttons[i].Size.X;
                    }

                    size.Y += Buttons[i].Size.Y;
                }

                for (int i = 0; i < Buttons.Count - 1; i++)
                {
                    size.Y += MarginY;
                }

                return size;
            }
        }
    }
}
