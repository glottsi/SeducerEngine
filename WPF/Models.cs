using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF
{

    public enum ButtonType
    {
        Default,
        End // instant game over
    }

    // used to deserialize from json
    public class ScenarioSchema
    {
        public string Title;
        public string Description;
        public ScenarioSettings Settings;
    }

    public class ScenarioSettings
    {
        public int StartingHP;
        public int StartingPoints;
        public string StartingBranch;
        public int StartingPathPosition;
    }

    // used to deserialize from json
    public class ButtonSchema
    {
        public ButtonType ButtonType;
        public string Label;
        public string VideoFilename;
        public StoryPath Path;
        public ScoreAdjustment ScoreAdjustment;
        public List<Ending> Endings;
    }

    public class Ending
    {
        public List<int> WhenPointsAreBetween;
        public string VideoFilename;
    }

    public class StoryPath
    {
        public string Branch;
        public int StartPosition;
    }

    public class ScoreAdjustment
    {
        public int HP;
        public int Points;
    }

    public class ButtonData
    {
        public ButtonType ButtonType;
        public StoryPath StoryPath;
        public string VideoFileLocation;
        public ScoreAdjustment ScoreAdjustment;
        public List<Ending> Endings;
    }
}
