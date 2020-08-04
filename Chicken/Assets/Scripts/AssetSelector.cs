using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enum for representing the different types of environments.
public enum EnvironmentType {
  TEST,
  BASIC,
  OPEN,
  NARROW
}

// Enum for representing the different types of robots.
public enum RobotType {
  TEST,
  PR2,
  SOPHIA
}

// Enum for representing the different colors that robots can have.
public enum RobotColor {
  BLUE,
  RED,
  PURPLE,
  YELLOW,
  PINK,
  GREEN,
  BROWN,
  ORANGE,
  BLACK,
  SPECIAL
}

// Class that specifies how to tie avatar prefabs to gender and skin color.
public class AssetSelector : MonoBehaviour {

  // Specify variables to drop the different avatar prefabs into in the inspector.
#pragma warning disable
  [SerializeField]
  private GameObject testEnvironment;
  [SerializeField]
  private GameObject basicEnvironment;
  [SerializeField]
  private GameObject openEnvironment;
  [SerializeField]
  private GameObject narrowEnvironment;
  [SerializeField]
  private GameObject female;
  [SerializeField]
  private GameObject male;
  [SerializeField]
  private Material playerBlack;
  [SerializeField]
  private Material playerBrown;
  [SerializeField]
  private Material playerLightYellow;
  [SerializeField]
  private Material playerPink;
  [SerializeField]
  private GameObject PR2;
  [SerializeField]
  private GameObject sophia;
  [SerializeField]
  private Material robotBlue;
  [SerializeField]
  private Material robotRed;
  [SerializeField]
  private Material robotPurple;
  [SerializeField]
  private Material robotYellow;
  [SerializeField]
  private Material robotPink;
  [SerializeField]
  private Material robotGreen;
  [SerializeField]
  private Material robotBrown;
  [SerializeField]
  private Material robotOrange;
  [SerializeField]
  private Material robotBlack;
  [SerializeField]
  private Material robotSpecial;
  [SerializeField]
  private Material sophiaBlue;
  [SerializeField]
  private Material sophiaRed;
  [SerializeField]
  private Material sophiaPurple;
  [SerializeField]
  private Material sophiaYellow;
  [SerializeField]
  private Material sophiaPink;
  [SerializeField]
  private Material sophiaGreen;
  [SerializeField]
  private Material sophiaBrown;
  [SerializeField]
  private Material sophiaOrange;
  [SerializeField]
  private Material sophiaBlack;
  [SerializeField]
  private Material sophiaSpecial;
#pragma warning restore

  public Dictionary<RobotType, float> colliderRadii; // Maps from robot type to the appropriate radius of the robot's capsule collider.
  public Dictionary<EnvironmentType, GameObject> environments; // Maps from environment type to environment prefab.

  public static AssetSelector instance;

  private Dictionary<Gender, string> coloredPlayerParts; // The names of the gameobject on the player avatar to change material of, depending on model.
  private Dictionary<RobotType, string[]> coloredRobotParts; // Maps from robot type to a list of strings with names for robot parts that will have their materials changed.
  private Dictionary<Gender, GameObject> playerAvatars; // Maps from gender expression to player model prefab.
  private Dictionary<SkinColor, Material> playerMaterials; // Maps from skin color to actual material.
  private Dictionary<RobotType, GameObject> robotAvatars; // Maps from RobotType to acutal robot game objects.
  private Dictionary<RobotType, Dictionary<RobotColor, Material>> robotMaterials; // Maps from robot type to mapping from robot color to material.
  private Dictionary<RobotColor, Material> generalRobotMaterials; // Maps from robot color to actual materials for general robots.
  private Dictionary<RobotColor, Material> sophiaMaterials; // Maps from robot color to actual materials for the Sophia robot model.

  // Handle avatar selector instancing between scene loads.
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

