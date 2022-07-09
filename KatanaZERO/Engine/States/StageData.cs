namespace Engine.States
{
    using MonoGame.Extended.Tiled;
    using MonoGame.Extended.Tiled.Renderers;

    public class StageData
    {
        public TiledMap Map { get; set; }

        public TiledMapRenderer MapRenderer { get; set; }
    }
}
