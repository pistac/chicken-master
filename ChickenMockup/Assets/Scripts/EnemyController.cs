using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    [SerializeField]
#pragma warning disable
    private float minTime;
    [SerializeField]
    private float maxTime;
#pragma warning restore

    private float accumulatedTime = 0.0f;
    private float thresholdTime;

    void Start() {
        thresholdTime = Random.Range(minTime, maxTime);
    }

    void Update() {
        if (accumulatedTime > thresholdTime && !HelperFunctions.GetGameOver()) {
            GetComponent<Rigidbody>().AddForceAtPosition(transform.forward * 10000, GameObject.Find("EnemyForcePosition").transform.position);
            HelperFunctions.SetGameOver(gameObject, HelperFunctions.GameOverSource.SWERVE);
        } else {
            accumulatedTime += Time.deltaTime;
        }
    }
}
