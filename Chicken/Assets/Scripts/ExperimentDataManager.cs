using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Runtime.InteropServices;

// Data structure for holding all the different parts of the experiment data:
// avatar appearance, participant information and trial data.
// Handles the arranging and sending of the data to the database.
public class ExperimentDataManager : MonoBehaviour {

  // Inspector variable for defining the length of the mturk completion code.
  [SerializeField]
  private int completionCodeLength = 10;

  private static ExperimentDataManager instance;

  // Indicates whether the game was loaded as a result of clicking on the survey link in mturk.
  private bool fromMturk = false;

  // Contains the different browser data in the order of:
  // usedBrowser, device width, device height, device pixel ratio, color depth, pixel depth.
  private string[] browserData;

  // The different parts of the experiment data.
  // Appearance is normally set in the intro scene.
  // Comments are set after the participant data in the ending scene.
  // Participant data is normally set in the ending scene.
  // Completion code is set inside this class and read by the ending manager.
  // Trial data is set at the end of the trial scene loop.
  public Appearance appearance { get; set; }
  public Comments comments { get; set; }
  public Participant participant { get; set; }
  public string completionCode { get; private set; }
  public string pilotQuestionAnswer { get; set; }
  public Trial[] allCompletedTrials { get; set; }

  // The text that is displayed once the data has been sent succesfully without any errors at all.
  private GameObject dataSentText;

  // Alphabet of the random characters available to pick when constructing the completion code.
  private string randomCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

  // Handle experiment data manager instancing between scene loads.
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

  // Start function strictly for debugging purposes.
  void Start() {
    // If playing in editor, the player might not start in the intro scene, and needs default experiment data.
    if (Application.isEditor) {
      if (participant == null) participant = new Participant(false, 100, "country", "games", "gender", "education", "robot");
      if (appearance == null) appearance = new Appearance(SkinColor.BLACK, Gender.FEMALE_PRESENTING);
      if (comments == null) comments = new Comments("", "");
      if (allCompletedTrials == null) allCompletedTrials = new Trial[0];
    }
  }

  // Receives the referrer url from the javascript script and checks if it is from mturk.
  // If it is, sets fromMturk.
  public void ProcessReferrer(string url) {
    if (url != null) {
      // If the url contains "mturk", the player was referred from mturk.
      fromMturk = url.Contains("mturk");
    }
  }

  // Receives the browser data from the javascript script and splits it into its components.
  // Sets browserdata.
  public void ProcessBrowserData(string data) {
    if (data != null) {
      // Split the data along the token string "SPLIT".
      browserData = data.Split(new string[] {"SPLIT"}, StringSplitOptions.None);
    }
  }

