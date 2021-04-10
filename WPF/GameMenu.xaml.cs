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

        private readonly Storyboard _fadeInStoryboards;
        private readonly Storyboard _fadeOutStoryboards;

        private ButtonData _prevButtonData;

        private bool _hasEndingVideo = false;

     
        public GameMenu(string path, int storyPosition, ButtonData buttonData)
        {

            InitializeComponent();
            string videoPath = buttonData.VideoFileLocation;

            // path for the current branch
            _branchPath = Path.Combine(path, MainResources.GetBranch() + storyPosition.ToString());

            // if we are at -1 story position or no HP, it's game over
            if (storyPosition == -1 || isPlayerDead())
            {
                ShowLoseScreen(buttonData);
                return;
            }

            // callback to invoke after the video is played
            Action afterVideoPlayed = AfterChoiceVideoPlayed;

            // win conditions
            // 1. if there are no more folders along this story path
            // 2. if the button data has an ending
            if (!Directory.Exists(_branchPath) || buttonData.Endings.Count > 0)
            {
                // start by setting the ending video we will play
                string endVideoPath = videoPath;

                // determine which ending based on points
                foreach (Ending ending in buttonData.Endings)
                {
                    // the first ending that matches the condition will be set as the ending (so consider the order of endings in the list)
                    if (MainResources.GetPoints() >= ending.WhenPointsAreBetween[0] && MainResources.GetPoints() <= ending.WhenPointsAreBetween[1])
                    {
                        _hasEndingVideo = true;

                        // set this ending to play after the choice video
                        endVideoPath = Path.Combine(path, "endings", ending.VideoFilename);
                        // break on first match
                        break;
                    }
                }

                if (!_hasEndingVideo && buttonData.Endings.Count == 1)
                {
                    // only 1 ending, play it (no point condition)
                    _hasEndingVideo = true;
                    endVideoPath = Path.Combine(path,"endings", buttonData.Endings[0].VideoFilename);
                }
          

                afterVideoPlayed = () => ShowWinScreen(buttonData, endVideoPath);
                // "just in case" there are  no matches
                if (!_hasEndingVideo || buttonData.Endings.Count == 0)
                {
                    // just show the regular win screen (no ending video)
                    ShowWinScreen(buttonData, videoPath);
                    return;
                }
            }

            _fadeInStoryboards = new Storyboard();
            _fadeOutStoryboards = new Storyboard();

            // setting fade in animations (not running them yet)
            var fadeIn = FindResource("FadeInStoryboard") as Storyboard;
            Storyboard.SetTarget(fadeIn, GameMenuGrid);

            var fadeIn2 = FindResource("FadeInStoryboard2") as Storyboard;
            Storyboard.SetTarget(fadeIn2, ChoiceMenuBorder);

            var fadeOut = FindResource("FadeOutStoryboard") as Storyboard;
            // sets the callback when storyboard (choices) have faded out
            fadeOut.Completed += NextVideoStart;
            Storyboard.SetTarget(fadeOut, GameMenuGrid);

            var fadeOut2 = FindResource("FadeOutStoryboard2") as Storyboard;
            Storyboard.SetTarget(fadeOut2, ChoiceMenuBorder);

           
            _fadeInStoryboards.Children.Add(fadeIn);
            _fadeInStoryboards.Children.Add(fadeIn2);

            _fadeOutStoryboards.Children.Add(fadeOut);
            _fadeOutStoryboards.Children.Add(fadeOut2);

            _scenarioPath = path;
            _storyPosition = storyPosition;


            if (!_hasEndingVideo)
            {
                // if we are not playing the ending video, we must be continuing the game, so add the choice buttons
                AddButtons();
            }

            // now play the video, and invoke the callback after it's played
            MainResources.MainWindow.PlayFile(videoPath, afterVideoPlayed);
        }

        private bool isPlayerDead()
        {
            return MainResources.GetHP() < 0;
        }

        private void ShowLoseScreen(ButtonData buttonD)
        {
            // lost the game, show LoseScreen
            var lost = new LoseScreen(buttonD);
            MainResources.MainWindow.MainPanel.Children.Add(lost);
            MainResources.MainWindow.MainPanel.Children.Remove(this);
        }

        private void ShowWinScreen(ButtonData buttonD, string videoFilepath)
        {
            // no ending video specified, so just display the win screen after the video played
            WinScreen win = new WinScreen(buttonD, videoFilepath);
            MainResources.MainWindow.MainPanel.Children.Add(win);
            MainResources.MainWindow.MainPanel.Children.Remove(this);
        }



        private void AddButtons()
        {
            List<GameButton> gameButtons = new List<GameButton>();

            // load the buttons from json file
            string branchPath = Path.Combine(_branchPath, "options.json");
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

                    // set default score adjustments
                    ScoreAdjustment scoreAdjustment = new ScoreAdjustment
                    {
                        HP = 0,
                        Points = 0
                    };

                    // default empty ending list
                    List<Ending> endings = new List<Ending>();

                    // the video path is the CURRENT BRANCH PATH, even if the branch will be changed
                    string videoPath = Path.Combine(_scenarioPath, storyPath.Branch + storyPath.StartPosition, item.VideoFilename);

                    // if item.Path is null, we stay on the same branch and advance 1 position (default behavior)
                    if (item.Path != null)
                    {
                        // if we change the path, we set the branch and the start position on that branch
                        storyPath.Branch = item.Path.Branch;
                        storyPath.StartPosition = item.Path.StartPosition;
                    }

                    if (item.ScoreAdjustment != null)
                    {
                        scoreAdjustment.HP = item.ScoreAdjustment.HP;
                        scoreAdjustment.Points = item.ScoreAdjustment.Points;
                    }

                    if (item.Endings != null)
                    {
                        endings = item.Endings;
                    }

                    // wrap the data in a class we can pass easier
                    ButtonData buttonData = new ButtonData()
                    {
                        ButtonType = item.ButtonType,
                        VideoFileLocation = videoPath,
                        StoryPath = storyPath,
                        ScoreAdjustment = scoreAdjustment,
                        Endings = endings,
                        EndScreenMessage = item.EndScreenMessage
                    };
                    GameButton gameButton = new GameButton(item.Label, buttonData, ButtonClicked);
                    gameButtons.Add(gameButton);
      
                }
            }

            gameButtons.Shuffle();

            // making 2 columns with multiple rows for the buttons
            int buttonPos = 0;
            foreach (var gameButton in gameButtons)
            {
                if(buttonPos == 0 || buttonPos == 2 || buttonPos == 4)
                {
                    ButtonStackPanel.Children.Add(gameButton);
                 
                }
                else
                {
                    ButtonStackPanel2.Children.Add(gameButton);
                }
                buttonPos += 1;
            }

            // Debug logging
            // display the current HP, points, branch and position on the choice menu 
            HealthAndPositionLabel.Content = $"{MainResources.GetBranch()}{_storyPosition} | HP:{MainResources.GetHP()} Pts:{MainResources.GetPoints()}";
        }

        private void AfterChoiceVideoPlayed()
        {
            _fadeInStoryboards.Begin();
            IsEnabled = true;
        }

        private void ButtonClicked(ButtonData buttonData)
        {
            IsEnabled = false;

            if(buttonData.StoryPath.Branch != MainResources.GetBranch())
            {
                MainResources.SetBranch(buttonData.StoryPath.Branch);
                MainResources.SetPathPosition(buttonData.StoryPath.StartPosition - 1);
            }

            _prevButtonData = buttonData;
            _fadeInStoryboards.Stop();
            _fadeOutStoryboards.Begin();
        }

        private void NextVideoStart(object sender, EventArgs e)
        {
            // by default we advance the position by 1
            int nextStoryPosition = MainResources.GetPathPosition() + 1;

            switch (_prevButtonData.ButtonType)
            {
                case ButtonType.Default:
                    MainResources.AdjustHP(_prevButtonData.ScoreAdjustment.HP);
                    MainResources.AdjustPoints(_prevButtonData.ScoreAdjustment.Points);
                    break;
                case ButtonType.End:
                    // an "End" button type will end the game immidiately
                    nextStoryPosition = -1;
                    break;
                default:
                    break;
            }

            GameMenu nextGameMenu = new GameMenu(_scenarioPath, nextStoryPosition, _prevButtonData);
            MainResources.MainWindow.MainPanel.Children.Add(nextGameMenu);
            MainResources.MainWindow.MainPanel.Children.Remove(this);
            MainResources.SetPathPosition(nextStoryPosition);
        }
    }
}