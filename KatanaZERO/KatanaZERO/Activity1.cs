namespace KatanaZERO
{
    using Android.App;
    using Android.Content.PM;
    using Android.OS;
    using Android.Views;
    using Microsoft.Xna.Framework;

    [Activity(
        Label = "KatanaZero",
        MainLauncher = true,
        Icon = "@drawable/icon",
        Theme = "@style/Theme.Splash",
        AlwaysRetainTaskState = true,
        LaunchMode = Android.Content.PM.LaunchMode.SingleInstance,
        ScreenOrientation = ScreenOrientation.Landscape,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize | ConfigChanges.ScreenLayout)]
    public class Activity1 : AndroidGameActivity
    {
        private Game1 game;

        private View view;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            game = new Game1();
            view = game.Services.GetService(typeof(View)) as View;

            SetContentView(view);
            game.Run();
        }
    }
}
