using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextAnimator : MonoBehaviour {

    [SerializeField]
#pragma warning disable
    private float minScale;
    [SerializeField]
    private float maxScale;
    [SerializeField]
    [Range(0.0f, 10.0f)]
    private float speed;
#pragma warning restore

    private int scalingDirection = 1;

    void Update() {
        float currentScale = transform.localScale.x;

        if (currentScale <= minScale) {
            scalingDirection = 1;
        }

        if (currentScale >= maxScale) {
            scalingDirection = -1;
        }

        transform.localScale += Vector3.one * scalingDirection * speed * Time.deltaTime;
    }
}
