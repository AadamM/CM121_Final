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

    public GameObject gunHolder;

    private CharacterController charCon;

    private float horDir;
    private float verDir;
    private float forDir;

    private float horRot;
    private float verRot;

    private Vector3 direction;

    private Vector3 crouchVector;
    private float defaultHeight;

    private Vector3 gunLocOffset;

    void Start() {
        charCon = GetComponent<CharacterController>();
        defaultHeight = charCon.height;
        gunHolder.transform.position = playerCam.position;
        //gunLocOffset = gunHolder.transform.position - transform.position;
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
            charCon.height = defaultHeight * crouchRatio;
        } else {
            charCon.height = defaultHeight;
        }

        gunHolder.transform.position = Vector3.Lerp(gunHolder.transform.position, playerCam.position + gunLocOffset, .5f);
        gunHolder.transform.rotation = Quaternion.Lerp(gunHolder.transform.rotation, playerCam.transform.rotation, .5f);
    }
}
