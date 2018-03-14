﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovement : MonoBehaviour {

    public float speed;
    public float rotSpeed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.W)) {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A)) {
            transform.Rotate(0, rotSpeed * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.D)) {
            transform.Rotate(0, -rotSpeed * Time.deltaTime, 0);
        }
    }
}
