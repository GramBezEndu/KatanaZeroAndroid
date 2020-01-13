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
using Engine.Controls;
using Engine.Controls.Buttons;
using Engine.Sprites;
using Engine.States;
using Engine.Storage;
using Microsoft.Xna.Framework;

namespace KatanaZero.States
{
    public class Rankings : State
    {
        public Rankings(Game1 gameReference) : base(gameReference)
        {
            AddUiComponent(new Sprite(commonTextures["MainMenu"]));
            CreateJobFolder();
            //Rectangle backgroundRankings = new Rectangle((int)(game.LogicalSize.X * (0.2f)), (int)(game.LogicalSize.Y * (0.1f)), (int)(game.LogicalSize.X * 0.6f), (int)(game.LogicalSize.Y * 0.7f));
            //AddUiComponent(new DrawableRectangle(backgroundRankings)
            //{
            //    Filled = true,
            //    Color = Color.Black * 0.9f,
            //});
            var backButton = new RectangleButton(inputManager, new Rectangle(0, 0, (int)(game.LogicalSize.X * 0.5f), (int)game.LogicalSize.Y / 10), fonts["Standard"], "BACK")
            {
                Color = Color.Gray * 0.3f,
                Filled = true,
                OnClick = (o, e) => game.ChangeState(new MainMenu(game))
            };
            var menu = new VerticalNavigationMenu(inputManager, new List<IButton>
            {
                backButton,
            });
            menu.Position = new Vector2(game.LogicalSize.X / 2 - menu.Size.X / 2, game.LogicalSize.Y * (0.9f) - menu.Size.Y / 2);
            AddUiComponent(menu);
        }

        private void CreateJobFolder()
        {
            float jobFolderScale = 2f;
            var folderFront = new Sprite(commonTextures["JobFolderFrontOpen"], new Vector2(jobFolderScale));
            folderFront.Position = new Vector2(game.LogicalSize.X * (0.15f), game.LogicalSize.Y * (0.05f));
            var folderBack = new Sprite(commonTextures["JobFolderBack"], new Vector2(jobFolderScale));
            folderBack.Position = new Vector2(folderFront.Rectangle.Right, game.LogicalSize.Y * (0.05f));
            var clubNeonText = new Text(fonts["Small"], "CLUB\nNEON")
            {
                Color = Color.Blue,
            };
            clubNeonText.Position = new Vector2(game.LogicalSize.X * (0.53f), game.LogicalSize.Y * (0.12f));

            AddUiComponent(folderFront);
            AddUiComponent(folderBack);
            AddUiComponent(clubNeonText);
            AddHighscoresComponents();
        }

        private void AddHighscoresComponents()
        {
            var color = Color.Black;
            var colorNoData = Color.DarkRed;
            var position = new Vector2(game.LogicalSize.X * 0.5f, game.LogicalSize.Y * 0.4f);
            var bestTimesText = new Text(fonts["Small"], "BEST TIMES:")
            {
                Position = position,
                Color = color,
            };
            position = new Vector2(position.X, position.Y + bestTimesText.Size.Y);
            AddUiComponent(bestTimesText);
            if(HighScoresStorage.Instance.ClubNeonScores.Count == 0)
            {
                var noDataFound = new Text(fonts["Small"], "NO DATA FOUND")
                {
                    Position = position,
                    Color = colorNoData,
                };
                position = new Vector2(position.X, position.Y + noDataFound.Size.Y);
                AddUiComponent(noDataFound);
            }
            else
            {
                for (int i = 0; i < HighScoresStorage.Instance.ClubNeonScores.Count; i++)
                {
                    var text = new Text(fonts["Small"], String.Format("{0}. {1} s", i + 1, Math.Round(HighScoresStorage.Instance.ClubNeonScores[i].Time, 2).ToString()))
                    {
                        Position = position,
                        Color = color
                    };
                    AddUiComponent(text);
                    position = new Vector2(position.X, position.Y + text.Size.Y);
                }
            }
        }
    }
}