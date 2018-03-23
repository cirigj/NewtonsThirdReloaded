using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorPickup : PowerUp {

    public float extraArmor;

    public override void OnPickup (Ship ship) {
        ship.AddArmor(extraArmor);
    }

}
