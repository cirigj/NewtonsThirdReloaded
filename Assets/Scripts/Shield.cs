using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShieldModifiers {
    ShieldOvercharger = 1,
    ReverseMagneticField = 2,
    OrganicShieldProjector = 4,
}

public class Shield : MonoBehaviour {

    [Header("Shield Stats")]
    public float maxHealth;
    public float rechargeRate;
    public float shieldHitCooldownPenalty;
    public float shieldDownCooldownPenalty;

    [Header("Visual")]
    public float shieldMaxOpacity;
    public Renderer shieldRenderer;

    [Header("Runtime")]
    public float health;
    public float rechargeCooldown;

    void Update () {
        Regenerate();
        SetShieldOpacity();
    }

    void SetShieldOpacity () {
        if (shieldRenderer != null) {
            shieldRenderer.material.SetFloat("_Alpha", (health / maxHealth) * shieldMaxOpacity);
        }
    }

    public virtual void Regenerate () {
        if (rechargeCooldown > 0f) {
            rechargeCooldown = Mathf.Clamp(rechargeCooldown - Time.fixedDeltaTime, 0f, Mathf.Max(shieldHitCooldownPenalty, shieldDownCooldownPenalty));
        }
        if (rechargeCooldown == 0f) {
            health = Mathf.Clamp(health + rechargeRate * Time.fixedDeltaTime, 0f, maxHealth);
        }
    }

    public virtual void TakeDamage (float dmg) {
        health = Mathf.Clamp(health - dmg, 0f, maxHealth);
        rechargeCooldown = health == 0 ? shieldDownCooldownPenalty : shieldHitCooldownPenalty;
    }

}
