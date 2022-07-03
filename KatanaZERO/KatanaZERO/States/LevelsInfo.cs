namespace KatanaZERO.States
{
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

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