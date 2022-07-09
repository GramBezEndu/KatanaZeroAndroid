namespace Engine.Storage
{
    public class Score
    {
        public Score()
        {
        }

        public Score(int levelId, double time)
        {
            LevelId = levelId;
            Time = time;
        }

        public double Time { get; set; }

        public int LevelId { get; set; }
    }
}
