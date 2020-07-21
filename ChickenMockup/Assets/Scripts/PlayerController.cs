using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKeyDown(KeyCode.Space) && !HelperFunctions.GetGameOver()) {
            GetComponent<Rigidbody>().AddForceAtPosition(transform.forward * 10000, GameObject.Find("PlayerForcePosition").transform.position);
            HelperFunctions.SetGameOver(gameObject, HelperFunctions.GameOverSource.SWERVE);
        }
    }
}
