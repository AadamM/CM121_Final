﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float moveSpeed = 5f;
    public float lookSensitivity = 20f;
    public float customGravity = .1f;

    public float jumpSpeed = 5f;

    public Transform playerCam;

    public Transform ADSLocation;

    public float crouchRatio = .33f;

    public GameObject gunHolder;
    public Transform gun;

    public Transform gunMuzzleLoc;
    public LineRenderer tracerRound;

    public float fireRate = 3f;
    public float effectDisplayTime = 3f;

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

    private RaycastHit hit;

    private Vector3[] tracerPositions = new Vector3[2];

    private float timeSinceShot = 0f;

    void Start() {
        charCon = GetComponent<CharacterController>();
        defaultHeight = charCon.height;
        gunHolder.transform.position = playerCam.position;
        gunLocOffset = gun.localPosition;
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

        gunHolder.transform.position = Vector3.Lerp(gunHolder.transform.position, playerCam.position, .8f);

        if (Input.GetMouseButton(1)) {
            gun.localPosition = Vector3.Lerp(gun.localPosition, gun.InverseTransformPoint(ADSLocation.position), .35f);
        } else {
            gun.localPosition = Vector3.Lerp(gun.localPosition, gunLocOffset, .3f);
        }

        gunHolder.transform.rotation = Quaternion.Slerp(gunHolder.transform.rotation, playerCam.transform.rotation, .35f);


        // Much of the logic for effects and fire rate comes from this tutorial:
        // https://www.youtube.com/watch?v=l86gpYbQFzY
        if (Input.GetMouseButton(0) && timeSinceShot >= fireRate) {
            Shoot();
        }

        if(timeSinceShot >= effectDisplayTime) {
            tracerRound.gameObject.SetActive(false);
        }

        timeSinceShot += Time.deltaTime;
    }

    void Shoot() {
        timeSinceShot = 0f;

        if (Physics.Raycast(playerCam.position, playerCam.forward, out hit)) {
            tracerRound.gameObject.SetActive(true);
            tracerPositions = new Vector3[2] { gunMuzzleLoc.position, hit.point };
            tracerRound.SetPositions(tracerPositions);

            if (hit.collider.tag == "Enemy") {
                Debug.Log("hit");
            } else {
                Debug.Log("miss");
            }
        }

        
    }
}
