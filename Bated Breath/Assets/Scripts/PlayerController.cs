using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float moveSpeed = 5f;
    public float lookSensitivity = 20f;
    public float customGravity = .1f;

    public float jumpSpeed = 5f;

    public Transform playerCam;

    public float crouchRatio = .33f;

    private CharacterController charCon;

    private float horDir;
    private float verDir;
    private float forDir;

    private float horRot;
    private float verRot;

    private Vector3 direction;

    private Vector3 crouchVector;
    private float defaultHeight;

    void Start() {
        charCon = GetComponent<CharacterController>();
        defaultHeight = transform.localScale.y;
    }

    void Update() {
        horDir = Input.GetAxis("Horizontal");
        forDir = Input.GetAxis("Vertical");

        if (charCon.isGrounded) {
            verDir = 0f;

            if (Input.GetButton("Jump")) {
                verDir += jumpSpeed;
            }
        } else {
            verDir -= customGravity;
        }

        horRot = Input.GetAxis("Mouse X");
        verRot = -Input.GetAxis("Mouse Y");


        transform.Rotate(new Vector3(0f, horRot * lookSensitivity * Time.deltaTime, 0f));
        playerCam.Rotate(new Vector3(verRot * lookSensitivity * Time.deltaTime, 0f, 0f));

        direction = transform.TransformDirection(horDir, verDir, forDir);

        charCon.Move(direction * Time.deltaTime * moveSpeed);

        if (Input.GetButton("Crouch")) {
            crouchVector = transform.localScale;
            crouchVector.y = defaultHeight * crouchRatio;
            transform.localScale = crouchVector;
        } else {
            crouchVector = transform.localScale;
            crouchVector.y = defaultHeight;
            transform.localScale = crouchVector;
        }
    }
}
