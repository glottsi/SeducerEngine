using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace WPF
{
    public partial class GameMenu : UserControl
    {
        private readonly string _scenarioPath;
        private readonly string _branchPath;
        private readonly int _storyPosition;

        private readonly Storyboard _fadeIn;
        private readonly Storyboard _fadeOut;

        private ButtonType _prevButtonType;
        private string _prevVideoPath;

     
        public GameMenu(string path, int storyPosition, string videoPath)
        {
            // path for the current branch
            _branchPath = Path.Combine(path, MainResources.GetBranch() + storyPosition.ToString());

            // if we are at -1 story position or no life points, it's game over
            bool gameOver = MainResources.GetLifePoints() < 0;
            if (storyPosition == -1 || gameOver)
            {
                // lost the game, show LoseScreen
                var lost = new LoseScreen(videoPath);
                MainResources.MainWindow.MainPanel.Children.Add(lost);
                MainResources.MainWindow.MainPanel.Children.Remove(this);
                return;
            }

            // if there are no more directories for the scenario, consider it a win
            if (!Directory.Exists(_branchPath))
            {
                // show win screen
                MainResources.MainWindow.MainPanel.Children.Add(new WinScreen(videoPath));
                MainResources.MainWindow.MainPanel.Children.Remove(this);
                return;
            }

            InitializeComponent();

            _fadeIn = FindResource("FadeInStoryboard") as Storyboard;
           // Debug.Assert(_fadeIn != null, nameof(_fadeIn) + " != null");
            Storyboard.SetTarget(_fadeIn, GameMenuGrid);

            _fadeOut = FindResource("FadeOutStoryboard") as Storyboard;
          //  Debug.Assert(_fadeOut != null, nameof(_fadeOut) + " != null");
            _fadeOut.Completed += NextVideoStart;
            Storyboard.SetTarget(_fadeOut, GameMenuGrid);

            _scenarioPath = path;
            _storyPosition = storyPosition;

            MainResources.MainWindow.PlayFile(videoPath, PrevVideoDone);
       
            AddButtons();
        }


        private void AddButtons()
        {
            List<GameButton> gameButtons = new List<GameButton>();
          
            string branchPath = Path.Combine(_branchPath, "options.json");
            // load the buttons from json file
            using (StreamReader r = new StreamReader(branchPath))
            {
                string json = r.ReadToEnd();
                List<ButtonSchema> items = JsonConvert.DeserializeObject<List<ButtonSchema>>(json);
                foreach (var item in items)
                {
                    // set default path progression
                    StoryPath storyPath = new StoryPath
                    {
                        Branch = MainResources.GetBranch(),
                        StartPosition = _storyPosition
                    };
                    // the video path is the CURRENT BRANCH PATH, even if the branch will be changed
                    string videoPath = Path.Combine(_scenarioPath, storyPath.Branch + storyPath.StartPosition, item.VideoFilename);

                    // if item.Path is null, we stay on the same branch and advance 1 position (default behavior)
                    if (item.Path != null)
                    {
                        // if we change the path, we set the branch and the start position on that branch
                        storyPath.Branch = item.Path.Branch;
                        storyPath.StartPosition = item.Path.StartPosition;
                    }
                
                    GameButton gameButton = new GameButton(item.Label, item.ButtonType, storyPath, videoPath, ButtonClicked);
                    gameButtons.Add(gameButton);
      
                }
            }
            // set the label shown at the top of the choices (to be made dynamic in a future update)
            ChoiceLabel.Content = "Make a choice:";
            // display the current HP, branch and position (for debugging)
            HealthAndPositionLabel.Content = "Path: " + MainResources.GetBranch() + _storyPosition+ "\nHP: " + MainResources.GetLifePoints();

            gameButtons.Shuffle();
            foreach (var gameButton in gameButtons)
                ButtonStackPanel.Children.Add(gameButton);
        }

        private void PrevVideoDone()
        {
            _fadeIn.Begin();
            IsEnabled = true;
        }

        private void ButtonClicked(ButtonType buttonType, StoryPath nextPath, string videoPath)
        {
            IsEnabled = false;

            _prevVideoPath = videoPath;
            _prevButtonType = buttonType;

            if(nextPath.Branch != MainResources.GetBranch())
            {
                MainResources.SetBranch(nextPath.Branch);
                MainResources.SetPathPosition(nextPath.StartPosition - 1);
            }
          
            _fadeIn.Stop();
            _fadeOut.Begin();
        }

        private void NextVideoStart(object sender, EventArgs e)
        {
            GameMenu nextGameMenu;
            // by default we advance the position by 1
            int nextStoryPosition = MainResources.GetPathPosition() + 1;

            switch (_prevButtonType)
            {
                case ButtonType.Win:
                    // adds 1 score if it was a "Win" choice
                    MainResources.Scores.Add(true);
                    break;
                case ButtonType.Lose:
                    // a "Lose" button type will reduce the life points
                    MainResources.ReduceLifePoints(1);
                    break;
                case ButtonType.End:
                    // an "End" button type will end the game immidiately
                    nextStoryPosition = -1;
                    break;
                default:
                    break;
            }

            nextGameMenu = new GameMenu(_scenarioPath, nextStoryPosition, _prevVideoPath);
            MainResources.MainWindow.MainPanel.Children.Add(nextGameMenu);
            MainResources.MainWindow.MainPanel.Children.Remove(this);
            MainResources.SetPathPosition(nextStoryPosition);
        }
    }
}