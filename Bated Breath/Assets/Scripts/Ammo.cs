using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour {
    PlayerController playerController;

    private void Start() {
        playerController = FindObjectOfType<PlayerController>();
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player") {
            playerController.ChangeAmmo(6);
            Destroy(gameObject);
        }
    }
}
