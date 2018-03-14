using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public float lifetime;
    public float life;
    public float damage;
    public float mass;
    public Vector3 velocity;

    void FixedUpdate () {
        transform.position += velocity * Time.fixedDeltaTime;
        life += Time.fixedDeltaTime;
        if (life >= lifetime) {
            Kill();
        }
    }

    void Kill () {
        Destroy(gameObject);
    }

}
