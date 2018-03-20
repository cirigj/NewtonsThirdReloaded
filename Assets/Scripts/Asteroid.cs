using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JBirdEngine;

public class Asteroid : ISpawnable, IShootable, ICollidable {

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

    bool destroyed;

    void Start () {
        if (collider == null) {
            collider = GetComponent<Collider>();
        }
        if (!startSpeedSet) {
            velocity = Random.onUnitSphere.SetY(0f) * startSpeed;
            startSpeedSet = true;
        }
        rotationSpeed = Quaternion.Slerp(Quaternion.identity, Random.rotation, Time.fixedDeltaTime * rotationMultiplier);
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
            DamageNumberController.instance.SpawnDamageNumber(dmg, fromProjectile ? projectileDamageReduction : damageReductionModifier, dmgPos);
        }
        TakeDamage(dmg);
    }

    public void TakeDamage (float dmg) {
        if (!destroyed) {
            health = Mathf.Clamp(health - dmg, 0f, health);
            if (health == 0f) {
                Kill();
            }
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

    public override void Despawn (bool callParent) {
        if (callParent) {
            parent.RemoveObject(this);
            StartCoroutine(DespawnAfterParticles());
        }
        else {
            Destroy(gameObject);
        }
    }

    IEnumerator DespawnAfterParticles () {
        model.SetActive(false);
        particles.Play();
        destroyed = true;
        yield return new WaitForSeconds(destroyWaitTime);
        Destroy(gameObject);
    }

    void Kill () {
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
            }
        }
        Destroy(collider);
        Despawn(true);
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

    public Vector3 GetPosition() {
        return transform.position;
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
