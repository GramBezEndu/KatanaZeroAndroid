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
        public int Id { get; set; }
        public string Name { get; set; }
        public Texture2D Texture { get; set; }
        public Action StartLevel { get; set; }
        public LevelInfo(int id, string name, Texture2D texture, Action startLevel)
        {
            Id = id;
            Name = name;
            Texture = texture;
            StartLevel = startLevel;
        }
    }
}