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
using Engine.Sprites;
using Engine.States;
using Microsoft.Xna.Framework;

namespace KatanaZero.States
{
    public class Hotel : GameState
    {
        public Hotel(Game1 gameReference) : base(gameReference)
        {
            game.PlaySong(songs["Stage1"]);
            SpawnPoliceCar();
            SpawnOfficer(game.LogicalSize.X *0.68f, "Game");
            gameCharacters.Add(player);
        }

        private void SpawnPoliceCar()
        {
            var car = new Sprite(commonTextures["PoliceCar"], new Vector2(3f, 3f));
            car.Position = new Vector2(game.LogicalSize.X/2, floorLevel - car.Size.Y);
            gameComponents.Add(car);
        }
    }
}