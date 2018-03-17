using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JBirdEngine;

[System.Serializable]
public class SpawnFrequency {
    public int targetNumber;
    public ISpawnable prefab;
}

public class Spawner : ISpawner {

    SpawnController master;

    public Ship target;

    public float startDistanceMin;
    public float startDistanceMax;

    public SpawnFrequency spawnFrequency;

    public float spawnDistanceMin;
    public float spawnDistanceMax;
    public float despawnDistance;

    List<ISpawnable> objects;

    public float spawnTime;
    public int spawnRetries;

    float spawnTimer;
    Coroutine spawnRoutine;

    void Awake () {
        master = GetComponentInParent<SpawnController>();
        spawnTimer = 0f;
        objects = new List<ISpawnable>();
    }

    void Start () {
        spawnRoutine = StartCoroutine(SpawnObjects(startDistanceMin, startDistanceMax, false));
    }

    void Update () {
        spawnTimer += Time.fixedDeltaTime;
        if (spawnTimer >= spawnTime) {
            DespawnObjects();
            if (spawnRoutine == null) {
                spawnRoutine = StartCoroutine(SpawnObjects(spawnDistanceMin, spawnDistanceMax, true));
                spawnTimer = 0f;
            }
            else {
                spawnTimer = spawnTime;
            }
        }
    }

    public override List<ISpawnable> GetList () {
        return objects;
    }

    public override ISpawner GetBreakApartParent () {
        return master.GetBreakApartMapping(this);
    }

    public override List<ISpawnable> GetMasterList () {
        return master.GetList();
    }

    IEnumerator SpawnObjects (float min, float max, bool stagger) {
        while (objects.Count < spawnFrequency.targetNumber) {
            ISpawnable newObject = SpawnObject(min, max, spawnFrequency.prefab);
            if (newObject != null) {
                objects.Add(newObject);
            }
            if (stagger) {
                yield return null;
            }
        }
        spawnRoutine = null;
        yield break;
    }

    ISpawnable SpawnObject (float min, float max, ISpawnable prefab) {
        Vector3 pos = Vector3.zero;
        int tries;
        for (tries = 0; tries < spawnRetries; ++tries) {
            pos = Random.onUnitSphere;
            pos = new Vector3(pos.x, 0f, pos.z).normalized * (Random.Range(min, max) + prefab.GetSpawnRadius());
            pos += target.transform.position;
            if (CheckPositionValidity(pos, prefab.GetSpawnRadius())) {
                break;
            }
        }
        if (tries == spawnRetries) {
            return null;
        }
        ISpawnable spawn = Instantiate(prefab, pos, Random.rotation);
        spawn.SetParent(this);
        return spawn;
    }

    bool CheckPositionValidity (Vector3 pos, float size) {
        foreach (var obj in GetMasterList()) {
            if (Vector3.Distance(pos, obj.transform.position) <= size + obj.GetSpawnRadius()) {
                return false;
            }
        }
        return true;
    }

    bool ObjectOutisdeRange (ISpawnable despawn) {
        return Vector3.Distance(despawn.transform.position, target.transform.position) > despawnDistance;
    }

    void DespawnObjects () {
        for (int i = 0; i < objects.Count; ++i) {
            if (ObjectOutisdeRange(objects[i])) {
                objects[i].Despawn(false);
                objects.RemoveAt(i);
                --i;
            }
        }
    }

    public override void AddObject (ISpawnable child) {
        objects.Add(child);
    }

    public override void RemoveObject (ISpawnable despawn) {
        for (int i = 0; i < objects.Count; ++i) {
            if (objects.Contains(despawn)) {
                objects.Remove(despawn);
                return;
            }
        }
    }

}
