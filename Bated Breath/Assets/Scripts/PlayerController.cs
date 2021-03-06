﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public GameObject gunModel;

    public Transform gunMuzzleLoc;
    public LineRenderer tracerRound;

    public float fireRate = 3f;
    public float effectDisplayTime = 3f;

    public int startingAmmo = 6;

    public Text ammoText;
    public Text timeUntilLose;
    public Text loseText;

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

    private int currentAmmo;

    public Image crosshair;

    public Animator gunAnimator;
    public Animator cameraAnimator;

    public bool isInvisible;

    public Material visibleMaterial;
    public Material invisibleMaterial;

    private float detectionTime;

    private bool isCrouching;

    public Text scoreText;

    public int score;

    void Start() {
        charCon = GetComponent<CharacterController>();
        defaultHeight = charCon.height;
        gunHolder.transform.position = playerCam.position;
        gunLocOffset = gun.localPosition;
        currentAmmo = startingAmmo;
        ammoText.text = "Ammo: " + currentAmmo;
        scoreText.text = "Score: " + score;
        detectionTime = 3f;
        timeUntilLose.text = "Time Until Failure: " + detectionTime.ToString() + "s";
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

        if (Input.GetButtonDown("Crouch")) {
            isCrouching = !isCrouching;
        }

        if (isCrouching) {
            charCon.height = defaultHeight * crouchRatio;
        } else {
            charCon.height = defaultHeight;
        }

        if((Mathf.Abs(Input.GetAxis("Vertical")) > .3f) && charCon.isGrounded) {
            cameraAnimator.Play("Bob");
        } else {
            cameraAnimator.Play(-1);
        }

        gunHolder.transform.position = Vector3.Lerp(gunHolder.transform.position, playerCam.position, .8f);


        if (Input.GetAxis("Fire2") > .5f) {
            gun.localPosition = Vector3.Lerp(gun.localPosition, gun.InverseTransformPoint(ADSLocation.position), .35f);

            Color tempColor;
            tempColor = crosshair.color;
            tempColor.a = Mathf.Lerp(tempColor.a, 0f, .35f);
            crosshair.color = tempColor;
        } else {
            gun.localPosition = Vector3.Lerp(gun.localPosition, gunLocOffset, .3f);

            Color tempColor;
            tempColor = crosshair.color;
            tempColor.a = Mathf.Lerp(tempColor.a, 1f, .35f);
            crosshair.color = tempColor;
        }

        gunHolder.transform.rotation = Quaternion.Slerp(gunHolder.transform.rotation, playerCam.transform.rotation, .35f);


        // Much of the logic for effects and fire rate comes from this tutorial:
        // https://www.youtube.com/watch?v=l86gpYbQFzY
        if ((Input.GetAxis("Fire1") > .5f) && timeSinceShot >= fireRate && (currentAmmo > 0)) {
            Shoot();
        }

        if(timeSinceShot >= effectDisplayTime) {
            tracerRound.gameObject.SetActive(false);
        }

        timeSinceShot += Time.deltaTime;
    }

    void Shoot() {
        gunAnimator.Play("Shoot");
        timeSinceShot = 0f;
        currentAmmo--;

        if (Physics.Raycast(playerCam.position, playerCam.forward, out hit)) {
            tracerRound.gameObject.SetActive(true);
            tracerPositions = new Vector3[2] { gunMuzzleLoc.position, hit.point };
            tracerRound.SetPositions(tracerPositions);

            if (hit.collider.tag == "Enemy") {
                EnemyBehavior enemyBehavior = hit.collider.gameObject.GetComponentInParent<EnemyBehavior>();
                if (enemyBehavior != null) {
                    enemyBehavior.StartCoroutine(enemyBehavior.Stun());
                }
            }
        }

        ammoText.text = "Ammo: " + currentAmmo;
    }

    public void ChangeAmmo(int ammoCount) {
        currentAmmo += ammoCount;
        ammoText.text = "Ammo: " + currentAmmo;
        score += ammoCount * 5;
        scoreText.text = "Score: " + score;
    }

    public IEnumerator ActivateInvisibility() {
        score += 15;
        scoreText.text = "Score: " + score;

        isInvisible = true;
        foreach (MeshRenderer modelPart in gunModel.GetComponentsInChildren<MeshRenderer>()) {
            modelPart.material = invisibleMaterial;
        }

        yield return new WaitForSeconds(5);

        isInvisible = false;
        foreach (MeshRenderer modelPart in gunModel.GetComponentsInChildren<MeshRenderer>()) {
            modelPart.material = visibleMaterial;
        }

        yield return null;
    }

    public void AddDetectTime() {
        detectionTime -= Time.deltaTime;
        timeUntilLose.text = "Time Until Failure: " + detectionTime.ToString("F2") + "s";
        if(detectionTime <= 0f) {
            loseText.gameObject.SetActive(true);
            detectionTime = 0f;
        }
    }
}
