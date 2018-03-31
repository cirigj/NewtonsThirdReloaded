using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterLifetime : MonoBehaviour {

    public float lifetime = 1f;

	void Start () {
        StartCoroutine(DestroyAfterWait());
    }

    IEnumerator DestroyAfterWait () {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

}
