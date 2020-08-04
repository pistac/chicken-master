using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls robot movement.
public class RobotController : MonoBehaviour {

#pragma warning disable
  [SerializeField]
  private SharedVariableManager sharedVariableManager;
  [SerializeField]
  private Transform playerTransform;
  [SerializeField]
  private bool debugDoNotSwerve = false;
#pragma warning restore

  private bool paused = true;
  private bool robotHasSwerved = false;
  private float margin;
  private float swerveDistance;
  private float speed;
  private float swerveForwardSpeed;
  private float swerveSideSpeed;
  private float rotationSpeed;
  private float radius;
  private int swerveDirection = 0;

  private Vector3 positiveSwerveDirection;
  private Vector3 robotDirection;

  void OnEnable() {
    // Subscribe unpausing to when the loading screen is finished.
    SharedVariableManager.onLoadIsFinished += UnPause;
  }

  void OnDisable() {
    // Mandatory unsibscription.
    SharedVariableManager.onLoadIsFinished -= UnPause;
  }

  public void OverrideSwerve() {
    swerveDistance = Mathf.Infinity;
  }

  public void UpdateWidth() {
    radius = GetComponent<CapsuleCollider>().radius;
    margin = sharedVariableManager.swerveMargin;
    sharedVariableManager.robotRadius = radius;

    // Minimum distance is calculated as the distance away from the player the robot
    // can be and still have time to swerve away assuming the player does not swerve.
    // 0.70710678118 is sin(45deg), sharedVariableManager.swerveSideSpeedRatio is always 0.5f.
    float minimumDistance = (sharedVariableManager.robotRadius + sharedVariableManager.playerRadius + sharedVariableManager.swerveMargin) * 0.70710678118f * ((2-sharedVariableManager.swerveSideSpeedRatio) / sharedVariableManager.swerveSideSpeedRatio + 1);
    // An error margin is added to the minimum distance in order to have differen behavior each time.
    swerveDistance = HelperFunctions.GenerateGaussian(minimumDistance, sharedVariableManager.swerveStandardDeviation);
    if (Application.isEditor) {
      if (debugDoNotSwerve) {
        swerveDistance = -10.0f;
      }
      Debug.Log(sharedVariableManager.robotRadius + ", " +  sharedVariableManager.playerRadius + ", " + sharedVariableManager.swerveMargin + ", " + sharedVariableManager.swerveSideSpeedRatio);
      Debug.Log("minimumDistance = " + minimumDistance);
      Debug.Log("swerveDistance = " + swerveDistance);
    }
  }

  void Awake() {
    robotDirection = transform.parent.InverseTransformDirection(transform.forward);
    positiveSwerveDirection = transform.parent.InverseTransformDirection(transform.right);
    speed = sharedVariableManager.agentSpeed;
    swerveForwardSpeed = speed * (1 - sharedVariableManager.swerveSideSpeedRatio);
    swerveSideSpeed = speed * sharedVariableManager.swerveSideSpeedRatio;
    rotationSpeed = sharedVariableManager.rotationSpeed;
  }

  void Start() {
    UpdateWidth();
  }

  public void Pause() {
    paused = true;
  }

  public void UnPause() {
    paused = false;
  }

  void Update() {
    // Only execute if not paused.
    if (paused) return;
    // Check if robot has swerved fully.
    if (Mathf.Abs(transform.localPosition.x) >= sharedVariableManager.swerveWidthOfLargestAgent) {
      robotHasSwerved = true;
    }
  }

  void FixedUpdate() {
    // Only execute if not paused.
    if (paused) return;

    // Handle the movement of the robot.
    MoveRobot();
  }

  // Moves the robot forward, swerving according to player position.
  // Rotation is also controlled by this method.
  private void MoveRobot() {
    float playerRobotDistance = Vector3.Distance(playerTransform.localPosition, transform.localPosition);

    // If the player is beyond the minimum swerve distance or has already swerved.
    // Also if the robot has swerved fully. Move normally.
    if (playerRobotDistance > swerveDistance ||
        sharedVariableManager.playerHasSwerved ||
        robotHasSwerved) {
      // Move robot in robotDirection at speed.
      transform.localPosition += robotDirection * speed * Time.fixedDeltaTime;

      // Rotate robot.
      transform.localRotation = Quaternion.LookRotation(
          Vector3.RotateTowards(transform.parent.InverseTransformDirection(transform.forward),
          robotDirection, rotationSpeed, 0.0f));
    } else { // If the player is within the minimum swerve distance.
      // Pick the appropriate swerve direction if none has been picked, and report distances.
      if (swerveDirection == 0) {
        PickSwerveDirection();

        sharedVariableManager.robotSwerved = true;

        // Report swerve distances.
        sharedVariableManager.robotPlayerSwerveDistance = (playerTransform.position - transform.position).z;
        sharedVariableManager.robotStartSwerveDistance = (GameObject.Find("RobotStartPoint").transform.position - transform.position).z;
      }

      // Move robot along swerve direction.
      // Vector3 direction = Vector3.Normalize(robotDirection + (positiveSwerveDirection * swerveDirection));
      // transform.localPosition += direction * swerveSpeed * Time.fixedDeltaTime;
      Vector3 swerveVelocity = swerveForwardSpeed * robotDirection + swerveSideSpeed * positiveSwerveDirection * swerveDirection;
      transform.localPosition += swerveVelocity * Time.fixedDeltaTime;

      // Rotate robot.
      transform.localRotation = Quaternion.LookRotation(
          Vector3.RotateTowards(transform.parent.InverseTransformDirection(transform.forward),
          swerveVelocity.normalized, rotationSpeed, 0.0f));
    }
  }

  // Picks and sets the swerve direction to be away from the player's swerved direction.
  // If player has not yet swerved, pick uniformly random direction.
  private void PickSwerveDirection() {
    float playerSwerveAmount = playerTransform.localPosition.x;
    // If player has started swerving to their right.
    if (playerSwerveAmount > 0) {
      swerveDirection = 1; // Swerve right.
    } else if (playerSwerveAmount < 0) { // If player has started swerving to their left.
      swerveDirection = -1; // Swerve left.
    } else { // If player has not swerved at all.
      // Pick randomly to swerve left or right.
      swerveDirection = Random.Range(0, 2) == 0 ? -1 : 1;
    }
  }
}
