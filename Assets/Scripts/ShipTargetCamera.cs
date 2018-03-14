using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipTargetCamera : MonoBehaviour {

    public Ship target;

    [Range(2f, 20f)]
    public float yDistance;
    [Range(0.01f, 1f)]
    public float easing;
    [Range(0.2f, 5f)]
    public float thrustMultipler;

    Vector3 targetPosition;

    void FixedUpdate () {
        CalculateTargetPosition();
        EaseTowardsTarget();
    }

    void CalculateTargetPosition () {
        targetPosition = target.transform.position + target.thrustVelocity * thrustMultipler + Vector3.up * yDistance;
    }

    void EaseTowardsTarget () {
        transform.position = Vector3.Lerp(transform.position, targetPosition, easing);
    }

}
