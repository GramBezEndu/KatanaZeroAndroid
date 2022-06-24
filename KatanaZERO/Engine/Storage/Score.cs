using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Storage
{
    public class Score
    {
        public double Time;
        public int LevelId;

        public Score()
        {

        }
        public Score(int levelId, double time)
        {
            LevelId = levelId;
            Time = time;
        }
    }
}
