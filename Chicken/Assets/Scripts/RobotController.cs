using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls robot movement.
public class RobotController : MonoBehaviour {

#pragma warning disable
  [SerializeField]
  private bool debugDoNotSwerve = false;
#pragma warning restore

  private bool paused = true;
  private bool robotHasSwerved = false;
  private float margin;
  private float swerveDistance;
  private float speed;
  private float swerveSpeed;
  private float rotationSpeed;
  private float width;
  private int swerveDirection = 0;
  private SharedVariableManager sharedVariableManager;
  private Vector3 positiveSwerveDirection;
  private Vector3 robotDirection;
  private Transform playerTransform;

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
    width = GetComponent<CapsuleCollider>().radius * 2;
    margin = width * sharedVariableManager.swerveMarginRatio; // The swerving margin is a part of the width.
    sharedVariableManager.swerveWidthOfLargestAgent = width + margin;

    // Minimum distance is calculated as the distance away from the player the robot
    // can be and still have time to swerve away assuming the player does not swerve.
    // In this case, it is based on the distance the player moves at its usual speed (speed)
    // during the time the robot is able to move at the swerve speed side component (swerveSpeed * cos45)
    // the sufficient distance to the side (width + margin).
    float minimumDistance = (speed + swerveSpeed) * sharedVariableManager.swerveWidthOfLargestAgent / (swerveSpeed * Mathf.Sqrt(2) / 2);
    // An error margin is added to the minimum distance in order to have differen behavior each time.
    swerveDistance = minimumDistance * Random.Range(1 - sharedVariableManager.errorMarginMaxRatio, 1 + sharedVariableManager.errorMarginMaxRatio);
    if (Application.isEditor) {
      if (debugDoNotSwerve) {
        swerveDistance = -10.0f;
      }
      Debug.Log("swerveDistance = " + swerveDistance);
    }
  }

  void Start() {
    sharedVariableManager = GameObject.Find("SharedVariableManager").GetComponent<SharedVariableManager>();

    robotDirection = transform.parent.InverseTransformDirection(transform.forward);
    positiveSwerveDirection = transform.parent.InverseTransformDirection(transform.right);
    playerTransform = GameObject.FindWithTag("Player").transform;
    speed = sharedVariableManager.agentSpeed;
    swerveSpeed = sharedVariableManager.agentSwerveSpeed;
    rotationSpeed = sharedVariableManager.rotationSpeed;
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
        sharedVariableManager.robotPlayerSwerveDistance = Vector3.Distance(transform.position, playerTransform.position);
        sharedVariableManager.robotStartSwerveDistance = Vector3.Distance(transform.position, GameObject.Find("RobotStartPoint").transform.position);
      }

      // Move robot along swerve direction.
      Vector3 direction = Vector3.Normalize(robotDirection + (positiveSwerveDirection * swerveDirection));
      transform.localPosition += direction * swerveSpeed * Time.fixedDeltaTime;

      // Rotate robot.
      transform.localRotation = Quaternion.LookRotation(
          Vector3.RotateTowards(transform.parent.InverseTransformDirection(transform.forward),
          direction, rotationSpeed, 0.0f));
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
