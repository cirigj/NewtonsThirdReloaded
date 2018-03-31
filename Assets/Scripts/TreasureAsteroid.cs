using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TreasureAsteroid : Asteroid {

    [Header("Treasure")]
    [Range(0f, 1f)]
    public float percentChance;

    public Renderer asteroidRenderer;
    public Material normalMaterial;

    public Material shinyMaterial;
    public bool isShiny;
    public ParticleSystem sparkles;

    public float extraHealth;

    public LootTable lootTable;

    protected override void Start () {
        if (Random.Range(0f, 1f) < percentChance) {
            SetShiny();
        }
        base.Start();
    }

    void SetShiny () {
        sparkles.Play();
        asteroidRenderer.material = shinyMaterial;
        isShiny = true;
        health += extraHealth;
    }

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
