using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace WPF
{
    /* 
     GameMenu handles most game logic such as 
        - loading videos based on current storypath position
        - determining ending video to be played on game over
        - preparing callbacks to invoke after the video is played
        - building the buttons from our JSON schema
        - fading in/out the button choices and accepting the user's selection

    A new GameMenu instance is created every gameplay loop (video, choice, reaction video)

     */
    public partial class GameMenu : UserControl
    {
        private readonly string _scenarioPath;
        private readonly string _branchPath;
        private readonly int _storyPosition;

        private readonly Storyboard _fadeInStoryboards;
        private readonly Storyboard _fadeOutStoryboards;

        private ButtonSchema _prevButtonSchema;

        private bool _hasEndingVideo = false;

        private int _currentlyPlayingVideo = 0;

        private List<GameButton> gameButtons;

        public GameMenu(ButtonSchema buttonData)
        {
            InitializeComponent();
            // load root directory
            _scenarioPath = MainResources.GetRootDirectory();

            // holds our buttons so we can iterate and enable/disable
            gameButtons = new List<GameButton>();

            // load current path position (ie 1, 2, 3, -1)
            _storyPosition = MainResources.GetPathPosition();

            // set default video to play (first video in list)
            string videoPath = buttonData.VideoFilename[_currentlyPlayingVideo];

            // path for the current branch
            _branchPath = Path.Combine(_scenarioPath, MainResources.GetBranch() + _storyPosition.ToString());

            // if we are at -1 story position or no HP, it's game over
            if (_storyPosition == -1 || isPlayerDead())
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
                        endVideoPath = Path.Combine(_scenarioPath, "endings", ending.VideoFilename);

                        // set the text to display on the ending screen depending on this ending
                        buttonData.EndScreenMessage = ending.EndScreenMessage;
                        // break on first match
                        break;
                    }
                }

                if (!_hasEndingVideo && buttonData.Endings.Count == 1)
                {
                    // only 1 ending, play it (no point condition)
                    _hasEndingVideo = true;
                    endVideoPath = Path.Combine(_scenarioPath, "endings", buttonData.Endings[0].VideoFilename);
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
           
            // see if we have other videos to play after this one
            if (buttonData.VideoFilename.Count > 1)
            {
               
                // set the callback to invoke after ALL videos are played
                Action finalCallback = afterVideoPlayed;
                // set the immidiate callback to play other videos first
                afterVideoPlayed = ()=>PlayOtherVideosFirst(buttonData.VideoFilename, finalCallback);
           
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
       
            if (!_hasEndingVideo)
            {
                // if we are not playing the ending video, we must be continuing the game, so add the choice buttons
                AddButtons();
            }

            // now play the video, and invoke the callback after it's played
            MainResources.MainWindow.PlayFile(videoPath, afterVideoPlayed);
        }

        private void PlayOtherVideosFirst(List<string> videosToPlay, Action finalCallback) 
        {
            // ignore first video in the list (its already been played by the time we get here)
            _currentlyPlayingVideo += 1;
            if(_currentlyPlayingVideo == videosToPlay.Count)
            {
               
                // all the videos are done, so we can invoke the final, intended callback
                finalCallback.Invoke();
            } else
            {
             
                // set the callback to this function to play other videos first
                Action nextVideo = () => PlayOtherVideosFirst(videosToPlay, finalCallback);
            
                // play the video and re-run this function
                MainResources.MainWindow.PlayFile(videosToPlay[_currentlyPlayingVideo], nextVideo);
            }
            
        }

        private bool isPlayerDead()
        {
            return MainResources.GetHP() < 0;
        }

        private void ShowLoseScreen(ButtonSchema buttonD)
        {
            // lost the game, show LoseScreen
            var lost = new LoseScreen(buttonD);
            MainResources.MainWindow.MainPanel.Children.Add(lost);
            MainResources.MainWindow.MainPanel.Children.Remove(this);
        }

        private void ShowWinScreen(ButtonSchema buttonD, string videoFilepath)
        {
            // no ending video specified, so just display the win screen after the video played
            WinScreen win = new WinScreen(buttonD, videoFilepath);
            MainResources.MainWindow.MainPanel.Children.Add(win);
            MainResources.MainWindow.MainPanel.Children.Remove(this);
        }

        private void AddButtons()
        {
            gameButtons = new List<GameButton>();

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
                    List<string> videoPaths = new List<string>() { };
                    int vidIndx = 0;
                    foreach (string vidPath in item.VideoFilename)
                    {
                        videoPaths.Add(Path.Combine(_scenarioPath, storyPath.Branch + storyPath.StartPosition, item.VideoFilename[vidIndx]));
                        vidIndx += 1;
                    }
                  

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

                    ButtonSchema buttonData = new ButtonSchema()
                    {
                        ButtonType = item.ButtonType,
                        VideoFilename = videoPaths,
                        Path = storyPath,
                        ScoreAdjustment = scoreAdjustment,
                        Endings = endings,
                        EndScreenMessage = item.EndScreenMessage
                    };
                    GameButton gameButton = new GameButton(item.Label, buttonData, ButtonClicked);
                    // disable the button by default
                    gameButton.IsEnabled = false;
                    // add the button to our global button container so we can re-enable them after the video is done
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

        // iterates through the list of game buttons, and enables/disables them. 
        private void SetButtonsEnabled(bool enabled)
        {
            foreach (GameButton btn in gameButtons)
            {
                btn.IsEnabled = enabled;
            }
        }

        private void AfterChoiceVideoPlayed()
        {
            // fade in the choice screen
            _fadeInStoryboards.Begin();
            // enable the buttons so they can be clicked
            SetButtonsEnabled(true);
            IsEnabled = true;
        }

        private void ButtonClicked(ButtonSchema ButtonSchema)
        {
            IsEnabled = false;
            
            if(ButtonSchema.Path.Branch != MainResources.GetBranch())
            {
                MainResources.SetBranch(ButtonSchema.Path.Branch);
                MainResources.SetPathPosition(ButtonSchema.Path.StartPosition - 1);
            }

            _prevButtonSchema = ButtonSchema;
            _fadeInStoryboards.Stop();
            _fadeOutStoryboards.Begin();
        }

        private void NextVideoStart(object sender, EventArgs e)
        {
            // by default we advance the position by 1
            int nextStoryPosition = MainResources.GetPathPosition() + 1;

            switch (_prevButtonSchema.ButtonType)
            {
                case ButtonType.Default:
                    MainResources.AdjustHP(_prevButtonSchema.ScoreAdjustment.HP);
                    MainResources.AdjustPoints(_prevButtonSchema.ScoreAdjustment.Points);
                    break;
                case ButtonType.End:
                    // an "End" button type will end the game immidiately
                    nextStoryPosition = -1;
                    break;
                default:
                    break;
            }
            // update the game resources
            MainResources.SetPathPosition(nextStoryPosition);
            // create new GameMenu instance
            GameMenu nextGameMenu = new GameMenu(_prevButtonSchema);
            MainResources.MainWindow.MainPanel.Children.Add(nextGameMenu);
            MainResources.MainWindow.MainPanel.Children.Remove(this);
            MainResources.SetPathPosition(nextStoryPosition);
        }
    }
}