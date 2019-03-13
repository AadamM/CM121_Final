using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// This script is a slighly modified version of the code seen in Sebastian Lague's stealth game tutorial:
// https://www.youtube.com/watch?v=jUdx_Nj4Xk0&list=PLFt_AvWsXl0fnA91TcmkRyhhixX9CO3Lw&index=24
// https://www.youtube.com/watch?v=TfhPBAe9Tt8&list=PLFt_AvWsXl0fnA91TcmkRyhhixX9CO3Lw&index=25
public class EnemyBehavior : MonoBehaviour {

    public float speed = 3f;
    public float waitTime = 5f;

    public float turnSpeed = 45f;

    public Transform pathContainer;

    public Transform player;

    public Light spotLight;
    public float viewDistance = 10f;
    public LayerMask viewMask;

    public TextMeshPro detectedText;

    private float viewAngle;
    private Color defaultLightColor;

    private Vector3[] waypoints;
    private bool isStunned;

    void Start() {
        waypoints = new Vector3[pathContainer.childCount];
        for(int i=0; i<waypoints.Length; i++) {
            waypoints[i] = pathContainer.GetChild(i).position;
        }
        StartCoroutine(FollowPath(waypoints));
        viewAngle = spotLight.spotAngle;
        defaultLightColor = spotLight.color;
    }

    void Update() {
        if (CanDetectPlayer(player)) {
            spotLight.color = Color.red;
            detectedText.text = "I SEE YOU";
        } else {
            spotLight.color = defaultLightColor;
            detectedText.text = "* Whistling *";
        }

        if (isStunned) {
            detectedText.text = "I am stunned! Ah...";
        }
    }

    IEnumerator FollowPath (Vector3[] waypoints) {
        transform.position = waypoints[0];

        int targetWaypointIndex = 1;
        Vector3 targetWaypoint = waypoints[targetWaypointIndex];

        transform.LookAt(targetWaypoint, Vector3.up);

        while (true) {
            if (!isStunned) {
                transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);
            }

            if(transform.position == targetWaypoint) {
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWaypointIndex];
                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine(TurnToward(targetWaypoint));
            }

            yield return null;
        }
    }

    IEnumerator TurnToward (Vector3 targetWaypoint) {
        Vector3 directionToTarget = (targetWaypoint - transform.position).normalized;
        float targetAngle = 90f - Mathf.Atan2(directionToTarget.z, directionToTarget.x) * Mathf.Rad2Deg;

        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f) {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);

            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }

    private void OnDrawGizmos() {
        Vector3 startPos = pathContainer.GetChild(0).position;

        Vector3 prevPos = startPos;
        for(int i=0; i<pathContainer.childCount; i++) {
            Gizmos.DrawSphere(pathContainer.GetChild(i).position, .5f);
            Gizmos.DrawLine(prevPos, pathContainer.GetChild(i).position);
            prevPos = pathContainer.GetChild(i).position;
        }

        Gizmos.DrawLine(prevPos, startPos);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }

    bool CanDetectPlayer(Transform player) {
        if(Vector3.Distance(transform.position, player.position) < viewDistance) {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            PlayerController playerController = FindObjectOfType<PlayerController>();

            if(angleToPlayer < viewAngle / 2f && !playerController.isInvisible) {
                if(!Physics.Linecast(transform.position, player.position, viewMask)) {
                    playerController.AddDetectTime();
                    return true;
                }
            }
        }

        return false;
    }

    public IEnumerator Stun() {
        isStunned = true;
        yield return new WaitForSeconds(3f);
        isStunned = false;
        yield return null;
    }
}
