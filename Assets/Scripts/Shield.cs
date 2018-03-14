using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShieldModifiers {
    ShieldOvercharger = 2<<0,
    ReverseMagneticField = 2<<1,
    OrganicShieldProjector = 2<<2,
}

public class Shield : MonoBehaviour {

    [Header("Shield Stats")]
    public float maxHealth;
    public float rechargeRate;
    public float shieldHitCooldownPenalty;
    public float shieldDownCooldownPenalty;

    [Header("Runtime")]
    public float health;
    public float rechargeCooldown;

}
