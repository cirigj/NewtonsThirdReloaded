using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class BreakApartMapping {
    public ISpawner parentSpawner;
    public ISpawner childSpawner;
}

public class SpawnController : MonoBehaviour {

    public List<ISpawner> spawners;
    public List<BreakApartMapping> breakApartMappings;

    public List<ISpawnable> GetList () {
        return spawners.SelectMany(s => s.GetList()).ToList();
    }

    public ISpawner GetBreakApartMapping (ISpawner parent) {
        BreakApartMapping mapping = breakApartMappings.FirstOrDefault(m => m.parentSpawner == parent);
        if (mapping == null) {
            return parent;
        }
        return mapping.childSpawner;
    }

}
