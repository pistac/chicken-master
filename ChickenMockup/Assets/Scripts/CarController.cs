using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour {

    [SerializeField]
#pragma warning disable
    float acceleration;
#pragma warning restore

#pragma warning disable
    Rigidbody rigidbody;
#pragma warning restore

    void Start() {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update() {
        
    }

    void FixedUpdate() {
        if (!HelperFunctions.GetGameOver()) {
            rigidbody.velocity += transform.right * acceleration;
        }
        //rigidbody.AddForce(transform.right * acceleration);
        //Debug.Log(transform.right * acceleration);
        //Debug.DrawLine(transform.position, transform.position  + transform.right * 100);
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Enemy")) {
            HelperFunctions.SetGameOver(gameObject, HelperFunctions.GameOverSource.COLLISION);
        }
    }
}
