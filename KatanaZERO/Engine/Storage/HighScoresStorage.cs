namespace Engine.Storage
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;

    public class HighScoresStorage
    {
        private static readonly string FileName = "highscores.xml";

        private static string filePath;

        private List<Score> scores;

        public const int MaxHighscoresCount = 5;

        private static HighScoresStorage instance;

        public static HighScoresStorage Instance
        {
            get
            {
                // Create full file path to check if user has saved data
                if (filePath == null)
                {
                    CreateFullPath();
                }

                // Create instance if does not exist
                if (instance == null)
                {
                    if (!File.Exists(filePath))
                    {
                        instance = new HighScoresStorage();
                    }
                    else
                    {
                        using (StreamReader reader = new StreamReader(new FileStream(filePath, FileMode.Open)))
                        {
                            XmlSerializer serilizer = new XmlSerializer(typeof(List<Score>));
                            try
                            {
                                List<Score> scores = (List<Score>)serilizer.Deserialize(reader);
                                instance = new HighScoresStorage(scores);
                            }
                            catch (InvalidOperationException e)
                            {
                                System.Diagnostics.Debug.WriteLine(e.Message);
                                System.Diagnostics.Debug.WriteLine("Inner exception: " + e.InnerException.Message);
                                instance = new HighScoresStorage();
                            }
                        }
                    }
                }

                // Return highscores storage
                return instance;
            }
        }

        private HighScoresStorage(List<Score> scores)
            : this()
        {
            this.scores = scores;
        }

        private HighScoresStorage()
        {
            scores = new List<Score>();
        }

        private static void CreateFullPath()
        {
            string[] paths =
            {
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),
                FileName,
            };
            filePath = Path.Combine(paths);
        }

        public void AddTime(Score s)
        {
            scores.Add(s);

            // Order and take best N scores for each level ID
            // 1. Find all distinct ids
            int[] allIds = scores.Select(x => x.LevelId).ToArray();
            int[] allDistnictIds = allIds.Distinct().ToArray();

            // 2. Find N best scores for each level id
            List<Score> tempScores = new List<Score>();
            for (int i = 0; i < allDistnictIds.Length; i++)
            {
                List<Score> bestScoresForThisId = scores.Where(x => x.LevelId == allDistnictIds[i]).OrderBy(x => x.Time).Take(MaxHighscoresCount).ToList();
                tempScores.AddRange(bestScoresForThisId);
            }

            scores = tempScores;
            Save();
        }

        public void Save()
        {
            using (StreamWriter writer = new StreamWriter(new FileStream(filePath, FileMode.Create)))
            {
                XmlSerializer serilizer = new XmlSerializer(typeof(List<Score>));

                serilizer.Serialize(writer, scores);
            }
        }

        public double[] GetBestScores(int levelId)
        {
            Score[] bestScores = scores.Where(x => x.LevelId == levelId).OrderBy(x => x.Time).Take(MaxHighscoresCount).ToArray();
            return bestScores.Select(x => x.Time).ToArray();
        }
    }
}
