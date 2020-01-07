using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Storage
{
    public class HighScoresStorage
    {
        public List<ClubNeonScore> ClubNeonScores;
        private int maxHighscoreCount = 5;
        private static HighScoresStorage instance;
        public static HighScoresStorage Instance
        {
            get
            {
                if (instance == null)
                    instance = new HighScoresStorage();
                return instance;
            }
        }
        private HighScoresStorage()
        {
            ClubNeonScores = new List<ClubNeonScore>();
        }
        public void AddTime(ClubNeonScore s)
        {
            ClubNeonScores.Add(s);
            //Order and take best N scores
            ClubNeonScores = ClubNeonScores.OrderBy(x => x.Time).ToList();
            ClubNeonScores = ClubNeonScores.Take(maxHighscoreCount).ToList();
        }
    }
}
