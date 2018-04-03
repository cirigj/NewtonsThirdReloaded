using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : PowerUp {

    public float healthRestore;

    public override void OnPickup (Ship ship) {
        base.OnPickup(ship);
        ship.RepairDamage(healthRestore);
    }

}
