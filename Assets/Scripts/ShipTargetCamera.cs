using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JBirdEngine.ColorLibrary;

public class ShipTargetCamera : MonoBehaviour {

    public Ship target;

    [Range(2f, 50f)]
    public float yDistance;
    [Range(0.01f, 1f)]
    public float aimEasing;
    [Range(0.01f, 1f)]
    public float movementEasing;
    [Range(0.01f, 1f)]
    public float finalEasing;
    [Range(0.2f, 5f)]
    public float thrustMultipler;
    [Range(0.2f, 5f)]
    public float viewMultiplier;
    [Range(0.2f, 5f)]
    public float thrustViewMultiplier;
    [Range(0f, 5f)]
    public float thrustViewMinimum;
    [Range(0.01f, 1f)]
    public float shootingEasing;
    [Range(0.2f, 10f)]
    public float shootingMultiplier;
    [Range(1f, 10f)]
    public float maxTargetLookahead;

    Vector3 targetPosition;

    Vector3 movementTargetPos;
    Vector3 aimTargetPos;

    Vector3 currentMovementPos;
    Vector3 currentAimPos;

    public bool showTargets;

    void FixedUpdate () {
        CalculateTargetPosition();
        EaseTowardsTarget();
    }

    void CalculateTargetPosition () {
        movementTargetPos = target.thrustVelocity * thrustMultipler;
        aimTargetPos = target.transform.forward
            * (target.mainWeaponActive
                ? Mathf.Max(shootingMultiplier, Mathf.Max(thrustViewMinimum, viewMultiplier * target.thrustVelocity.magnitude * thrustViewMultiplier))
                : Mathf.Max(thrustViewMinimum, viewMultiplier * target.thrustVelocity.magnitude * thrustViewMultiplier)
            );
        currentMovementPos = Vector3.Lerp(currentMovementPos, movementTargetPos, movementEasing);
        currentAimPos = Vector3.Lerp(currentAimPos, aimTargetPos, target.mainWeaponActive ? shootingEasing : aimEasing);
        Vector3 lookahead = currentMovementPos + currentAimPos;
        if (lookahead.magnitude > maxTargetLookahead) {
            lookahead = lookahead.normalized * maxTargetLookahead;
        }
        targetPosition = target.transform.position + lookahead + Vector3.up * yDistance;
        if (showTargets) {
            ShowTarget(target.transform.position + movementTargetPos, Color.blue, 0.75f);
            ShowTarget(target.transform.position + aimTargetPos, MoreColors.maroon, 0.75f);
            ShowTarget(target.transform.position + movementTargetPos + aimTargetPos, MoreColors.purple, 0.75f);
            ShowTarget(target.transform.position + currentMovementPos, Color.cyan, 0.75f);
            ShowTarget(target.transform.position + currentAimPos, Color.red, 0.75f);
            ShowTarget(target.transform.position + lookahead, Color.magenta, 0.75f);
            ShowTarget(transform.position - Vector3.up * yDistance, Color.white, 0.75f);
        }
    }

    void EaseTowardsTarget () {
        transform.position = Vector3.Lerp(transform.position, targetPosition, finalEasing);
    }

    void ShowTarget (Vector3 pos, Color color, float size = 1f) {
        Debug.DrawLine(pos + Vector3.forward * size, pos + Vector3.back * size, color);
        Debug.DrawLine(pos + Vector3.right * size, pos + Vector3.left * size, color);
    }

}
