using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invisibility : MonoBehaviour {
    PlayerController playerController;

    private void Start() {
        playerController = FindObjectOfType<PlayerController>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            playerController.ActivateInvisibility();
            Destroy(gameObject);
        }
    }
}
