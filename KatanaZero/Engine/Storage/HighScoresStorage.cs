using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Engine.Storage
{
    public class HighScoresStorage
    {
        private static string fullPath;
        private static string filename = "highscores.xml";
        public List<ClubNeonScore> ClubNeonScores;
        private int maxHighscoreCount = 5;
        private static HighScoresStorage instance;
        public static HighScoresStorage Instance
        {
            get
            {
                //Create full file path to check if user has saved data
                if (fullPath == null)
                    CreateFullPath();

                if (instance == null)
                {
                    if(!File.Exists(fullPath))
                        return new HighScoresStorage();
                    else
                    {
                        using (var reader = new StreamReader(new FileStream(fullPath, FileMode.Open)))
                        {
                            var serilizer = new XmlSerializer(typeof(List<ClubNeonScore>));
                            var scores = (List<ClubNeonScore>)serilizer.Deserialize(reader);
                            return new HighScoresStorage(scores);
                        }
                    }
                }
                else
                {
                    return instance;
                }
            }
        }

        private HighScoresStorage(List<ClubNeonScore> scores) : this()
        {
            ClubNeonScores = scores;
        }

        private HighScoresStorage()
        {
            ClubNeonScores = new List<ClubNeonScore>();
        }

        private static void CreateFullPath()
        {
            string[] paths =
{
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),
                filename
            };
            fullPath = Path.Combine(paths);
        }

        public void AddTime(ClubNeonScore s)
        {
            ClubNeonScores.Add(s);
            //Order and take best N scores
            ClubNeonScores = ClubNeonScores.OrderBy(x => x.Time).ToList();
            ClubNeonScores = ClubNeonScores.Take(maxHighscoreCount).ToList();
            Save();
        }

        public void Save()
        {
            using (var writer = new StreamWriter(new FileStream(fullPath, FileMode.Create)))
            {
                var serilizer = new XmlSerializer(typeof(List<ClubNeonScore>));

                serilizer.Serialize(writer, ClubNeonScores);
            }
        }
    }
}
