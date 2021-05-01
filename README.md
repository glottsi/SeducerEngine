# SeducerEngine example project

I forked this from [https://github.com/WWRS/SeducerEngine](https://github.com/WWRS/SeducerEngine) which is a good base project, but the creator made no proper readme or explanation how to use it, and hasn't been updated in 2 years. So I'm attempting to provide more of an explanation to how it works, and what is required to add more Scenarios and choices. 


## Change notes

### 05/01/2021
  - fixed bug where player could click the hidden buttons while the video was being played
  - choice videos can now be played conditionally based on player's current score
  - if the application crashes, a file will be created called `CRASH_REPORT.txt` with detailed information such as the stack trace, and a dialog will be shown to the user 
  - on LoseScreen, player has the option to retry from the last choice

### 04/25/21
  - all 'VideoFilename' attributes on the ButtonSchema (not Endings) is now an array of filenames to play, instead of a single string
  - ButtonData was removed and replaced with ButtonSchema (they were basically the same anyways)
  - added an 'Image' and 'IntroVideo' to the Scenario object, so we can define the filenames and types of the video to play and the image to use to represent the scenario directly in the .json file
  - you can now use EndScreenMessage for a conditional ending 

### 04/10/21
  - buttons can have an `EndScreenMessage` that will display on the ending screen if that option ends the game. If no message is specified (or if it is null), it will use a default message for that screen.
  - added a game settings screen (no settings yet)
  - added a 'Press any key' screen that is displayed before the main menu
  - added BGM to the main menus and intros (until the actual scenario video is started)
  - made the dev logos screen show a full-page image instead of small logo and text

### 04/02/21
  - changed button styling for the main menu, scenario selection, and choice buttons with an animated gradient effect
  - removed Win and Lose button types, replaced them with Default. The only other button type is End will instantly lose the game (send to game over screen) after playing the video for the choice
  - added a ScoreAdjustment object to the button schema so we can set any adjustments to HP or Points on button click
  - added Endings array to the button schema, outlining the possible endings and the points required to view them
  - ending system will play an ending video after playing the last choice video (and determining which video to play based off the score)
  - made a new class ButtonData to wrap parameters to pass through to buttons, callbacks, etc
  - added a top and bottom border to the choice button menu (like in super seducer screenshot)
  - added a settings object to the scenario.json were we define the starting values for things such as HP, path, etc. 
  - moved ButtonType into Models.cs and deleted the ButtonType.cs file

### 03/21/21
 - branching system implemented
 - default branch is A, but other branches can be named whatever you want
 - HP and current path are displayed at the bottom of the select choice screen to help with debugging
 - removed Scenario2 and Scenerio3 assets, and replaced the image for Scenario1
 - remove youtube link (will make a new video later demonstrating this new version)
 - button json object has changed

### 03/05/21 
 - buttons and scenarios are now loaded from `.json` files and not the `.txt`. 
 - scenario json contains the Title and Description
 - the button json objects contain 3 attributes
 - we can now have more than 1 button for each type (for example we can have 2 Lose buttons that will play different videos each)

## Preparation

You need Visual Studio 2019 to build the project.

Also if you don't have it already, you need to download the `.NET Framework 4.7.1` SDK from here: [https://dotnet.microsoft.com/download/visual-studio-sdks](https://dotnet.microsoft.com/download/visual-studio-sdks), be sure to download the 4.7.1 "Developer Pack".  

## Building and Running

Open the `WPF.csproj` file contained in the `WPF` folder. This should automatically open Visual Studio and load the project. 

Build the project by pressing F5, or clicking "Start" button at the top. 

## Engine Documentation

All Scenarios are contained within their own folders in the `Assets/Scenarios`. Every Scenario folder needs to be setup like this:

```
/Scenario1
   |  bg.png
   |  pre.avi
   |  scenario.json
   |  endings/
        | default_ending.avi
        | bad_ending.avi
        | good_ending.avi
   |  A1/
         | options.json
         | video1_or_whatever_name_you_want.avi
         | video2.avi
         | video3.avi
   |  A2/
         | options.json
         | video1.avi
         | video2.avi
         | video_some_other_name.avi
   |  B1/
         | options.json
         | video1.avi
         | video2.avi
         | video_some_other_name.avi
   | etc...
```
`/Scenario1` - the folder containing the required data and assets for the entire Scenario. _you can name this folder whatever you want_

`bg.png` - the image to display on the Scenario select screen.

`pre.avi` - the first video file to play for this scenario.

`scenario.json` -  _don't change the name_ - a json file containing the scenario Title and Description, as well as the starting Settings for things such as HP and Points.

```
// scenario.json
{
  "Title": "Episode 1",
  "Subtitle": "subtitle for the scenario"
  "Description": "Some text to describe the scenario...",
  "Image": "bg.png",
  "IntroVideo": "pre.avi"
  "Settings": {
    "StartingHP": 3,
    "StartingPoints": 0,
    "StartingBranch": "A",
    "StartingPathPosition": 1
  }
}

```

`A1/` -  _name the folders incrementally ie A1, A2, A3, B1, B2 ..._ - folder to contains the choices and reaction videos for each. the default branch is A, the game start on A branch at position 1 (A1)

`A1/options.json` -  _don't change the name_ - the json to build our buttons with, the object in this file is defined like this: 

  ```
 [
   // Default button (advance 1 in the storypath, no point adjustment)
   {
    "Label": "choice 1",
    "VideoFilename": [ "choice1.mp4", "anothervideo.mp4" ]
  },

  // End game instantly (immidiately show the Lose screen after playing the choice video)
  {
    "ButtonType": "End",
    "EndScreenMessage": "GG"
    "Label": "choice 2",
    "VideoFilename": [ "choice2.avi" ]
  },

  // adjust points when button selected
  {
    "Label": "choice 1",
    "ScoreAdjustment": {
      "HP": 0,
      "Points": 1
    },
    "VideoFilename": [ "choice1.mp4" ]
  }
  
  // play a choice video based on player's current points (can be combined with conditional endings)
  {
    "Label": "play conditional choice video",
    "Videos": [
      {
        "WhenPointsAreBetween": [ 0, 3 ],
        "VideoFilename": [ "A1choice1.avi", "A1choice2.avi" ]
      },
      {
        "WhenPointsAreBetween": [ 3, 6 ],
        "VideoFilename": [ "A1choice2.avi", "A1choice1.avi" ]
      }
    ]
  },

  // decide an ending based on the points (play video, then play ending video)
  {
    "Label": "choice 1",
     "Endings": [
      {
        "WhenPointsAreBetween": [ 0, 3 ],
        "EndScreenMessage": "game over",
        "VideoFilename": "default_ending.mp4"
      },
      {
        "WhenPointsAreBetween": [ 3, 8 ],
        "VideoFilename": "normal_ending.avi"
      },
      {
        "WhenPointsAreBetween": [ 8, 10 ],
        "VideoFilename": "great_ending.avi"
      }
    ]
    "VideoFilename": [ "choice1.mp4" ]
  },

  // change path to another Branch 
  {
    "Path": {
      "Branch": "B",
      "StartPosition": "1"
    },
    "Label": "jump to branch B1",
    "VideoFilename": [ "choice_d.avi" ]
  }
]


  ```

## Considerations

- If Health reaches 0, or if the `End` button type is clicked, game is instantly over and player is shown the LoseScreen.
- on a LoseScreen, player can choose to retry, and it will play the last video and display the previous choices
- You win the game when there are no more numbered folders to progress to, or if an Ending has been defined in the button's data. 
- On a win, if there is no ending video defined, the game will play the choice video and then show the Win screen.
- On a win, if there is only 1 Ending defined, it will play it regardless of the point value.
- On a win, if there are multiple Endings defined, the player's Point value is used to determine the ending video to play.
- You can combine conditional choice videos and conditional endings to make unique story endings paths

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.
