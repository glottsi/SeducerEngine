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
        public string Subtitle;
        public string Description;
        public ScenarioSettings Settings;
        public string Image;
        public string IntroVideo;
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
        public List<string> VideoFilename;
        public List<ConditionalVideos> Videos;
        public StoryPath Path;
        public ScoreAdjustment ScoreAdjustment;
        public List<Ending> Endings;
        public string EndScreenMessage;
    }

    // used to hold the previous game state, 
    // in case game ends and player wants to retry from previous choice.
    public class RestorePoint
    {
        public ScenarioSettings ScenarioState;
        public ButtonSchema LastChoice;
    }

    public class ConditionalVideos
    {
        public List<int> WhenPointsAreBetween;
        public List<string> VideoFilename;
    }

    public class Ending
    {
        public string EndScreenMessage;
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

    public class GameSettings
    {
        // not used yet
    }
}
