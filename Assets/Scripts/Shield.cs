using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShieldModifiers {
    ShieldOvercharger = 1,
    ReverseMagneticField = 2,
    OrganicShieldProjector = 4,
}

public class Shield : MonoBehaviour, IShootable, ICollidable {

    public Ship ship;

    public Transform root;

    [Header("Shield Stats")]
    public float maxHealth;
    public float rechargeRate;
    public float shieldHitCooldownPenalty;
    public float shieldDownCooldownPenalty;
    public float projectileDamageReduction;

    [Header("Visual")]
    public float shieldMaxOpacity;
    public Renderer shieldRenderer;

    [Header("Runtime")]
    public float health;
    public float rechargeCooldown;

    [Header("Collisions")]
    public Collider collider;
    public float damageModifier;
    public float damageReductionModifier;
    public float elasticity;

    void Start () {
        if (collider == null) {
            collider = GetComponent<Collider>();
        }
    }

    void Update () {
        Regenerate();
        SetShieldOpacity();
        SetColliderState();
    }

    protected virtual void SetColliderState () {
        collider.enabled = health > 0f;
    }

    void SetShieldOpacity () {
        if (shieldRenderer != null) {
            shieldRenderer.material.SetFloat("_Alpha", (health / maxHealth) * shieldMaxOpacity * (ship.cloaked ? ship.cloakShieldAlphaModifier : 1f));
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

    public void Interact (Projectile proj) {
        proj.Contact(this);
        float dmg = CalculateProjectileDamageReduction(proj.damage);
        TakeDamage(dmg, true, proj.transform.position);
    }

    public virtual void TakeDamage (float dmg, bool fromProjectile, Vector3 dmgPos) {
        if (fromProjectile || Mathf.RoundToInt(dmg) > 0) {
            GameController.instance.dmgNumController.SpawnDamageNumber(dmg, fromProjectile ? projectileDamageReduction : damageReductionModifier, dmgPos, ship.shipLayer == Layers.PlayerShip);
        }
        TakeDamage(dmg);
    }

    public virtual void TakeDamage (float dmg) {
        health = Mathf.Clamp(health - dmg, 0f, maxHealth);
        rechargeCooldown = health == 0 ? shieldDownCooldownPenalty : shieldHitCooldownPenalty;
    }

    void OnTriggerEnter (Collider other) {
        ICollidable collidable = other.GetComponent<ICollidable>();
        if (collidable != null) {
            this.HandleCollision(collidable);
        }
    }

    void OnTriggerStay (Collider other) {
        ICollidable collidable = other.GetComponent<ICollidable>();
        if (collidable != null) {
            this.HandleCollision(collidable);
        }
    }

    public float GetMass () {
        return ship.mass;
    }

    public Vector3 GetVelocity () {
        return ship.thrustVelocity;
    }

    public void SetVelocity (Vector3 vel) {
        ship.thrustVelocity = vel;
        ship.ClampVelocity();
    }

    public Vector3 GetPosition () {
        return ship.transform.position;
    }

    public float GetElasticity () {
        return elasticity;
    }

    public float GetDamage (float momentumDiff) {
        return momentumDiff * damageModifier;
    }

    public float CalculateCollisionDamageReduction (float dmg) {
        return Mathf.Clamp(dmg - dmg * damageReductionModifier, 0f, Mathf.Infinity);
    }

    public float CalculateProjectileDamageReduction (float dmg) {
        return Mathf.Clamp(dmg - dmg * projectileDamageReduction, 0f, Mathf.Infinity);
    }

}
