using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Engine;
using Engine.SpecialEffects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KatanaZero.SpecialEffects
{
    public class Rain : IDrawableComponent
    {
        public bool Hidden { get; set; }
        public Vector2 Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Vector2 Size => throw new NotImplementedException();

        public Rectangle Rectangle => throw new NotImplementedException();

        public Color Color 
        { 
            get => color; 
            set
            {
                if(color != value)
                {
                    color = value;
                    foreach (var r in rainComponents)
                        r.Color = color;
                }
            }
        }

        public List<SpecialEffect> SpecialEffects { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        List<DrawableRectangle> rainComponents = new List<DrawableRectangle>();
        Random random = new Random();
        private Color color;

        public Rain()
        {
            Color = Color.DarkGray;
            for (int i = 0; i < 35; i++)
            {
                rainComponents.Add(new DrawableRectangle(new Rectangle(5 + i * 41, 0 - random.Next(50, 400), 7, 30 + random.Next(0, 20)))
                {
                    Color = this.Color * 0.14f,
                    Filled = true
                });
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!Hidden)
            {
                foreach (var r in rainComponents)
                    r.Draw(gameTime, spriteBatch);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (!Hidden)
            {
                foreach (var r in rainComponents)
                {
                    r.Position = new Vector2(r.Position.X, r.Position.Y + 40 /*random.Next(15, 40)*/);
                    if (r.Position.Y >= 720)
                        r.Position = new Vector2(r.Position.X, 0 - random.Next(50, 400));
                    r.Update(gameTime);
                }
            }
        }

        public void AddSpecialEffect(SpecialEffect effect)
        {
            throw new NotImplementedException();
        }
    }
}