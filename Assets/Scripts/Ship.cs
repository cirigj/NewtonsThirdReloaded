using System.Collections;
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

    [Header("Anchor Points")]
    public PartAnchor mainWeaponAnchor;
    public PartAnchor spreadWeaponAnchor;
    public PartAnchor sideWeaponAnchor;
    public PartAnchor engineAnchor;
    public PartAnchor shieldAnchor;

    [Header("Easing")]
    [Range(0.01f,1f)]
    public float yawEasing;

    [Header("Runtime")]
    public Vector2 targetYaw;

    void Update () {
        MoveTowardsTargetYaw();
    }

    public void SetTargetYaw (Vector2 yaw) {
        targetYaw = yaw.normalized;
    }

    public void MoveTowardsTargetYaw () {
        Vector3 target = new Vector3(targetYaw.x, 0, targetYaw.y).normalized;
        float azimuth = transform.forward.GetAzimuth(transform.position, Vector3.up, target);
        if (azimuth > 180f) {
            azimuth -= 360f;
        }
        float easeAngle = azimuth * yawEasing;
        Vector3 easeTarget = VectorHelper.FromAzimuthAndElevation(easeAngle, 0f, Vector3.up, transform.forward).normalized;
        if (easeTarget.magnitude != 0f) {
            transform.rotation = Quaternion.LookRotation(VectorHelper.FromAzimuthAndElevation(easeAngle, 0f, Vector3.up, transform.forward), Vector3.up);
        }
    }

}
