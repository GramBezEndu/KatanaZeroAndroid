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
using Microsoft.Xna.Framework.Graphics;

namespace KatanaZero
{
    public class LevelInfo
    {
        public string Name { get; set; }
        public Texture2D Texture { get; set; }
        public LevelInfo(string name, Texture2D texture)
        {
            Name = name;
            Texture = texture;
        }
    }
}