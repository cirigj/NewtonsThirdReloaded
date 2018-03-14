using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponModifiers {
    SpreadNozzle = 2<<0,
    SideMounts = 2<<1,
    QuantumFluctuator = 2<<2,
    ExplosiveCharges = 2<<3,
    HeatSeekingNanobots = 2<<4,
}

public class Weapon : MonoBehaviour {

    [Header("Weapon Stats")]
    public float fireRate;
    public float recoilPercentage;

    [Header("Projectile Stats")]
    public float projectileSpeed;
    public float projectileDamage;
    public float projectileMass;
    public float projectileLifetime;

    [Header("Instantiation")]
    public Vector3 muzzleOffset;
    public Projectile projectilePrefab;

    [Header("Runtime")]
    public float cooldown;

    void Update () {
        if (cooldown > 0) {
            cooldown = Mathf.Clamp(cooldown - Time.fixedDeltaTime, 0f, 100f);
        }
    }

    public Vector3 TryFire (float shipMass) {
        if (cooldown == 0f) {
            Fire();
            return GetKickback(shipMass);
        }
        else return Vector3.zero;
    }

    public void Fire () {
        Projectile proj = Instantiate(projectilePrefab, transform.position + (transform.rotation * muzzleOffset), transform.rotation);
        proj.velocity = transform.forward * projectileSpeed;
        proj.lifetime = projectileLifetime;
        proj.damage = projectileDamage;
        proj.mass = projectileMass;
        cooldown = 1f / fireRate;
    }

    public Vector3 GetKickback (float shipMass) {
        return -transform.forward * (projectileSpeed * projectileMass / shipMass) * recoilPercentage * Time.fixedDeltaTime;
    }

}
