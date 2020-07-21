# Chicken documentation

## Repo overview

The repo top level contains four directories: Chicken, ChickenMockup, docs and Text. These are explained below.
- Chicken: a Unity project directory for the real version of the chicken game.
- ChickenMockup: a Unity project directory for an early mockup of the chicken game.
- docs: a directory containing the WebGL build of the chicken game, enabling the game to be hosted on github pages for the KTH Github Enterprise.
- Text: a directory containing miscellaneous text files and other files.

Development happens in the Chicken directory, and builds are placed in the docs directory for online hosting.

## Chicken Unity project

The Chicken Unity project is rather small, but requires some explanation. It runs on Unity version 2019.4.1f1 but any Unity 2019.4 (LTS) version should work.

### General flow

The general flow of the project is as follows. The participant accepts the HIT on Amazon mechanical turk and clicks the so-called survey link. This takes them to the website where the game is hosted.

The game starts in the IntroScene, where participants read instructions and give consent to partake. Participants also select their avatar's gender expression and skin color. The main script governing this scene is the IntroManager.

Then, the game goes onto the trial scene. Participants are told what type of trial it is on the loading screen, then the trial starts, where the participant faces a robot and can decide to swerve. After the agents have passed each other, the trial is over. The scene is then reloaded with new trial settings. The main script governing this scene is the TrialManager.

After all the trials are complete, the participant is taken to the ending scene. Here, the participant enters their demographic information for the experiment in three steps, are allowed to enter additional comments and report technical issues and then are given the mturk completion code in a dialog box. The participant returns to the mturk page and enters the completion code to complete the HIT.

### Project file structure

All the different directories in the project file structure is explained here. The directories are:
- Fonts
- Materials
- MesoGames
- Models
- Plugins
- Prefabs
- Scenes
- Scripts
- Textures
- WebGLTemplates

#### Fonts

Directory containing fonts for use in the project. Only the bahnschrift font and the Arial font are used in the project. Arial is imported into Unity by default, while the bahnschrift font was taken from the Windows system fonts and imported into Untiy.

#### Materials

Directory containing subdirectories for materials for different uses.

The PlayerMaterials directory contains materials for different skin colors for player avatars. The materials are modified versions of materials from the player avatar package.

The PR2Materials directory contains materials that were imported along with the PR2 robot model.

The RobotMaterials directory contains materials used to make robot models look distinct in the trials. These are the general materials that do not depend on UV coordinates to work, and can be applied to any part of a robot's model.

The SophiaMaterials directory contains materials imported along with the Sophia robot model, as well as materials used to make the Sophia robot model look distinct in the trials. These materials are specialized for the Sophia model, corresponding to the model's UV coordinates.

#### MesoGames

Directory created when importing the customizable player models. Contains everything that the models need, with animations, materials and everything. This directory should not need to be accessed now after the player avatar prefabs and materials have been set up.

#### Models

Directory containing imported .fbx files.

#### Plugins

Directory containing a single .jslib file that contains the functions to interact with the website where the game is hosted. Contains a single function for displaying a string (the mturk completion code) in a dialog box.

