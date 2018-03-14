﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JBirdEngine;

public enum ShipModifiers {
    Snake = 2<<0,
    BustedEngine = 2<<1,
    Squad = 2<<2,
}

public enum ShipAbility {
    None = 0,
    RamThrusters = 1,
    CloakingDevice = 2,
    VendingMachine = 3,
    ChargeShot = 4,
}

[System.Serializable]
public class PartAnchor {
    public Vector3 relativePos;
    public Vector3 relativeDir;
}

public class Ship : MonoBehaviour {

    [Header("Ship Stats")]
    public float mass;
    public float maxSpeed;

    [Header("Ship Mods")]
    [EnumHelper.EnumFlags]
    public ShipModifiers mods;
    public ShipAbility ability;

    [Header("Parts")]
    public Engine engine;
    public Weapon weapon;
    public Shield shield;

    [Header("Part Mods")]
    [EnumHelper.EnumFlags]
    public WeaponModifiers weaponMods;
    [EnumHelper.EnumFlags]
    public ShieldModifiers shieldMods;

    [Header("RCS Thrusters")]
    public Engine rightSideThruster;
    public Engine leftSideThruster;
    public Engine frontRightThruster;
    public Engine frontLeftThruster;

    [Header("Anchor Points")]
    public PartAnchor mainWeaponAnchor;
    public PartAnchor spreadWeaponAnchor;
    public PartAnchor sideWeaponAnchor;
    public PartAnchor engineAnchor;
    public PartAnchor shieldAnchor;

    [Header("Easing")]
    [Range(0.01f,1f)]
    public float yawEasing;
    [Range(0.01f, 1f)]
    public float friction;

    [Header("Runtime")]
    public Vector2 targetYaw;
    public Vector3 thrustVelocity;
    public bool mainThrusterActive;
    public bool mainWeaponActive;

    void FixedUpdate () {
        // Turn first, for accuracy
        MoveTowardsTargetYaw();
        // Fire weapons
        FireWeaponAndHandleKickback();
        // Calculate Thrust
        CalculateTotalThrust();
        AdjustThrustFromFriction();
        MoveFromThrust();
    }

    public void SetTargetYaw (Vector2 yaw) {
        targetYaw = yaw.normalized;
    }

    public void MoveTowardsTargetYaw () {
        Vector3 target = new Vector3(targetYaw.x, 0, targetYaw.y).normalized;
        float azimuth = transform.forward.GetAzimuth(Vector3.zero, Vector3.up, target);
        if (azimuth > 180f) {
            azimuth -= 360f;
        }
        float easeAngle = azimuth * yawEasing;
        Vector3 easeTarget = VectorHelper.FromAzimuthAndElevation(easeAngle, 0f, Vector3.up, transform.forward).normalized;
        if (easeTarget.magnitude != 0f) {
            transform.rotation = Quaternion.LookRotation(VectorHelper.FromAzimuthAndElevation(easeAngle, 0f, Vector3.up, transform.forward), Vector3.up);
        }
    }

    public void FireWeaponAndHandleKickback () {
        if (mainWeaponActive) {
            Vector3 kickback = weapon.TryFire(mass); // update this line for spread/side shot
            Vector3 mitigation = engine.GetKickbackMitigation(mass);
            if (mitigation.magnitude > kickback.magnitude) {
                engine.OverheatFromThrust(-kickback);
                kickback = Vector3.zero;
            }
            else {
                engine.OverheatFromThrust(mitigation);
                kickback += mitigation;
            }
            thrustVelocity += kickback;
        }
    }

    public void CalculateTotalThrust () {
        if (mainThrusterActive) {
            Vector3 deltaThrust = engine.GetDeltaThrust(mass);
            thrustVelocity += deltaThrust;
            engine.OverheatFromThrust(deltaThrust);
        }
        if (thrustVelocity.magnitude > maxSpeed) {
            thrustVelocity = thrustVelocity.normalized * maxSpeed;
        }
    }

    public void AdjustThrustFromFriction () {
        // left rcs
        float leftThrustComponent = Vector3.Dot(thrustVelocity, -transform.right);
        if (leftThrustComponent > 0.01f) {
            Vector3 leftThrust = leftSideThruster.GetDeltaThrust(mass);
            if (leftThrust.magnitude > leftThrustComponent) {
                leftThrust = leftThrust.normalized * leftThrustComponent;
            }
            thrustVelocity += leftThrust;
            leftSideThruster.TurnOnParticles();
        }
        else {
            leftSideThruster.TurnOffParticles();
        }
        // right rcs
        float rightThrustComponent = Vector3.Dot(thrustVelocity, transform.right);
        if (rightThrustComponent > 0.01f) {
            Vector3 rightThrust = rightSideThruster.GetDeltaThrust(mass);
            if (rightThrust.magnitude > rightThrustComponent) {
                rightThrust = rightThrust.normalized * rightThrustComponent;
            }
            thrustVelocity += rightThrust;
            rightSideThruster.TurnOnParticles();
        }
        else {
            rightSideThruster.TurnOffParticles();
        }
        // forward rcs
        float frontThrustComponent = Vector3.Dot(thrustVelocity, transform.forward);
        if (frontThrustComponent > 0.01f && !mainThrusterActive) {
            Vector3 frontThrust = frontLeftThruster.GetDeltaThrust(mass) + frontRightThruster.GetDeltaThrust(mass);
            if (frontThrust.magnitude > frontThrustComponent) {
                frontThrust = frontThrust.normalized * frontThrustComponent;
            }
            thrustVelocity += frontThrust;
            frontLeftThruster.TurnOnParticles();
            frontRightThruster.TurnOnParticles();
        }
        else {
            frontLeftThruster.TurnOffParticles();
            frontRightThruster.TurnOffParticles();
        }
    }

    public void MoveFromThrust() {
        transform.position += thrustVelocity * Time.fixedDeltaTime;
    }

    public void ActivateThruster () {
        engine.TurnOnParticles();
        mainThrusterActive = true;
    }

    public void DeactivateThruster () {
        engine.TurnOffParticles();
        mainThrusterActive = false;
    }

    public void ActivateWeapon () {
        mainWeaponActive = true;
    }

    public void DeactivateWeapon () {
        mainWeaponActive = false;
    }

}