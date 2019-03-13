using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndScript : MonoBehaviour {

    public Text winText;

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player") {
            winText.gameObject.SetActive(true);
        }
    }
}
