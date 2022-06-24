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
        public List<Score> Scores;
        public const int MAX_HIGHSCORES_COUNT = 5;
        private static HighScoresStorage instance;
        public static HighScoresStorage Instance
        {
            get
            {
                //Create full file path to check if user has saved data
                if (fullPath == null)
                    CreateFullPath();

                //Create instance if does not exist
                if (instance == null)
                {
                    if(!File.Exists(fullPath))
                    {
                        instance = new HighScoresStorage();
                    }
                    else
                    {
                        using (var reader = new StreamReader(new FileStream(fullPath, FileMode.Open)))
                        {
                            var serilizer = new XmlSerializer(typeof(List<Score>));
                            try
                            {
                                var scores = (List<Score>)serilizer.Deserialize(reader);
                                instance = new HighScoresStorage(scores);
                            }
                            catch(InvalidOperationException e)
                            {
                                System.Diagnostics.Debug.WriteLine(e.Message);
                                System.Diagnostics.Debug.WriteLine("Inner exception: " + e.InnerException.Message);
                                instance = new HighScoresStorage();
                            }
                        }
                    }
                }
                //Return highscores storage
                return instance;
            }
        }

        private HighScoresStorage(List<Score> scores) : this()
        {
            Scores = scores;
        }

        private HighScoresStorage()
        {
            Scores = new List<Score>();
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

        public void AddTime(Score s)
        {
            Scores.Add(s);
            //Order and take best N scores for each level ID
            //1. Find all distinct ids
            int[] allIds = Scores.Select(x => x.LevelId).ToArray();
            int[] allDistnictIds = allIds.Distinct().ToArray();
            //2. Find N best scores for each level id
            List<Score> tempScores = new List<Score>();
            for (int i = 0; i < allDistnictIds.Length; i++)
            {
                var bestScoresForThisId = Scores.Where(x => x.LevelId == allDistnictIds[i]).OrderBy(x => x.Time).Take(MAX_HIGHSCORES_COUNT).ToList();
                tempScores.AddRange(bestScoresForThisId);
            }

            Scores = tempScores;
            Save();
        }

        public void Save()
        {
            using (var writer = new StreamWriter(new FileStream(fullPath, FileMode.Create)))
            {
                var serilizer = new XmlSerializer(typeof(List<Score>));

                serilizer.Serialize(writer, Scores);
            }
        }

        public double[] GetBestScores(int levelId)
        {
            var bestScores = Scores.Where(x => x.LevelId == levelId).OrderBy(x => x.Time).Take(MAX_HIGHSCORES_COUNT).ToArray();
            return bestScores.Select(x => x.Time).ToArray();
        }
    }
}
