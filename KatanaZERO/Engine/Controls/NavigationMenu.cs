namespace Engine.Controls
{
    using System;
    using System.Collections.Generic;
    using Engine.Controls.Buttons;
    using Engine.Input;
    using Engine.SpecialEffects;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public abstract class NavigationMenu : IDrawableComponent
    {
        private readonly InputManager inputManager;

        /// <summary>
        /// Creates a new instance of navigation through buttons.
        /// </summary>
        /// <param name="listButtons">List of all buttons.</param>
        public NavigationMenu(InputManager im, List<IButton> listButtons)
        {
            inputManager = im;
            if (listButtons == null || listButtons.Count < 1)
            {
                throw new ArgumentException("Invalid list of buttons");
            }

            Buttons = listButtons;
        }

        public List<IButton> Buttons { get; protected set; }

        public bool Hidden { get; set; }

        /// <summary>
        /// Position determines beginning of the navigation menu.
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

        public List<SpecialEffect> SpecialEffects { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!Hidden)
            {
                foreach (IButton button in Buttons)
                {
                    button.Draw(gameTime, spriteBatch);
                }
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            if (!Hidden)
            {
                UpdateButtons(gameTime);
            }
        }

        public void AddSpecialEffect(SpecialEffect effect)
        {
            throw new NotImplementedException();
        }

        private void UpdateButtons(GameTime gameTime)
        {
            foreach (IButton button in Buttons)
            {
                button.Update(gameTime);
            }
        }
    }
}
