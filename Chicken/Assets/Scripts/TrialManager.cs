using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using static HelperFunctions;

// Class that keeps track of the trials, runs through them in sequence, stores the data
// and then reports the data to the ExperimentDataManager. This is the main controller of
// the trial scene loop.
public class TrialManager : MonoBehaviour {

#pragma warning disable
  [SerializeField]
  private bool debugTrials = false;
  [SerializeField]
  private string AIText;
  [SerializeField]
  private string humanText;
#pragma warning restore

  public List<Trial> trials { get; private set; } // All the trials.

  private AssetLoader assetLoader;
  private ExperimentDataManager experimentDataManager;
  private int currentTrialNum = 0; // Keeps track of which trial is currently being played.
  private OverlayManager overlayManager;
  private SharedVariableManager sharedVariableManager;
  private Text robotViewText;

  private static TrialManager instance;

  // Handle trial manager instancing between scene loads.
  void Awake() {
    // If there is no instance, let this be the new instance, otherwise, destroy this object.
    if (instance == null) {
      instance = this;
    } else {
      Destroy(gameObject);
      return;
    }

    // If this object was set as the instance, make sure it is not destroyed on scene loads.
    DontDestroyOnLoad(gameObject);
  }

  void OnEnable() {
    // Subscribe trial finishing and further actions to game over event.
    SharedVariableManager.onGameIsOver += FinishTrialAndContinue;
  }

  void OnDisable() {
    // Mandatory unsubscriptions.
    SceneManager.sceneLoaded -= NewSceneActions;
    SharedVariableManager.onGameIsOver -= FinishTrialAndContinue;
  }

  void Start() {
    // Unless the start function initializations are timed correctly, there is a
    // null reference exception in builds.
    StartCoroutine(TimeActionsWithFirstSceneLoaded());
  }