  void Start() {
    // Set up all the dictionaries.
    colliderRadii = new Dictionary<RobotType, float>();
    colliderRadii.Add(RobotType.TEST, 0.3f);
    colliderRadii.Add(RobotType.PR2, 0.5f);
    colliderRadii.Add(RobotType.SOPHIA, 0.3f);

    environments = new Dictionary<EnvironmentType, GameObject>();
    environments.Add(EnvironmentType.TEST, testEnvironment);
    environments.Add(EnvironmentType.BASIC, basicEnvironment);
    environments.Add(EnvironmentType.OPEN, openEnvironment);
    environments.Add(EnvironmentType.NARROW, narrowEnvironment);

    coloredPlayerParts = new Dictionary<Gender, string>();
    coloredPlayerParts.Add(Gender.FEMALE_PRESENTING, "female_char/body/body_top_and_pantsLong_and_shoes/opt60_vLSleeve_and_pantsLongShoes");
    coloredPlayerParts.Add(Gender.MALE_PRESENTING, "mdl_male/bodies/opt8_shirt_and_shorts_and_shoes");

    coloredRobotParts = new Dictionary<RobotType, string[]>();
    coloredRobotParts.Add(RobotType.PR2, new string[] {
      "head_tilt_link",
      "l_shoulder_pan_link", "l_upper_arm_link", "l_forearm_link",
      "r_shoulder_pan_link", "r_upper_arm_link", "r_forearm_link" });

    playerAvatars = new Dictionary<Gender, GameObject>();
    playerAvatars.Add(Gender.FEMALE_PRESENTING, female);
    playerAvatars.Add(Gender.MALE_PRESENTING, male);

    playerMaterials = new Dictionary<SkinColor, Material>();
    playerMaterials.Add(SkinColor.BLACK, playerBlack);
    playerMaterials.Add(SkinColor.BROWN, playerBrown);
    playerMaterials.Add(SkinColor.LIGHT_YELLOW, playerLightYellow);
    playerMaterials.Add(SkinColor.PINK, playerPink);

    robotAvatars = new Dictionary<RobotType, GameObject>();
    robotAvatars.Add(RobotType.PR2, PR2);
    robotAvatars.Add(RobotType.SOPHIA, sophia);

    generalRobotMaterials = new Dictionary<RobotColor, Material>();
    generalRobotMaterials.Add(RobotColor.BLUE, robotBlue);
    generalRobotMaterials.Add(RobotColor.RED, robotRed);
    generalRobotMaterials.Add(RobotColor.PURPLE, robotPurple);
    generalRobotMaterials.Add(RobotColor.YELLOW, robotYellow);
    generalRobotMaterials.Add(RobotColor.PINK, robotPink);
    generalRobotMaterials.Add(RobotColor.GREEN, robotGreen);
    generalRobotMaterials.Add(RobotColor.BROWN, robotBrown);
    generalRobotMaterials.Add(RobotColor.ORANGE, robotOrange);
    generalRobotMaterials.Add(RobotColor.BLACK, robotBlack);
    generalRobotMaterials.Add(RobotColor.SPECIAL, robotSpecial);

    sophiaMaterials = new Dictionary<RobotColor, Material>();
    sophiaMaterials.Add(RobotColor.BLUE, sophiaBlue);
    sophiaMaterials.Add(RobotColor.RED, sophiaRed);
    sophiaMaterials.Add(RobotColor.PURPLE, sophiaPurple);
    sophiaMaterials.Add(RobotColor.YELLOW, sophiaYellow);
    sophiaMaterials.Add(RobotColor.PINK, sophiaPink);
    sophiaMaterials.Add(RobotColor.GREEN, sophiaGreen);
    sophiaMaterials.Add(RobotColor.BROWN, sophiaBrown);
    sophiaMaterials.Add(RobotColor.ORANGE, sophiaOrange);
    sophiaMaterials.Add(RobotColor.BLACK, sophiaBlack);
    sophiaMaterials.Add(RobotColor.SPECIAL, sophiaSpecial);

    robotMaterials = new Dictionary<RobotType, Dictionary<RobotColor, Material>>();
    robotMaterials.Add(RobotType.PR2, generalRobotMaterials);
    robotMaterials.Add(RobotType.SOPHIA, sophiaMaterials);
  }

  // Returns the player avatar prefab according to the specified gender.
  public GameObject GetPlayerAvatar(Gender gender) {
    return playerAvatars[gender];
  }

  // Takes a player avatar game object and colors the appropriate parts of the avatar with the wanted color.
  public void ApplyPlayerColor(GameObject playerAvatar, Gender gender, SkinColor skinColor) {
    Material wantedMaterial = playerMaterials[skinColor];
    if (playerAvatar != null && wantedMaterial != null) {
      playerAvatar.transform.Find(coloredPlayerParts[gender]).GetComponent<Renderer>().material = wantedMaterial;
    }
  }

  // Returns a robot model of the wanted type.
  public GameObject GetRobotAvatar(RobotType type) {
    return robotAvatars[type];
  }

  // Takes a robot game object and colors the appropriate parts of the robot with the wanted color.
  public void ApplyRobotColor(GameObject robot, RobotType type, RobotColor color) {
    Material wantedMaterial = robotMaterials[type][color];
    if (robot != null && wantedMaterial != null) {
      if (type == RobotType.SOPHIA) {
        SkinnedMeshRenderer renderer = robot.GetComponentInChildren<SkinnedMeshRenderer>();
        Material[] materials = renderer.materials;
        materials[1] = wantedMaterial;
        renderer.materials = materials;
      } else {
        foreach (string partName in coloredRobotParts[type]) {
          robot.transform.Find(partName).GetComponent<MeshRenderer>().material = wantedMaterial;
        }
      }
    }
  }
}
