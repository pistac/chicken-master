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

    // Hide the data sent text.
    dataSentText.SetActive(false);

    // Make sure no data is sent from the editor.
    if (Application.isEditor) {
      return;
    }

    // Create empty form to be filled.
    WWWForm form = new WWWForm();

    // The URL for the form action of method POST. Defined by the google forms form.
    string postURL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSflMHv71xaahMF13wde1sVFQIk2ngq71OGbu8QEdHGdoZtKOg/formResponse"; // Final form.

    // All field names are defined in the google forms form.
    // Each name "entry.xxxxxxxxx" in the page source of the form is associated with
    // a field in the form. The names appear in the same order as their associated form fields.

    // Define the google forms entry names for the browser data field.
    string[] entryNames = {
      "entry.2102565097", "entry.1050089162", "entry.325057486", "entry.668756629", "entry.1299027027", "entry.209343723", "entry.2143973390", "entry.1170127299", "entry.1971309472", "entry.959514617", "entry.1448387481", "entry.1498849561", "entry.1638486898", "entry.442090000", "entry.350014395", "entry.356375231", "entry.1309304759", "entry.38794875", "entry.1171193467", "entry.879983441", "entry.680918694", "entry.1106786836", "entry.1836300788", "entry.1065831825", "entry.1849938861", "entry.1888736609", "entry.199685991", "entry.857520269", "entry.977553379", "entry.553380819", "entry.1073660661", "entry.1672035224", "entry.1615693308", "entry.507018996", "entry.989184125", "entry.162187417", "entry.1815978254", "entry.796281002", "entry.1385308899", "entry.144971403", "entry.209492634", "entry.1846236541", "entry.610244547", "entry.640924627", "entry.1205187465", "entry.366145254", "entry.1787509928", "entry.1554757662", "entry.1472227790", "entry.1767150327", "entry.1691391716", "entry.1064521466", "entry.205553732", "entry.891216200", "entry.850884029", "entry.1812249595", "entry.634894112", "entry.1358194031", "entry.507395107", "entry.1677946273", "entry.1389930311", "entry.2024119880", "entry.677488588", "entry.666595036", "entry.1204487034", "entry.752503403", "entry.778445308", "entry.159005044", "entry.1287998562", "entry.59459121", "entry.1228489286", "entry.808833129", "entry.557002799", "entry.1950560555", "entry.1856650253", "entry.1767336715", "entry.1667804196", "entry.762485876", "entry.1028264811", "entry.1608521129", "entry.704817160", "entry.617317449", "entry.1563994397", "entry.1710075187", "entry.184670083", "entry.152965652", "entry.123955457", "entry.681027680", "entry.1695068342", "entry.353900112", "entry.869033953", "entry.1474451829", "entry.939991490", "entry.211043753", "entry.362969139", "entry.60821853", "entry.1146785843", "entry.24193386", "entry.27987899", "entry.1652192115", "entry.1027795179", "entry.1946494531", "entry.445322347", "entry.316062295", "entry.234804059", "entry.1578950088", "entry.1165839429", "entry.1469196843", "entry.30384960", "entry.1907492921", "entry.1709798930", "entry.936868166", "entry.626030777", "entry.1779219849", "entry.1280929265", "entry.1464747003", "entry.711649843", "entry.524931184", "entry.630635211", "entry.327756603", "entry.61937369", "entry.1416826944", "entry.489635428", "entry.1595136968", "entry.199916412", "entry.194938037", "entry.1506447004", "entry.1564931999", "entry.1176109734", "entry.610932943", "entry.1049466728", "entry.1777864433", "entry.857718607", "entry.204287501", "entry.1137653101", "entry.1162151476", "entry.1062180568", "entry.1213508073", "entry.83842712", "entry.629317917", "entry.290610110", "entry.1679535671", "entry.1646461902", "entry.724822568", "entry.686790924", "entry.958512629", "entry.62442000", "entry.1848800995", "entry.143980122", "entry.1596709479", "entry.96611693", "entry.2095172296", "entry.723345761", "entry.2029916574", "entry.2061760441", "entry.575846027", "entry.1831188811", "entry.744952910", "entry.1175503029", "entry.713333567", "entry.1199722310", "entry.1832120513", "entry.642829443", "entry.1932968240", "entry.583340503", "entry.1111166703", "entry.305658947", "entry.265415914", "entry.1593029504", "entry.1221871163", "entry.1171308453", "entry.1780979012", "entry.806223075", "entry.240684259", "entry.1188964473", "entry.1415593360", "entry.196197999", "entry.731226930", "entry.905927214", "entry.1060749197", "entry.1007765480", "entry.1980898855", "entry.1516352616", "entry.32009194", "entry.1689382772", "entry.707087444", "entry.837458053", "entry.590833243", "entry.782851574", "entry.748850698", "entry.120995142", "entry.1058189061", "entry.931833010", "entry.1417346672", "entry.732631853", "entry.1044617373", "entry.1419472031", "entry.1255430642", "entry.806433142", "entry.487521379", "entry.505232445", "entry.1988601980", "entry.2062137073", "entry.352657901", "entry.676781871", "entry.878308445", "entry.1945010534", "entry.1088334287", "entry.1704380725", "entry.1658704227", "entry.1606447449", "entry.1488825025", "entry.1311070530", "entry.1870522338", "entry.1372930152", "entry.1740317376", "entry.1136013579", "entry.703951857", "entry.341959164", "entry.1538726404", "entry.894794263", "entry.70233209", "entry.168923590", "entry.474571341", "entry.89565362", "entry.636553449", "entry.802927827", "entry.1034650556", "entry.372742471", "entry.1937689360", "entry.546587912", "entry.1170429375", "entry.1482689945", "entry.985531814", "entry.558112315", "entry.1843359020", "entry.920394459", "entry.2142056780", "entry.89603061", "entry.240285220", "entry.37428136", "entry.2032568713", "entry.1383321686", "entry.1176133842", "entry.1726931234", "entry.942665654", "entry.898573227", "entry.1823800061", "entry.877187950", "entry.1065864885",
    };

    int entryNameIndex = 0;

    // Add mturk completion code.
    form.AddField(entryNames[entryNameIndex++], completionCode);

    // Add mturk referral field.
    form.AddField(entryNames[entryNameIndex++], fromMturk.ToString());

    // Add browser data.
    for (int i = 0; i < browserData.Length; ++i) {
      form.AddField(entryNames[entryNameIndex++], browserData[i]);
    }

    // Add participant data.
    form.AddField(entryNames[entryNameIndex++], participant.seenRobotsBefore.ToString());
    form.AddField(entryNames[entryNameIndex++], participant.age);
    form.AddField(entryNames[entryNameIndex++], participant.country);
    form.AddField(entryNames[entryNameIndex++], participant.gamesExperience);
    form.AddField(entryNames[entryNameIndex++], participant.gender);
    form.AddField(entryNames[entryNameIndex++], participant.levelOfEducation);
    form.AddField(entryNames[entryNameIndex++], participant.robotExperience);

    // Add appearance data.
    form.AddField(entryNames[entryNameIndex++], appearance.skinColor.ToString());
    form.AddField(entryNames[entryNameIndex++], appearance.gender.ToString());

    // Add comments.
    form.AddField(entryNames[entryNameIndex++], comments.additionalComments);
    form.AddField(entryNames[entryNameIndex++], comments.technicalIssues);

    // Add trial data.
    for (int i = 0; i < allCompletedTrials.Length; ++i) {
      Trial trial = allCompletedTrials[i];

      form.AddField(entryNames[entryNameIndex++], trial.trialType.ToString());
      form.AddField(entryNames[entryNameIndex++], trial.environmentType.ToString());
      form.AddField(entryNames[entryNameIndex++], trial.robotColor.ToString());

      // Send that robot type is not applicable if the trial is a test trial.
      if (trial.trialType == TrialType.TEST) {
        form.AddField(entryNames[entryNameIndex++], "N/A");
      } else {
        form.AddField(entryNames[entryNameIndex++], trial.robotType.ToString());
      }

      form.AddField(entryNames[entryNameIndex++], trial.collision.ToString());
      form.AddField(entryNames[entryNameIndex++], trial.playerSwerve.ToString());

      // Send that robot and start distance are not applicable if the player did not swerve.
      if (!trial.playerSwerve) {
        form.AddField(entryNames[entryNameIndex++], "N/A");
        form.AddField(entryNames[entryNameIndex++], "N/A");
      } else {
        form.AddField(entryNames[entryNameIndex++], trial.playerRobotDistance.ToString());
        form.AddField(entryNames[entryNameIndex++], trial.playerStartDistance.ToString());
      }

      form.AddField(entryNames[entryNameIndex++], trial.robotSwerve.ToString());

      // Send that robot and start distance are not applicable if the player did not swerve.
      if (!trial.robotSwerve) {
        form.AddField(entryNames[entryNameIndex++], "N/A");
        form.AddField(entryNames[entryNameIndex++], "N/A");
      } else {
        form.AddField(entryNames[entryNameIndex++], trial.robotPlayerDistance.ToString());
        form.AddField(entryNames[entryNameIndex++], trial.robotStartDistance.ToString());
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
		else
        {
            Debug.Log("Form upload complete!");
        }
      }

      dataSentText.SetActive(true);
    }
  }
}
