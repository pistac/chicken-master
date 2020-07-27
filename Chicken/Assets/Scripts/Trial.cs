using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enumeration holding all the different types of trials.
public enum TrialType {
  TEST, // The test trial at the start of the experiment.
  AI, // A trial where the player is told the other agent is an AI.
  HUMAN // A trial where the player is told the other agent is another human.
}

// Data structure to hold information about a trial.
public class Trial {

  public bool collision { get; set; } // Whether a collision happened in the trial.
  public bool playerSwerve { get; set; } // Whether the player swerved in the trial.
  public bool robotSwerve { get; set; } // Whether the robot swerved in the trial.
  public EnvironmentType environmentType { get; private set; }
  public float playerRobotDistance { get; set; } // The distance between player and robot when player swerved.
  public float playerStartDistance { get; set; } // The distance between player and start position when player swerved.
  public float robotPlayerDistance { get; set; } // The distance between robot and player when robot swerved.
  public float robotStartDistance { get; set; } // The distance between robot and start position when robot swerved.
  public int trialNum { get; private set; } // This trial's number in the order.
  public RobotColor robotColor { get; set; }
  public RobotType robotType { get; private set; } // The type of robot that appears in the trial.
  public TrialType trialType { get; private set; } // The type of this trial.

  private static int numTrials = 0; // Static variable to keep track of the total number of trials.

  // Basic constructor that creates a trial of type test with no robot type.
  public Trial() {
    this.environmentType = EnvironmentType.OPEN;
    this.robotType = RobotType.TEST;
    this.trialType = TrialType.TEST;

    // Set trial number to current number of trials, then increment the number of trials.
    // E.g. the first trial will have the trial number 0.
    trialNum = numTrials++;
  }

  // Standard constructor allowing for defining of robot type and trial type.
  public Trial(EnvironmentType environmentType, RobotType robotType, TrialType trialType) {
    this.environmentType = environmentType;
    this.robotType = robotType;
    this.trialType = trialType;

    // Set trial number to current number of trials, then increment the number of trials.
    // E.g. the first trial will have the trial number 0.
    trialNum = numTrials++;
  }

  public override string ToString() {
    return "{collision:" + collision + ", playerSwerve:" + playerSwerve + ", robotSwerve:" + robotSwerve + ", playerRobotDistance:" + playerRobotDistance + ", playerStartDistance:" + playerStartDistance + ", robotPlayerDistance:" + robotPlayerDistance + ", robotStartDistance:" + robotStartDistance + ", trialNum:" + trialNum + ", robotType:" + robotType + ", trialType:" + trialType + "}";
  }
}