  // Organizes the experiment data and sends it to the desired google forms url for data collection.
  public void PackAndSendExperimentData() {
    // Find the data sent text.
    dataSentText = GameObject.Find("SentText");

    // Generate mturk survey completion code.
    completionCode = "";
    for (int i = 0; i < completionCodeLength; ++i) {
      completionCode += randomCharacters[UnityEngine.Random.Range(0, randomCharacters.Length)];
    }

    // Only organize and send data if the game is loaded from clicking the mturk link.
    if (!fromMturk) {
      dataSentText.GetComponent<Text>().text = "Not referred by mturk, no data sent.";
      return;
    }

    // Hide the data sent text.
    dataSentText.SetActive(false);

    // Create empty form to be filled.
    WWWForm form = new WWWForm();

    // The URL for the form action of method POST. Defined by the google forms form.
    string postURL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSflMHv71xaahMF13wde1sVFQIk2ngq71OGbu8QEdHGdoZtKOg/formResponse"; // Final form.

    // All field names are defined in the google forms form.
    // Each name "entry.xxxxxxxxx" in the page source of the form is associated with
    // a field in the form. The names appear in the same order as their associated form fields.

    form.AddField("entry.810487420", pilotQuestionAnswer);

    // Add mturk completion code.
    form.AddField("entry.2102565097", completionCode);

    // Define the google forms entry names for the browser data field.
    string[] browserDataEntryNames = {
      "entry.325057486", "entry.668756629", "entry.1299027027", "entry.209343723", "entry.2143973390", "entry.1170127299"
    };

    // Add browser data.
    for (int i = 0; i < browserData.Length; ++i) {
      form.AddField(browserDataEntryNames[i], browserData[i]);
    }

    // Add participant data.
    form.AddField("entry.1971309472", participant.seenRobotsBefore.ToString());
    form.AddField("entry.959514617", participant.age);
    form.AddField("entry.1448387481", participant.country);
    form.AddField("entry.1498849561", participant.gamesExperience);
    form.AddField("entry.1638486898", participant.gender);
    form.AddField("entry.442090000", participant.levelOfEducation);
    form.AddField("entry.350014395", participant.robotExperience);

    // Add appearance data.
    form.AddField("entry.356375231", appearance.skinColor.ToString());
    form.AddField("entry.1309304759", appearance.gender.ToString());

    // Add comments.
    form.AddField("entry.38794875", comments.additionalComments);
    form.AddField("entry.1171193467", comments.technicalIssues);

    // Define the google forms entry names for each trial.
    string[,] trialEntryNames = {
      {"entry.879983441","entry.680918694","entry.1106786836","entry.1836300788","entry.1065831825","entry.1849938861","entry.1888736609","entry.199685991"},
      {"entry.335766038","entry.1453525793","entry.934214645","entry.1463040831","entry.1546759079","entry.352759968","entry.1938948057","entry.372870555"},
      {"entry.1108675670","entry.2099749519","entry.686536743","entry.1007710289","entry.553595352","entry.68145581","entry.1613375165","entry.369633415"},
      {"entry.547662755","entry.1341296202","entry.2016970015","entry.565818566","entry.516080432","entry.624369123","entry.1289430951","entry.651925165"},
      {"entry.1686098330","entry.1353514503","entry.1147680330","entry.463536733","entry.361701779","entry.935620407","entry.799813456","entry.872932068"},
      {"entry.1805301822","entry.908710470","entry.1655319026","entry.669290301","entry.293867272","entry.1976823509","entry.916877349","entry.1697271908"},
      {"entry.1833734458","entry.560654799","entry.102216654","entry.1021738104","entry.239221839","entry.1086998453","entry.1913373518","entry.879181614"},
      {"entry.1051276648","entry.24681248","entry.2040495134","entry.1036063803","entry.818857647","entry.1125434731","entry.1463591272","entry.2038625118"},
      {"entry.1092458334","entry.82790655","entry.2004824751","entry.1016526827","entry.467810038","entry.370641291","entry.1681493879","entry.318090532"},
      {"entry.1736037603","entry.1712962438","entry.786867737","entry.1393834200","entry.910339490","entry.1151404678","entry.2146224678","entry.973371027"},
      {"entry.2143721170","entry.712001352","entry.919974641","entry.1142628345","entry.1737435162","entry.1765279931","entry.885230708","entry.1394075929"},
      {"entry.1711784366","entry.384591618","entry.727551167","entry.413515915","entry.1013973194","entry.1189584915","entry.1355563117","entry.1147670153"},
      {"entry.488222834","entry.1819301157","entry.229626024","entry.2139534803","entry.1264152858","entry.322839153","entry.1237410752","entry.1520056163"},
      {"entry.1922678614","entry.1784253077","entry.1014114353","entry.1711659410","entry.274965944","entry.134272482","entry.1024507424","entry.230611040"},
      {"entry.1159200869","entry.2103098658","entry.2068176126","entry.624704976","entry.723624491","entry.1285381588","entry.266196737","entry.1690403357"},
      {"entry.1942776621","entry.970083436","entry.1432431720","entry.1232356477","entry.122687633","entry.1441986603","entry.1606775578","entry.1200963722"},
      {"entry.221276858","entry.1325730178","entry.1322541997","entry.1256238534","entry.766982390","entry.1384350353","entry.1269286734","entry.1648182067"},
      {"entry.1551881026","entry.885470804","entry.318883204","entry.2076447515","entry.757382736","entry.1801881580","entry.1893952987","entry.1694451280"},
      {"entry.599367667","entry.1193692957","entry.84537236","entry.383814949","entry.1590380662","entry.195685737","entry.90589781","entry.399713135"},
      {"entry.1826138451","entry.214842897","entry.1926166373","entry.1793449713","entry.593745412","entry.32747945","entry.357377458","entry.399639497"},
      {"entry.1965854686","entry.1632869581","entry.357488391","entry.2025558837","entry.1459318163","entry.62423361","entry.1063948085","entry.299054411"}
    };

    // Add trial data.
    for (int i = 0; i < allCompletedTrials.Length; ++i) {
      Trial trial = allCompletedTrials[i];

      form.AddField(trialEntryNames[i,0], trial.trialType.ToString());
      form.AddField(trialEntryNames[i,1], trial.environmentType.ToString());
      form.AddField(trialEntryNames[i,2], trial.robotColor.ToString());

      // Send that robot type is not applicable if the trial is a test trial.
      if (trial.trialType == TrialType.TEST) {
        form.AddField(trialEntryNames[i,3], "N/A");
      } else {
        form.AddField(trialEntryNames[i,3], trial.robotType.ToString());
      }

      form.AddField(trialEntryNames[i,4], trial.collision.ToString());
      form.AddField(trialEntryNames[i,5], trial.swerve.ToString());

      // Send that robot and start distance are not applicable if the player did not swerve.
      if (!trial.swerve) {
        form.AddField(trialEntryNames[i,6], "N/A");
        form.AddField(trialEntryNames[i,7], "N/A");
      } else {
        form.AddField(trialEntryNames[i,6], trial.robotDistance.ToString());
        form.AddField(trialEntryNames[i,7], trial.startDistance.ToString());
      }
    }

    // Start sending form data in a separate coroutine for asynchronous behavior.
    StartCoroutine(SendFormData(postURL, form));
  }

  // Sends the specified form data to the specified url using the POST method with a UnityWebRequest.
  IEnumerator SendFormData(string postURL, WWWForm form) {
    // Create temporary UnityWebRequest.
    using (UnityWebRequest w = UnityWebRequest.Post(postURL, form)) {
      // Send web request and wait for response.
      yield return w.SendWebRequest();

      // Logs potential errors or displays the text indicating no errors.
      // When playing in the editor there should be no errors.
      // When playing in a browser locally or online, there is a CORS error because
      // we are not allowed to fetch data from the url, but the data is still posted correctly.
      if (w.isNetworkError || w.isHttpError) {
        Debug.LogError(w.error);
      } else {
        dataSentText.SetActive(true);
      }
    }
  }
}
