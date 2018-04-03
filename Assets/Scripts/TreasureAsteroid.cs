using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TreasureAsteroid : Asteroid {

    public LootTable lootTable;

    public override void Kill () {
        if (isShiny && !destroyed) {
            SpawnPowerUp();
        }
        base.Kill();
    }

    void SpawnPowerUp () {
        PowerUp choice = lootTable.GetLoot(1).FirstOrDefault();
        if (choice != null) {
            Instantiate(choice, transform.position, Quaternion.identity);
        }
    }

}
