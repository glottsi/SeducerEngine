# SeducerEngine example project

I forked this from [https://github.com/WWRS/SeducerEngine](https://github.com/WWRS/SeducerEngine) which is a good base project, but the creator made no proper readme or explanation how to use it, and hasn't been updated in 2 years. So I'm attempting to provide more of an explanation to how it works, and what is required to add more Scenarios and choices. 

Demonstration of the game when running (assets are included in this repo):
[https://www.youtube.com/watch?v=E3i96P9AGAs](https://www.youtube.com/watch?v=E3i96P9AGAs)

## Change notes
### 03/05/21 
 - buttons and scenarios are now loaded from `.json` files and not the `.txt`. 
 - scenario json contains the Title and Description
 - the button json objects contain 3 attributes:
 
         ButtonType: can be either Win (+1 Point), Lose (-1 Health), or End (instant game over)
         Label: text to display on the button
         VideoFilename: name of the file to be played after this button is chosen
         
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
   |  1/
         | options.json
         | video1_or_whatever_name_you_want.avi
         | video2.avi
         | video3.avi
   |  2/
         | options.json
         | video1.avi
         | video2.avi
         | video_some_other_name.avi
   | etc...
```
`/Scenario1` - the folder containing the required data and assets for the entire Scenario. _you can name this folder whatever you want_

`bg.png` -  _don't change the name_ - the image to display on the Scenario select screen.

`pre.avi` -  _don't change the name_ - the first video file to play for this scenario.

`scenario.json` -  _don't change the name_ - a json file containing the scenario Title and Description.

`1/` -  _name the folders incrementally ie 1,2,3,4_ - folder to contains the choices and reaction videos for each.

`1/options.json` -  _don't change the name_ - the json to build our buttons with, the object in this file is defined like this: 

  ```
 [
   {
    "ButtonType": "Win",
    "Label": "say hi",
    "VideoFilename": "say_hi.mp4"
  },
  {
    "ButtonType": "Lose",
    "Label": "say something dumb",
    "VideoFilename": "loser.avi"
  },
  {
    "ButtonType": "End",
    "Label": "say something so terrible the game ends immidiately",
    "VideoFilename": "gameover.avi"
  }
]
  ```
  **Note:** you can name the video files whatever you want, but make sure its the same as the VideoFilename in the `options.json`

## Considerations

- After playing a Win video, the game advances to the next choice and adds 1 point (we don't do anything with the point system currently).
- After playing a Lose video, the game advances to the next choice, but reduces the Health by 1 point.
- After an End video, the game displays the Game Over screen.
- If Health reaches 0, game is over.
- You win the game when there are no more numbered folders to progress to. If there are only 3 folders, after the 3rd choice is a W, the game displays the WinScreen. 
- I added my own LoseScreen, because there wasn't one. The number of the losing condition is `-1`

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.
