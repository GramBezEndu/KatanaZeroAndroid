namespace KatanaZERO
{
    using System;
    using Microsoft.Xna.Framework.Graphics;

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