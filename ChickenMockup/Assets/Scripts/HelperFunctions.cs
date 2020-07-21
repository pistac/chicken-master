using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelperFunctions {

    public enum GameOverSource {
        COLLISION,
        SWERVE
    }

    public static void SetGameOver(GameObject caller, GameOverSource source) {
        GameObject.Find("FlagManager").GetComponent<FlagManager>().gameIsOver = true;

        Text promptText = GameObject.Find("Prompt").GetComponent<Text>();

        switch (source) {
            case GameOverSource.COLLISION:
                promptText.text = "You collided!";
                break;
            case GameOverSource.SWERVE:
                promptText.text = caller.name + " swerved!";
                break;
        }
    }

    public static bool GetGameOver() {
        return GameObject.Find("FlagManager").GetComponent<FlagManager>().gameIsOver;
    }
}
