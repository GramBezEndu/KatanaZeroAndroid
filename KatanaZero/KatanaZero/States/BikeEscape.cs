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
    public class BikeEscape : GameState
    {
        public BikeEscape(Game1 gameReference, int levelId, bool showLevelTitle) : base(gameReference, levelId, showLevelTitle)
        {
            game.PlaySong(songs["BikeEscape"]);
            Camera.CameraMode = Engine.Camera.CameraModes.ConstantVelocity;
        }

        public override string LevelName => "TEST";

        public override void SetPlayerSpawnPoint()
        {
            //
        }

        protected override void LoadMap()
        {
            map = content.Load<TiledMap>("Maps/Escape/Escape");
        }

        internal override void RestartLevel()
        {
            //
        }

        internal override Vector2 SetMapSize()
        {
            return new Vector2(42900, 334);
        }
    }
}