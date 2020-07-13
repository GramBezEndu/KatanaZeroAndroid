using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Engine.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Tiled;

namespace KatanaZero.States
{
    public class PrisonPart1 : GameState
    {
        public PrisonPart1(Game1 gameReference, bool showLevelTitle) : base(gameReference, showLevelTitle)
        {
            game.PlaySong(content.Load<Song>("Songs/Prison"));
        }

        public override string LevelName { get { return "PRISON"; } }

        public override void SetPlayerSpawnPoint()
        {
            player.Position = new Vector2(10, 200);
        }

        protected override void AddHighscore()
        {
            //throw new NotImplementedException();
        }

        protected override void LoadMap()
        {
            map = content.Load<TiledMap>("Maps/PrisonPart1/PrisonPart1");
        }

        internal override Vector2 SetMapSize()
        {
            return new Vector2(1480, 464);
        }
    }
}