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
using Engine.States;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;

namespace KatanaZero.States
{
    public class PrisonPart2 : GameState
    {
        public PrisonPart2(Game1 gameReference, bool showLevelTitle) : base(gameReference, showLevelTitle)
        {
        }

        public override string LevelName { get { return "PRISON"; } }

        public override void SetPlayerSpawnPoint()
        {
            player.Position = new Vector2(40, 170);
        }

        protected override void AddHighscore()
        {
            //throw new NotImplementedException();
        }

        protected override void LoadMap()
        {
            map = content.Load<TiledMap>("Maps/PrisonPart2/PrisonPart2");
        }

        internal override Vector2 SetMapSize()
        {
            return new Vector2(1480, 464);
        }
    }
}