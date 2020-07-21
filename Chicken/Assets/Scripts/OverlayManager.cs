using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Class managing the loading screen and loading time of the trial scene loop.
[RequireComponent(typeof(Image))]
public class OverlayManager : MonoBehaviour {

  private static OverlayManager instance;

  // Handle overlay manager uniqueness.
  void Awake() {
    // If there is no instance, let this be the new instance, otherwise, destroy this object.
    if (instance == null) {
      instance = this;
    } else {
      Destroy(gameObject);
      return;
    }
  }

  // Various inspector variables.
#pragma warning disable
  [SerializeField]
  private int fadeFrames;
  [SerializeField]
  private GameObject testTitle;
  [SerializeField]
  private GameObject testText;
  [SerializeField]
  private float testLoadDuration;
  [SerializeField]
  private GameObject AITitle;
  [SerializeField]
  private GameObject AIText;
  [SerializeField]
  private float AILoadDuration;
  [SerializeField]
  private GameObject humanTitle;
  [SerializeField]
  private GameObject humanText;
  [SerializeField]
  private float humanMeanLoadDuration;
  [SerializeField]
  private float humanLoadDurationRange;
#pragma warning restore

  private float fadeStepDuration;
  private CanvasGroup canvasGroup;

  void Start() {
    canvasGroup = GetComponent<CanvasGroup>();

    // Ensure that the range does not exceed the mean, so that the time is never negative.
    Mathf.Clamp(humanLoadDurationRange, 0.0f, humanMeanLoadDuration);
  }

  // Public method called externally telling the OverlayManager to display a certain type of loading screen.
  public void DisplayLoadingScreen(TrialType trialType) {
    // The load is starting, signal that the load is not finished.
    GameObject.Find("SharedVariableManager").GetComponent<SharedVariableManager>().loadIsFinished = false;

    // Decide which type of loading screen to display.
    switch (trialType) {
      case TrialType.TEST:
      StartCoroutine(DrawLoadingScreen(testTitle, testText, testLoadDuration));
      break;
      case TrialType.AI:
      StartCoroutine(DrawLoadingScreen(AITitle, AIText, AILoadDuration));
      break;
      case TrialType.HUMAN:
      // Generate random wait time.
      float humanLoadDuration = humanMeanLoadDuration + Random.Range(-humanLoadDurationRange, humanLoadDurationRange);
      StartCoroutine(DrawLoadingScreen(humanTitle, humanText, humanLoadDuration));
      break;
    }
  }

  // Private method called internally that does the actual drawing of the specified loading screen.
  private IEnumerator DrawLoadingScreen(GameObject title, GameObject text, float duration) {
    title.SetActive(true);
    text.SetActive(true);

    // Wait for the duration.
    yield return new WaitForSecondsRealtime(duration);

    // Signal that the loading is complete.
    GameObject.Find("SharedVariableManager").GetComponent<SharedVariableManager>().loadIsFinished = true;

    // Fade the image and text back out.
    for (int i = 0; i < fadeFrames; ++i) {
      // Calculate new alpha.
      float progress = ((float) i + 1) / ((float) fadeFrames);
      float alpha = Mathf.Lerp(1.0f, 0.0f, progress);

      canvasGroup.alpha = alpha;

      // Wait until next frame.
      yield return new WaitForEndOfFrame();
    }
  }
}
