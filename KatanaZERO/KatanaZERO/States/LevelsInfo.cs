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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KatanaZERO.States
{
    public static class LevelsInfo
    {
        public static LevelInfo[] LevelInfo { get; private set; }
        public static void Init(Game1 game, ContentManager content)
        {
            LevelInfo = new LevelInfo[]
            {
                new LevelInfo(0, "CLUB NEON", content.Load<Texture2D>("Textures/LevelSelect/ClubNeon"), () => game.ChangeState(new ClubNeon(game, 0, true))),
                new LevelInfo(1, "PRISON", content.Load<Texture2D>("Textures/LevelSelect/Prison"), () => game.ChangeState(new PrisonPart1(game, 1, true))),
                new LevelInfo(2, "BIKE ESCAPE", content.Load<Texture2D>("Textures/LevelSelect/Escape"), () => game.ChangeState(new BikeEscape(game, 2, true))),
            };
        }
    }
}