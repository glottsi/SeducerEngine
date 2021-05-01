using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF
{
   
    public static class DebugFunctions
    {
        
        private class DebugSettings
        {
            public static readonly bool LogVideoList;

            static DebugSettings()
            {
                LogVideoList = false;
            }
        }

        public static void DEBUG_output_list_of_videos(string label, ButtonSchema button)
        {
            if (!DebugSettings.LogVideoList)
            {
                return;
            }
            string videos_to_play = "";
            bool isFirst = true;
            foreach (string videofile in button.VideoFilename)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    videos_to_play += " => ";
                }
                string filename = videofile.Substring(videofile.LastIndexOf('\\') + 1);
                videos_to_play += filename;
            }
            Debug.WriteLine(videos_to_play, label);
        }

        public static int CRASH_PROGRAM()
        {
            int num1 = 0;
            int num2 = 0;
            return Fraction(num1, num2);
        }

        private static int Fraction(int num1, int num2)
        {
            return num1 / num2;
        }
    }
}
