using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls player input and movement. Also checks for game over conditions.
public class PlayerController : MonoBehaviour {

#pragma warning disable
  [SerializeField]
  private SharedVariableManager sharedVariableManager;
  [SerializeField]
  private Transform robotTransform;
  [SerializeField]
  private float gameOverDelay = 3.0f;
  [SerializeField]
  private float collisionDelay = 0.5f;
#pragma warning restore

  private bool currentlySwerving = false;
  private bool paused = true;
  private float margin;
  private float speed;
  private float swerveForwardSpeed;
  private float swerveSideSpeed;
  private float rotationSpeed;
  private float radius;
  private int swerveDirection = 0;
  private Vector3 playerDirection;
  private Vector3 positiveSwerveDirection;
  private Vector3 upDirection;

  void OnEnable() {
    // Subscribe unpausing to when the loading screen is finished.
    SharedVariableManager.onLoadIsFinished += UnPause;
  }

  void OnDisable() {
    // Mandatory unsibscription.
    SharedVariableManager.onLoadIsFinished -= UnPause;
  }

  void Awake() {
    // Calculate movement basis vectors relative to parent.
    playerDirection = transform.parent.InverseTransformDirection(transform.forward);
    positiveSwerveDirection = transform.parent.InverseTransformDirection(transform.right);

    speed = sharedVariableManager.agentSpeed;
    swerveForwardSpeed = speed * (1 - sharedVariableManager.swerveSideSpeedRatio);
    swerveSideSpeed = speed * sharedVariableManager.swerveSideSpeedRatio;
    rotationSpeed = sharedVariableManager.rotationSpeed;
    radius = GetComponent<CapsuleCollider>().radius;
    margin = sharedVariableManager.swerveMargin; // The swerving margin is a part of the width.
    sharedVariableManager.playerRadius = radius;
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

    // Check if player has swerved fully.
    if (Mathf.Abs(transform.localPosition.x) >= sharedVariableManager.swerveWidthOfLargestAgent) {
      sharedVariableManager.playerHasSwerved = true;
      currentlySwerving = false;
    }

    // Check if experiment is over.
    if (ExperimentIsOver() && !sharedVariableManager.gameIsOver) {
      // Commence game over sequence.
      StartCoroutine(GameOverSequence());
    }
  }

  // Waits for a specified time period, then sets the gameIsOver flag.
  IEnumerator GameOverSequence() {
    yield return new WaitForSecondsRealtime(gameOverDelay);
    sharedVariableManager.gameIsOver = true;
  }

  void FixedUpdate() {
    // Only execute if not paused.
    if (paused) return;

    // Handle the movement of the player.
    MovePlayer();
  }

  // Take player input and moves the player forward, swerving according to input.
  // Rotation is also controlled by this method.
  private void MovePlayer() {
    // Check for swerving input.
    bool swerveInput = Input.GetKey(KeyCode.Space);

    // If there is swerve input and player is not currently swerving.
    if (swerveInput && !currentlySwerving) {
      sharedVariableManager.playerSwerved = true;

      // Report swerve distances.
      sharedVariableManager.playerRobotSwerveDistance = (robotTransform.position - transform.position).z;
      sharedVariableManager.playerStartSwerveDistance = (GameObject.Find("PlayerStartPoint").transform.position - transform.position).z;

      // Pick a random swerve direction.
      PickSwerveDirection();
      currentlySwerving = true;
    }

    // If the player is not currently swerving or has already swerved.
    if (!currentlySwerving || sharedVariableManager.playerHasSwerved) {
      // Move player in playerDirection at speed.
      transform.localPosition += playerDirection * speed * Time.fixedDeltaTime;

      // Rotate player towards playerDirection.
      transform.localRotation = Quaternion.LookRotation(
          Vector3.RotateTowards(transform.parent.InverseTransformDirection(transform.forward),
          playerDirection, rotationSpeed, 0.0f));
    } else { // If the player is currently swerving and has not yet swerved fully.
      // Move player along swerve direction.
      // Vector3 direction = Vector3.Normalize(playerDirection + (positiveSwerveDirection * swerveDirection));
      // transform.localPosition += direction * swerveSpeed * Time.fixedDeltaTime;
      Vector3 swerveVelocity = swerveForwardSpeed * playerDirection + swerveSideSpeed * positiveSwerveDirection * swerveDirection;
      transform.localPosition += swerveVelocity * Time.fixedDeltaTime;

      // Rotate player towards the swerve direction.
      transform.localRotation = Quaternion.LookRotation(
          Vector3.RotateTowards(transform.parent.InverseTransformDirection(transform.forward),
          swerveVelocity.normalized, rotationSpeed, 0.0f));
    }
  }

  // Checks for collisions between the player and the robot.
  private void OnCollisionEnter(Collision other) {
    if (other.gameObject.tag == "Robot") {
      sharedVariableManager.collisionHasHappened = true;
      if (Application.isEditor) Debug.Log("collided");
      StartCoroutine(PauseAfterCollision());
    }
  }

  IEnumerator PauseAfterCollision() {
    Pause();
    RobotController robot = FindObjectOfType<RobotController>();
    robot.Pause();

    Animator playerAnimator = gameObject.GetComponentInChildren<Animator>();
    Animator robotAnimator = robot.gameObject.GetComponentInChildren<Animator>();

    if (playerAnimator != null) {
      playerAnimator.SetTrigger("getHitMiddle");
    }

    if (robotAnimator != null && robotAnimator.gameObject.name.Contains("Sophia")) {
      robotAnimator.SetTrigger("getHitMiddle");
    }

    yield return new WaitForSecondsRealtime(collisionDelay);

    if (playerAnimator != null) {
      playerAnimator.SetTrigger("walk");
    }

    if (robotAnimator != null && robotAnimator.gameObject.name.Contains("Sophia")) {
      robotAnimator.SetTrigger("walk");
    }

    UnPause();
    robot.UnPause();
    robot.OverrideSwerve();
  }

  // Checks if the condition for the experiment to be over has been reached.
  // In this case, the experiment is over if the robot has passed the
  private bool ExperimentIsOver() {
    return robotTransform.localPosition.z <= transform.localPosition.z ||
          sharedVariableManager.collisionHasHappened;
  }

  // Picks and sets the swerve direction to be away from the robot's swerved direction.
  // If robot has not yet swerved, pick uniformly random direction.
  private void PickSwerveDirection() {
    float robotSwerveAmount = GameObject.Find("RobotAgent").transform.localPosition.x;
    // If player has started swerving to their right.
    if (robotSwerveAmount < 0) {
      swerveDirection = 1; // Swerve right.
    } else if (robotSwerveAmount > 0) { // If player has started swerving to their left.
      swerveDirection = -1; // Swerve left.
    } else { // If player has not swerved at all.
      // Pick randomly to swerve left or right.
      swerveDirection = Random.Range(0, 2) == 0 ? -1 : 1;
    }
  }
}
