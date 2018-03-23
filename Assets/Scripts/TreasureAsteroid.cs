using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TreasureAsteroid : Asteroid {

    [System.Serializable]
    public class PowerUpChance {
        public PowerUp prefab;
        public float chanceMin;
        public float chanceMax;
    }

    [Header("Treasure")]
    [Range(0f, 1f)]
    public float percentChance;

    public Renderer asteroidRenderer;
    public Material normalMaterial;

    public Material shinyMaterial;
    public bool isShiny;
    public ParticleSystem sparkles;

    public float extraHealth;

    public List<PowerUpChance> powerUps;

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

    protected override void Kill () {
        if (isShiny && !destroyed) {
            SpawnPowerUp();
        }
        base.Kill();
    }

    void SpawnPowerUp () {
        float rand = Random.Range(0f, 1f);
        PowerUpChance choice = powerUps.Where(pc => rand >= pc.chanceMin && rand <= pc.chanceMax).FirstOrDefault();
        if (choice != null) {
            Instantiate(choice.prefab, transform.position, Quaternion.identity);
        }
        else {
            Debug.LogErrorFormat("PowerUp chance '{0}' yields no result!", rand);
        }
    }

}
