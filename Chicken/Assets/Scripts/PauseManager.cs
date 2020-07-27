using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

// Controls the pausing behavior in the trial scene.
public class PauseManager : MonoBehaviour {

#pragma warning disable
  [SerializeField]
  private CanvasGroup canvasGroup;
#pragma warning restore

  // Checks if the game should be paused each frame and pauses if it should be. Otherwise unpauses.
  void Update() {
    if (!MouseIsOnScreen()) {
      Time.timeScale = 0.0f;
      canvasGroup.alpha = 1.0f;
    } else {
      Time.timeScale = 1.0f;
      canvasGroup.alpha = 0.0f;
    }
  }

  // Returns true if the mouse is inside the game area.
  public bool MouseIsOnScreen() {
#if UNITY_EDITOR
    if (Input.mousePosition.x <= 0 || Input.mousePosition.y <= 0 || Input.mousePosition.x >= Handles.GetMainGameViewSize().x - 1 || Input.mousePosition.y >= Handles.GetMainGameViewSize().y - 1){
      return false;
    }
#else
    if (Input.mousePosition.x <= 0 || Input.mousePosition.y <= 0 || Input.mousePosition.x >= Screen.width - 1 || Input.mousePosition.y >= Screen.height - 1) {
      return false;
    }
#endif

    return true;
  }
}
