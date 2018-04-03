using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public float lifetime;
    public float life;
    public float damage;
    public float mass;
    public Vector3 velocity;

    [Header("Visual")]
    public Renderer bulletRenderer;
    public Material playerBulletMat;
    public Material enemyBulletMat;
    public Light glowLight;
    public Color playerGlowColor;
    public Color enemyGlowColor;

    void Start () {
        if (gameObject.layer == Convert.ToInt32(Layers.PlayerBullet)) {
            bulletRenderer.material = playerBulletMat;
            glowLight.color = playerGlowColor;
        }
        if (gameObject.layer == Convert.ToInt32(Layers.EnemyBullet)) {
            bulletRenderer.material = enemyBulletMat;
            glowLight.color = enemyGlowColor;
        }
    }

    void FixedUpdate () {
        transform.position += velocity * Time.fixedDeltaTime;
        life += Time.fixedDeltaTime;
        if (life >= lifetime) {
            Kill();
        }
    }

    void OnTriggerEnter(Collider other) {
        IShootable shootable = other.GetComponent<IShootable>();
        if (shootable != null) {
            shootable.Interact(this);
        }
    }

    public void Contact (IShootable other) {
        Kill();
    }

    void Kill () {
        Destroy(gameObject);
    }

}
