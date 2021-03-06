﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JBirdEngine;

public class Asteroid : ISpawnable, IShootable, ICollidable, IKillable {

    [Header("Movement Stats")]
    public float rotationMultiplier;
    public float startSpeed;
    public float maxSpeed;

    [Header("Stats")]
    public float health;
    public float mass;
    public float radius;
    public float projectileDamageReduction;

    [Header("Breakapart")]
    public Asteroid breakApartPrefab;
    public float breakApartMin;
    public float breakApartMax;
    public float breakApartSpeedMultiplierMin;
    public float breakApartSpeedMultiplierMax;
    public float breakApartDistanceMultiplier;
    public float breakApartShinyChanceMultiplier;
    public float breakApartShinyChainChance;

    [Header("Visual")]
    public ParticleSystem particles;
    public float destroyWaitTime;
    public GameObject model;

    [Header("Runtime")]
    public Vector3 velocity;
    public Quaternion rotationSpeed;
    public bool startSpeedSet;
    public ISpawner parent;
    public int firstIndex;

    [Header("Collisions")]
    public float damageModifier;
    public float damageReductionModifier;
    public float elasticity;
    public Collider collider;
    public SoundHandler damageSound;

    [Header("Treasure")]
    [Range(0f, 1f)]
    public float shinyChance;

    public Renderer asteroidRenderer;
    public Material normalMaterial;

    public Material shinyMaterial;
    public bool isShiny;
    public ParticleSystem sparkles;

    public float extraHealth;

    protected bool destroyed;

    protected virtual void Start () {
        if (Random.Range(0f, 1f) < shinyChance) {
            SetShiny();
        }
        if (collider == null) {
            collider = GetComponent<Collider>();
        }
        if (!startSpeedSet) {
            velocity = Random.onUnitSphere.SetY(0f) * startSpeed;
            startSpeedSet = true;
        }
        rotationSpeed = Quaternion.Slerp(Quaternion.identity, Random.rotation, Time.fixedDeltaTime * rotationMultiplier);
    }

    void SetShiny () {
        sparkles.Play();
        asteroidRenderer.material = shinyMaterial;
        isShiny = true;
        health += extraHealth;
    }

    void FixedUpdate () {
        if (!destroyed) {
            Move();
        }
    }

    void Move () {
        transform.position += velocity * Time.fixedDeltaTime;
        transform.rotation *= rotationSpeed;
    }

    public void Interact (Projectile proj) {
        if (!destroyed) {
            proj.Contact(this);
            TakeRecoil(proj.velocity * (proj.mass / mass));
            float dmg = CalculateProjectileDamageReduction(proj.damage);
            TakeDamage(dmg, true, proj.transform.position);
        }
    }

    public virtual void TakeDamage (float dmg, bool fromProjectile, Vector3 dmgPos) {
        if (fromProjectile || Mathf.RoundToInt(dmg) > 0) {
            GameController.Instance.textController.SpawnDamageNumber(dmg, fromProjectile ? projectileDamageReduction : damageReductionModifier, dmgPos);
        }
        TakeDamage(dmg);
    }

    public void TakeDamage (float dmg) {
        health = Mathf.Clamp(health - dmg, 0f, health);
        if (health == 0f) {
            Kill();
        }
    }

    void TakeRecoil (Vector3 recoil) {
        velocity += recoil;
        if (velocity.magnitude > maxSpeed) {
            velocity = velocity.normalized * maxSpeed;
        }
    }

    public override float GetSpawnRadius () {
        return radius;
    }

    public override ISpawner GetParent () {
        return parent;
    }

    public override void SetParent (ISpawner p) {
        parent = p;
    }

    public override void PostDespawn (bool calledParent) {
        if (calledParent) {
            Explode();
        }
        else {
            Destroy(gameObject);
        }
    }

    void Explode () {
        model.SetActive(false);
        ParticleSystem explosion = Instantiate(particles, transform.position, Quaternion.identity);
        explosion.Play();
        SoundHandler sound = explosion.GetComponent<SoundHandler>();
        if (sound) sound.Play();
        Destroy(gameObject);
    }

    public virtual void Kill () {
        if (!destroyed) {
            destroyed = true;
            int breakApartNumber = Mathf.FloorToInt(Random.Range(breakApartMin, breakApartMax));
            if (breakApartNumber > 0 && breakApartPrefab != null) {
                float radius = breakApartPrefab.radius;
                float startAngle = Random.Range(0f, 360f);
                for (int i = 0; i < breakApartNumber; ++i) {
                    Vector3 pos = VectorHelper.FromAzimuthAndElevation(startAngle + i * (360f / breakApartNumber), 0f) * breakApartPrefab.radius * breakApartDistanceMultiplier;
                    Asteroid child = Instantiate(breakApartPrefab, transform.position + pos, Random.rotation);
                    child.velocity = velocity + pos * Random.Range(breakApartSpeedMultiplierMin, breakApartSpeedMultiplierMax);
                    child.parent = parent.GetBreakApartParent();
                    child.parent.AddObject(child);
                    child.startSpeedSet = true;
                    if (i == 0 && isShiny) {
                        child.shinyChance = 1f;  //guarantee at least one shiny child asteroid
                    }
                    else {
                        child.shinyChance = isShiny ? breakApartShinyChainChance : child.shinyChance * breakApartShinyChanceMultiplier;
                    }
                }
            }
            Destroy(collider);
            Despawn(true);
        }
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

    public float GetCollisionRadius () {
        return radius;
    }

    public float GetMass () {
        return mass;
    }

    public Vector3 GetVelocity () {
        return velocity;
    }

    public void SetVelocity (Vector3 vel) {
        velocity = vel;
        if (velocity.magnitude > maxSpeed) {
            velocity = velocity.normalized * maxSpeed;
        }
    }

    public Vector3 GetPosition () {
        return transform.position;
    }

    public void SetPosition (Vector3 pos) {
        transform.position = pos;
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
