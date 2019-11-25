using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine
{
    /// <summary>
    /// Currently text is being scaled
    /// </summary>
    public class Text : IDrawableComponent
    {
        private string _message;
        public bool Hidden { get; set; }
        public Vector2 Position { get; set; }
        protected SpriteFont font;
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
            }
        }

        public Vector2 Size
        {
            get
            {
                if (font != null && _message != null)
                    return new Vector2((font.MeasureString(_message).X) * Scale.X, (font.MeasureString(_message).Y) * Scale.Y);
                else
                    return Vector2.Zero;
            }
        }

        public Vector2 Scale { get; set; } = new Vector2(1f, 1f);

        public Color Color { get; set; } = Color.White;

        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
            }
        }

        public Text(SpriteFont f, String msg, Vector2 scale)
        {
            font = f;
            Message = msg;
            Scale = scale;
        }

        public Text(SpriteFont f, String msg)
        {
            font = f;
            Message = msg;
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!Hidden)
                spriteBatch.DrawString(font, Message, Position, Color, 0f, new Vector2(0, 0), Scale, SpriteEffects.None, 0f);
        }

        public virtual void Update(GameTime gameTime)
        {

        }
    }
}
