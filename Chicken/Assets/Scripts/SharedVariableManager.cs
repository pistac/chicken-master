using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Events;

// Holds variables, including flags, that multiple classes need access to and
// that do not make sense being part of any one of the accessing classes.
public class SharedVariableManager : MonoBehaviour {

  // Delegate to subscribe to in order to be signalled when the game is over.
  public static UnityAction onGameIsOver;

  // Delegate to subscribe to in order to be signalled when the loading is finished.
  public static UnityAction onLoadIsFinished;

  // Basic flags:

  public bool collisionHasHappened { get; set; } = false;

  public bool playerHasSwerved { get; set; } = false;

  // Delegate-associated flags:

  private bool _gameIsOver = false;
  public bool gameIsOver {
    get => _gameIsOver;
    set {
      bool oldGameIsOver = _gameIsOver;
      _gameIsOver = value;

      // If set to true, was false before and delegate is not null, call delegate.
      if (value == true && oldGameIsOver == false && onGameIsOver != null) {
        onGameIsOver();
      }
    }
  }

  private bool _loadIsFinished = false;
  public bool loadIsFinished {
    get => _loadIsFinished;
    set {
      _loadIsFinished = value;

      // If set to true and delegate is not null, call delegate.
      if (value == true && onLoadIsFinished != null) {
        onLoadIsFinished();
      }
    }
  }

  // Inspector variables:

  // The speed in Unity units per second that the agents move along their respective directions.
  [FormerlySerializedAs("Agent Speed")]
  [SerializeField]
  [Range(0.0f, 10.0f)]
  private float _agentSpeed = 1.0f;
  public float agentSpeed {
    get => _agentSpeed;
    private set => _agentSpeed = value;
  }

  // The speed in Unity units per second that the agents move along their respective directions when swerving.
  [FormerlySerializedAs("Agent Swerve Speed")]
  [SerializeField]
  [Range(0.0f, 10.0f)]
  private float _agentSwerveSpeed = 0.5f;
  public float agentSwerveSpeed {
    get => _agentSwerveSpeed;
    private set => _agentSwerveSpeed = value;
  }

  // The maximum number of Unity units that the robot can get the minimum swerve distance wrong.
  [FormerlySerializedAs("Maximum Robot Swerve Error Ratio")]
  [SerializeField]
  [Range(0.0f, 1.0f)]
  private float _errorMarginMaxRatio = 1.0f;
  public float errorMarginMaxRatio {
    get => _errorMarginMaxRatio;
    private set => _errorMarginMaxRatio = value;
  }

  // The speed in radians per second at which agents rotate along local y when swerving.
  [FormerlySerializedAs("Rotation Speed")]
  [SerializeField]
  [Range(0.0f, 0.5f)]
  private float _rotationSpeed = 0.1f;
  public float rotationSpeed {
    get => _rotationSpeed;
    private set => _rotationSpeed = value;
  }

  // The ratio that defines how large of a part of the width that the margin is.
  [FormerlySerializedAs("Swerve Margin Ratio")]
  [SerializeField]
  [Range(0.0f, 1.0f)]
  private float _swerveMarginRatio = 0.2f;
  public float swerveMarginRatio {
    get => _swerveMarginRatio;
    private set => _swerveMarginRatio = value;
  }

  // Other variables:

  // Distances reported by the player and read by the trial manager.
  public float playerRobotSwerveDistance { get; set; }
  public float playerStartSwerveDistance { get; set; }

  private float _swerveWidthOfLargestAgent;
  public float swerveWidthOfLargestAgent {
    get => _swerveWidthOfLargestAgent;
    set {
      _swerveWidthOfLargestAgent = (value > _swerveWidthOfLargestAgent) ? value : _swerveWidthOfLargestAgent;

      if (Application.isEditor) Debug.Log("swerveWidthOfLargestAgent = " + _swerveWidthOfLargestAgent);
    }
  }
}
