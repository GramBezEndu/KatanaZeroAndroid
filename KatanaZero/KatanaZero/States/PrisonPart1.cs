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
using MonoGame.Extended.Tiled;

namespace KatanaZero.States
{
    public class PrisonPart1 : GameState
    {
        public PrisonPart1(Game1 gameReference, bool showLevelTitle) : base(gameReference, showLevelTitle)
        {
        }

        protected override void AddHighscore()
        {
            //throw new NotImplementedException();
        }

        protected override void LoadMap()
        {
            try
            {
                map = content.Load<TiledMap>("Maps/PrisonPart1/PrisonPart1");

            }
            catch(ContentLoadException e)
            {
                Debug.WriteLine(e.HelpLink);
                Debug.WriteLine(e.Data);
                Debug.WriteLine(e.Message);
            }
        }

        internal override Vector2 SetMapSize()
        {
            return new Vector2(1295, 464);
        }
    }
}