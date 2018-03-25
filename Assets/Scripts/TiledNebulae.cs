using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiledNebulae : MonoBehaviour {

    public Camera targetCamera;

    public float tileWidth;
    public float tileHeight;

    void FixedUpdate () {
        RezeroGrid();
    }

    void RezeroGrid () {
        if (targetCamera.transform.position.x - transform.position.x > tileWidth) {
            transform.position = transform.position + Vector3.right * tileWidth;
        }
        if (targetCamera.transform.position.x - transform.position.x < -tileWidth) {
            transform.position = transform.position + Vector3.left * tileWidth;
        }
        if (targetCamera.transform.position.z - transform.position.z > tileHeight) {
            transform.position = transform.position + Vector3.forward * tileHeight;
        }
        if (targetCamera.transform.position.z - transform.position.z < -tileHeight) {
            transform.position = transform.position + Vector3.back * tileHeight;
        }
    }

}
