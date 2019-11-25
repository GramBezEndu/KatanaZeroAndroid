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
    public abstract class NavigationMenu : IDrawableComponent
    {
        protected readonly InputManager inputManager;
        protected List<IButton> buttons;
        public bool Hidden { get; set; }
        /// <summary>
        /// Position determines beginning of the navigation menu
        /// </summary>
        public abstract Vector2 Position { get; set; }

        public abstract Vector2 Size { get; }

        public Color Color { get; set; }

        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
            }
        }

        /// <summary>
        /// Creates a new instance of navigation through buttons
        /// </summary>
        /// <param name="listButtons"></param>
        public NavigationMenu(InputManager im, List<IButton> listButtons)
        {
            inputManager = im;
            if (listButtons == null || listButtons.Count < 1)
                throw new ArgumentException("Invalid list of buttons");
            buttons = listButtons;
        }
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!Hidden)
            {
                foreach (var button in buttons)
                    button.Draw(gameTime, spriteBatch);
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            if (!Hidden)
            {
                UpdateButtons(gameTime);
            }
        }

        private void UpdateButtons(GameTime gameTime)
        {
            foreach (var button in buttons)
                button.Update(gameTime);
        }
    }
}
