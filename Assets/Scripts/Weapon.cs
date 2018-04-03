using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public enum WeaponModifiers {
    SpreadNozzle = 1,
    SideMounts = 2,
    QuantumFluctuator = 4,
    ExplosiveCharges = 8,
    HeatSeekingNanobots = 16,
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

    [Header("Sound")]
    public SoundHandler sound;

    [Header("Runtime")]
    public float cooldown;

    void Update () {
        if (cooldown > 0) {
            cooldown = Mathf.Clamp(cooldown - Time.fixedDeltaTime, 0f, 100f);
        }
    }

    public Vector3 TryFire (float shipMass, Layers bulletLayer, float dmgMultiplier) {
        if (cooldown == 0f) {
            Fire(bulletLayer, dmgMultiplier);
            return GetKickback(shipMass);
        }
        else return Vector3.zero;
    }

    public void Fire (Layers bulletLayer, float dmgMultiplier) {
        Projectile proj = Instantiate(projectilePrefab, transform.position + (transform.rotation * muzzleOffset), transform.rotation);
        proj.velocity = transform.forward * projectileSpeed;
        proj.lifetime = projectileLifetime;
        proj.damage = projectileDamage * dmgMultiplier;
        proj.mass = projectileMass;
        proj.gameObject.layer = Convert.ToInt32(bulletLayer);
        cooldown = 1f / fireRate;
        sound.Play();
    }

    public Vector3 GetKickback (float shipMass) {
        return -transform.forward * (projectileSpeed * projectileMass / shipMass) * recoilPercentage * Time.fixedDeltaTime;
    }

}
