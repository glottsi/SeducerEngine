using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF
{

    public class ScenarioSchema
    {
        public string Title;
        public string Description;
    }

    public class ButtonSchema
    {
        public ButtonType ButtonType;
        public string Label;
        public string VideoFilename;
        public StoryPath Path;
    }

    public class StoryPath
    {
        public string Branch;
        public int StartPosition;
    }
}
