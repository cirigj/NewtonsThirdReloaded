using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {

    bool pickedUp;

    public void OnTriggerEnter (Collider other) {
        Ship ship = other.GetComponent<Ship>();
        if (ship != null) {
            if (!pickedUp) {
                pickedUp = true;
                OnPickup(ship);
                Kill();
                return;
            }
        }
        HullShield shield = other.GetComponent<HullShield>();
        if (shield != null) {
            if (!pickedUp) {
                pickedUp = true;
                OnPickup(shield.ship);
                Kill();
                return;
            }
        }
    }

    public virtual void OnPickup (Ship ship) {
        throw new System.NotImplementedException();
    }

    void Kill () {
        GetComponent<Collider>().enabled = false;
        Destroy(gameObject);
    }

}