  IEnumerator TimeActionsWithFirstSceneLoaded() {
    // Make sure that this script is executed after all the others.
    yield return new WaitForEndOfFrame();

    // Find all the objects required by this class.
    assetLoader = GameObject.Find("AssetLoader").GetComponent<AssetLoader>();
    experimentDataManager = GameObject.Find("ExperimentDataManager").GetComponent<ExperimentDataManager>();
    overlayManager = GameObject.Find("OverlayCanvas").GetComponent<OverlayManager>();
    sharedVariableManager = GameObject.Find("SharedVariableManager").GetComponent<SharedVariableManager>();
    robotViewText = GameObject.Find("RobotViewText").GetComponent<Text>();

    // Set up list of robot colors.
    List<RobotColor> colors = new List<RobotColor>();
    colors.Add(RobotColor.BLUE);
    colors.Add(RobotColor.RED);
    colors.Add(RobotColor.PURPLE);
    colors.Add(RobotColor.YELLOW);
    colors.Add(RobotColor.PINK);
    colors.Add(RobotColor.GREEN);
    colors.Add(RobotColor.BROWN);
    colors.Add(RobotColor.ORANGE);
    colors.Add(RobotColor.BLACK);
    colors.Add(RobotColor.SPECIAL);
    colors.Shuffle();

    // Initialize trials list as empty.
    trials = new List<Trial>();

    // Trial list construction.
    // Add trials. This is dependent on the experiment design.
    if (debugTrials) {
      // Add the trials.
      // trials.Add(new Trial(EnvironmentType.BASIC, RobotType.PR2, TrialType.AI));
      // trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.HUMAN));
      // trials.Add(new Trial(EnvironmentType.NARROW, RobotType.PR2, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.PR2, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.PR2, TrialType.HUMAN));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.PR2, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.PR2, TrialType.HUMAN));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.PR2, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.PR2, TrialType.HUMAN));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.PR2, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.PR2, TrialType.HUMAN));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.PR2, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.PR2, TrialType.HUMAN));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.HUMAN));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.HUMAN));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.HUMAN));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.HUMAN));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.HUMAN));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.PR2, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.PR2, TrialType.HUMAN));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.PR2, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.PR2, TrialType.HUMAN));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.PR2, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.PR2, TrialType.HUMAN));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.PR2, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.PR2, TrialType.HUMAN));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.PR2, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.PR2, TrialType.HUMAN));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.HUMAN));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.HUMAN));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.HUMAN));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.HUMAN));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.HUMAN));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.PR2, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.PR2, TrialType.HUMAN));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.PR2, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.PR2, TrialType.HUMAN));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.PR2, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.PR2, TrialType.HUMAN));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.PR2, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.PR2, TrialType.HUMAN));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.PR2, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.PR2, TrialType.HUMAN));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.HUMAN));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.HUMAN));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.HUMAN));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.HUMAN));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.HUMAN));
    } else {
      // Add the trials.
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.PR2, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.PR2, TrialType.HUMAN));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.PR2, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.PR2, TrialType.HUMAN));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.PR2, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.PR2, TrialType.HUMAN));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.PR2, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.PR2, TrialType.HUMAN));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.PR2, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.PR2, TrialType.HUMAN));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.HUMAN));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.HUMAN));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.HUMAN));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.HUMAN));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.AI));
      trials.Add(new Trial(EnvironmentType.OPEN, RobotType.SOPHIA, TrialType.HUMAN));
    }

    // Set colors. Colors are assigned randomly in a way, looped around for the number of trials.
    for (int i = 0; i < trials.Count; ++i) {
      trials[i].robotColor = colors[i % colors.Count];
    }

    // Shuffle the trials.
    trials.Shuffle();

    // Add the test trial to the front of the list (should always be first!).
    trials.Insert(0, new Trial());
    // End trial list construction.

    // Load the trial environment.
    assetLoader.LoadEnvironment(trials[currentTrialNum].environmentType);

    // Display the first trial's loading screen.
    overlayManager.DisplayLoadingScreen(trials[currentTrialNum].trialType);

    // Subscribe the new scene actions to future scene loads.
    // Needs to be here in order to not perform NewSceneActions for the first trial.
    SceneManager.sceneLoaded += NewSceneActions;
  }

  // Actions to take to finish a trial after the game is over.
  void FinishTrialAndContinue() {
    PackTrialData();
    NextTrial();
  }

  // Actions to take to start a trial properly after the trial scene is reloaded.
  private void NewSceneActions(Scene scene, LoadSceneMode mode) {
    StartCoroutine(TimeActionsWithSceneLoaded());
  }

  // Reloads the managers and displays the loading screen after scene reloads.
  // Ensures correct timing between scene load and finding of objects.
  IEnumerator TimeActionsWithSceneLoaded() {
    // Time the actions.
    yield return new WaitForEndOfFrame();

    // Update the managers to the ones loaded in the new scene.
    sharedVariableManager = GameObject.Find("SharedVariableManager").GetComponent<SharedVariableManager>();
    overlayManager = GameObject.Find("OverlayCanvas").GetComponent<OverlayManager>();
    robotViewText = GameObject.Find("RobotViewText").GetComponent<Text>();

    // Set the approprite robot view text.
    switch (trials[currentTrialNum].trialType) {
      case TrialType.AI:
      robotViewText.text = AIText;
      break;
      case TrialType.HUMAN:
      robotViewText.text = humanText;
      break;
    }

    // Display the loading screen depending on the type of trial.
    overlayManager.DisplayLoadingScreen(trials[currentTrialNum].trialType);

    // Load the appropriate models for the player and robot avatars.
    assetLoader.LoadPlayerAvatar(experimentDataManager.appearance.gender, experimentDataManager.appearance.skinColor);
    assetLoader.LoadRobotAvatar(trials[currentTrialNum].robotType, trials[currentTrialNum].robotColor);

    // Load the appropriate environment.
    assetLoader.LoadEnvironment(trials[currentTrialNum].environmentType);
  }

  // Collects the final data in a Trial data structure after a trial has finished.
  private void PackTrialData() {
    // Get the current trial.
    Trial currentTrial = trials[currentTrialNum];

    // Pack information about whether collision and swerve has happened into the trial data.
    currentTrial.collision = sharedVariableManager.collisionHasHappened;
    currentTrial.playerSwerve = sharedVariableManager.playerSwerved;
    currentTrial.robotSwerve = sharedVariableManager.robotSwerved;

    // If player swerved, pack the distances into the trial data.
    if (currentTrial.playerSwerve) {
      currentTrial.playerRobotDistance = sharedVariableManager.playerRobotSwerveDistance;
      currentTrial.playerStartDistance = sharedVariableManager.playerStartSwerveDistance;
    } else {
      currentTrial.playerRobotDistance = -1.0f;
      currentTrial.playerStartDistance = -1.0f;
    }

    // If robot swerved, pack the distances into the trial data.
    if (currentTrial.robotSwerve) {
      currentTrial.robotPlayerDistance = sharedVariableManager.robotPlayerSwerveDistance;
      currentTrial.robotStartDistance = sharedVariableManager.robotStartSwerveDistance;
    } else {
      currentTrial.robotPlayerDistance = -1.0f;
      currentTrial.robotStartDistance = -1.0f;
    }
  }

  // Defines behavior to be executed to move the experiment on after a trial has finished.
  private void NextTrial() {
    // Move on to next trial.
    currentTrialNum++;

    // If there are trials left, go to next trial.
    if (currentTrialNum < trials.Count) {
      // Reload the scene with the new trial.
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    } else { // If there are no trials left:
      // Set the trials property of the experiment data manager.
      experimentDataManager.allCompletedTrials = trials.ToArray();

      // Show collision statistics.
      if (Application.isEditor) {
        int numSwerves = 0;
        int numCollisions = 0;

        foreach (Trial trial in trials) {
          if (trial.playerSwerve) ++numSwerves;
          if (trial.collision) ++numCollisions;
        }

        Debug.Log("Number of trials: " + trials.Count);
        Debug.Log("Number of swerves: " + numSwerves + ", swerve rate: " + ((float) numSwerves / trials.Count));
        Debug.Log("Number of collisions: " + numCollisions + ", collision rate: " + ((float) numCollisions / trials.Count));
      }

      // Load ending scene, initiating packing and sending of data.
      SceneManager.LoadScene("EndingScene");

      // Clean up persistent objects.
      Destroy(assetLoader.gameObject);
      Destroy(gameObject);
    }
  }
}