The directory is required by Unity, and has a specific structure that Unity recognizes and can work with. See [the documentation](https://docs.unity3d.com/Manual/webgl-interactingwithbrowserscripting.html) for more information.

#### Prefabs

Directory containing prefab files that are derived from game objects. Some are abstract objects containing scripts but no rendering components, others are environment models created from Unity primitives and some are created outside of Unity, imported into Unity and modified with transforms, scripts and materials to make a prefab.

Prefabs are used as premade objects. They can be imported into a scene directly, or used by a script and loaded at runtime.

#### Scenes

Directory containing all of the three scenes of the game.

#### Scripts

Directory containing all of the scripts of the game. An overview of each script is given below, and each script contains code comments.

- Appearance: Defines a data structure to hold all the data about a player avatar's appearance.
- AssetLoader: Contains functions for using the AssetSelector script's functions for loading player avatar assets, robot model assets and environment model assets into the scene at runtime. Also applies materials for different colors to player avatars and robot models.
- AssetSelector: Takes a lot of materials and prefabs that are drag-and-dropped into it in the inspector and provides functions that make an interface for the prefabs. Mostly provides access to prefabs through enumerators.
- CameraController: Simple script for ensuring that the player camera is in the correct position in every frame during a trial.
- Comments: Defines a data structure to hold information about comments that the participant enters.
- EndingManager: Driving script for the ending scene. Defines behavior for the buttons that require scripting in the scene, and validates input where required. Contacts the ExperimentDataManager script to send participant data and to initiate the sending of experiment data.
- EnvironmentUniquenessChecker: Simple script for ensuring that at most one environment model is loaded at any given time during a trial. This was a necessity to fix a bug where multiple environments were loaded.
- ExperimentDataManager: Acts as a repository for all the experiment data that is acquired during the game. Contains a list of Trials, an Appearance, a Participant, a Comments and a string for the mturk completion code. Calls the EndingManager to display the dialog box. Defines a function for sending the experiment data to the Google Forms form.
- HelperFunctions: Contains static helper functions that can be accessed from anywhere but do not make sense to be put in any particular script.
- IntroManager: Driving script for the intro scene. Defines behavior for the buttons that require scripting in the scene, and validates input where required. Contacts the ExperimentDataManager script to send appearance data.
- OverlayManager: Defines the behavior of the loading screens during the trial scene. Manages the onLoadIsFinished delegate by setting the flags in the SharedVariableManager.
- Participant: Defines a data structure to hold all the data about a participant.
- PlayerController: Takes player input and moves the player avatar. Also defines when the game is over. Relies on the onLoadIsFinished delegate.
- RobotController: Moves the robot agent, decides when to swerve. Relies on the onLoadIsFinished delegate.
- SharedVariableManager: Holds variables, including flags, that multiple classes need access to and that do not make sense being part of any one of the accessing classes. Defines the UnityAction delegates onGameIsOver and onLoadIsFinished that synchronizes the behavior of other scripts in the trial scene.
- Trial: Defines a data structure to hold information about a trial.
- TrialManager: Driving script for the trial scene. Contains the definition of the trials that will be run during the trial scene loop. Calls AssetLoader to load assets into the trial scene. Relies on the onGameIsOver delegate to know when to collect the trial data and then reload the scene but with the next trial. Contacts the ExperimentDataManager to deliver the trial data after all the trials are complete. Handles cleanup of game objects that are not needed after the trial scene.

#### Textures

Directory containing imported texture files that certain materials use.

#### WebGLTemplates

Directory containing subdirectories for WebGL templates for different uses. A WebGL template is the containing html document that loads the game into the browser when running the WebGL build of the game. The templates are as follows:

The BetterTemplate directory contains the template that is primarily used when building the game. The template was created by Github user greggman int [this repo](https://github.com/greggman/better-unity-webgl-template). It defines a way of making the WebGL container take up the entire browser window, rather than being contained in a static container. Also contains code to send browser data to the unity instance.

The CleanWithAddedJS directory contains a template similar to the default template, but without the footer in the container that is usually there. Also contains code to send browser data to the unity instance.

The directory is required by Unity, and has a specific structure that Unity recognizes and can work with. See [the documentation](https://docs.unity3d.com/Manual/webgl-interactingwithbrowserscripting.html) for more information.

### Building WebGL and publishing

In order to build the game, simply go into the build settings in Unity, ensure the platform is WebGL, click Build and select any location. Publish the build by replacing the contents of the docs directory by the contents of the directory created during the building process, then adding the files in git, commit and push.

## Experiment data

Experiment data is all the data that is collected during the game, and used for the purposes of the experiment. The ExperimentDataManager script manages all of the data, by collecting the pieces of data from different places in the game. The script also constructs a form and sends it to a Google Forms form.

The different categories of data are as follows:
- Amazon mechanical turk completion code: Random string of numbers and capital letters created once a participant has finished the trials and entered the participant data.
- Browser data: Collected by a short javascript script in the index.html file in the docs directory and sent into the Unity instance using a SetInterval to ensure that the data is collected correctly. Consists of the name of the browser used by the participant, as well as device width, height, pixel ratio and depth and color depth. When send it is formatted as a string with each piece of data separated by a string token of the form "SPLIT". ExperimentDataManager collects the string and splits into the separate pieces of data.
- Appearance data: The player avatar's appearance data, consisting of the skin color and gender expression of the avatar. Entered by the participant in the intro scene.
- Participant data: Demographic information about the participant, entered by the participant in the ending scene.
- Comments: Additional comments and comments about technical issues entered by the player in the ending scene.
- Trial data: All the data collected during the trial scene. At the end of each trial, that trial's data is collected by the TrialManager script. At the end of the trial scene, the TrialManager sends the complete trial data to the ExperimentDataManager.

The Google Forms form is constructed beforehand, with one free-text question with no input validation created for each piece of data to be collected. After the form is created, it is previewed by clicking the preview button in the Google Forms interface. Viewing the page source and searching for "form action" gives the url that Unity should send its form data to using the POST method.

The field names in the form are tedious to enter into Unity. By viewing the page source of the previewd form in the inspector and searching for "entry." gives a long list of entry names. Each question in the form has one name associated with it on the form "entry.XXXXXXX" where the Xs is some string of numbers. For question number i in the form, the ith entry name is the one associated with the question. In Unity, the form data is constructed in the same order, in order to correspond to the Google Forms form.

## Amazon mechanical turk

The game is connected to Amazon mechanical turk through a HIT. The HIT is created from the "survey link" template provided by Amazon. The HIT simply provides a hyperlink for workers to click on, that leads to the website where the game is hosted. In order to complete the HIT, the worker must enter a survey completion code. This code is in turn generated in the game and provided to the worker at the very end of the game in a dialog box that the worker can copy the code from and paste it into the HIT.

To validate that the worker has truthfully completed the HIT and not made up some survey completion code, cross-validate that the completion code entered in mturk (available in the HIT results) is present in the responses to the Google Forms form.

Data is only sent to the form and entered into the spreadsheet if the game website was referred by mturk. This is checked by the index.html file sending the site referrer to the Unity instance, and the ExperimentDataManager script making sure the referrer contains the string "mturk". This is not foolproof in any way, but it is secure enough for the purposes of the experiment, while also staying general enough to withstand potential changes that Amazon does to the mturk service.
