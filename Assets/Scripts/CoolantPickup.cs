﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolantPickup : PowerUp {

    public float extraCoolant;

    public override void OnPickup (Ship ship) {
        ship.engine.AddCoolant(extraCoolant);
    }

}
