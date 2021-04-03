using System;
using System.Collections.Generic;

namespace WPF
{
    public static class MainResources
    {
        public static MainWindow MainWindow;
        public static List<bool> Scores;

        private static int _HP;
        private static int _Points;
        private static string _Branch;
        private static int _PathPosition;

        private static string _endingRootPath;
        private static string _endingVideoPath;

        private static readonly Random Rand = new Random(Guid.NewGuid().GetHashCode());
        
        // Look at my amazing Fisher-Yates implementation
        public static void Shuffle<T>(this IList<T> list)
        {
            for (int i = list.Count - 1; i > 1; i--)
            {
                int j = Rand.Next(i + 1);
                T obj = list[i];
                list[i] = list[j];
                list[j] = obj;
            }
        }

        public static void SetScenarioSettings(ScenarioSettings settings)
        {
            
            _HP = settings.StartingHP;
            _Points = settings.StartingPoints;
            _Branch = settings.StartingBranch;
            _PathPosition = settings.StartingPathPosition;
        }

        public static void ClearScenarioSettings()
        {

        }


        public static string GetEndingVideoPath()
        {
            return _endingVideoPath;
        }

        public static void SetEndingVideoPath(string path)
        {
            _endingVideoPath = path;
        }

        public static void SetEndingPathRoot(string path)
        {
            _endingRootPath = path;
        }

        public static string GetEndingPathRoot()
        {
            return _endingRootPath;
        }

        public static void AdjustPoints(int amount)
        {
            _Points += amount;
        }

        public static void SetPoints(int amount)
        {
            _Points = amount;
        }

        public static int GetPoints()
        {
            return _Points;
        }

        public static void AdjustHP(int amount)
        {
            _HP += amount;
        }

        public static void SetHP(int amount)
        {
            _HP = amount;
        }

        public static int GetHP()
        {
            return _HP;
        }

        public static void SetBranch(string branch)
        {
           
            _Branch = branch.ToUpper().Trim();
            
        }

        public static string GetBranch()
        {
            return _Branch;
        }

        public static void SetPathPosition(int position)
        {

            _PathPosition = position;

        }

        public static int GetPathPosition()
        {
            return _PathPosition;
        }
    }
}