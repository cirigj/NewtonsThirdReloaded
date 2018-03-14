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
    public float projectileSpeed;
    public float projectileDamage;
    public float projectileMass;
    public float recoilPercentage;

}
