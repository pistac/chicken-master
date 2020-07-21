using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Data structure to hold a participant's personal information.
public class Participant {

  public bool seenRobotsBefore { get; private set; }
  public int age { get; private set; }
  public string country { get; private set; }
  public string gamesExperience { get; private set; }
  public string gender { get; private set; }
  public string levelOfEducation { get; private set; }
  public string robotExperience { get; private set; }

  public Participant(bool seenRobotsBefore, int age, string country, string gamesExperience, string gender, string levelOfEducation, string robotExperience) {
    this.seenRobotsBefore = seenRobotsBefore;
    this.age = age;
    this.country = country;
    this.gamesExperience = gamesExperience;
    this.gender = gender;
    this.levelOfEducation = levelOfEducation;
    this.robotExperience = robotExperience;
  }

  public override string ToString() {
    return "{seenRobotsBefore:" + seenRobotsBefore + ", age:" + age + ", country:" + country + ", gamesExperience:" + gamesExperience + ", gender:" + gender + ", levelOfEducation:" + levelOfEducation + ", robotExperience:" + robotExperience + "}";
  }
}
