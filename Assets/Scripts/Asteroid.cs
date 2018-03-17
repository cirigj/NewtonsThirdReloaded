using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JBirdEngine;

public class Asteroid : ISpawnable, IShootable {

    [Header("Movement Stats")]
    public float rotationMultiplier;
    public float startSpeed;
    public float maxSpeed;

    [Header("Stats")]
    public float health;
    public float mass;
    public float radius;

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
    public Vector3 movement;
    public Quaternion rotationSpeed;
    public bool startSpeedSet;
    public ISpawner parent;
    public int firstIndex;

    bool destroyed;

    void Start () {
        if (!startSpeedSet) {
            movement = Random.onUnitSphere;
            movement = new Vector3(movement.x, 0f, movement.z) * startSpeed;
            startSpeedSet = true;
        }
        rotationSpeed = Quaternion.Slerp(Quaternion.identity, Random.rotation, Time.fixedDeltaTime * rotationMultiplier);
    }

    void Update () {
        Move();
    }

    void Move () {
        transform.position += movement * Time.fixedDeltaTime;
        transform.rotation *= rotationSpeed;
    }

    public void Interact (Projectile proj) {
        if (!destroyed) {
            proj.Contact(this);
            TakeRecoil(proj.velocity * (proj.mass / mass));
            TakeDamage(proj.damage);
        }
    }

    void TakeDamage (float dmg) {
        health = Mathf.Clamp(health - dmg, 0f, health);
        if (health == 0f) {
            Kill();
        }
    }

    void TakeRecoil (Vector3 recoil) {
        movement += recoil;
        if (movement.magnitude > maxSpeed) {
            movement = movement.normalized * maxSpeed;
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
        }
        StartCoroutine(DespawnAfterParticles());
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
                child.movement = movement + pos * Random.Range(breakApartSpeedMultiplierMin, breakApartSpeedMultiplierMax);
                child.parent = parent.GetBreakApartParent();
                child.parent.AddObject(child);
                child.startSpeedSet = true;
            }
        }
        Despawn(true);
    }

}
