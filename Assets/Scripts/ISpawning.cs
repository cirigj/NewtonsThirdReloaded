using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ISpawner : MonoBehaviour {

    public abstract List<ISpawnable> GetList ();
    public abstract List<ISpawnable> GetMasterList ();
    public abstract void AddObject (ISpawnable obj);
    public abstract void RemoveObject (ISpawnable obj);
    public abstract ISpawner GetBreakApartParent ();

}

public abstract class ISpawnable : MonoBehaviour {

    public abstract float GetSpawnRadius ();
    public abstract ISpawner GetParent ();
    public abstract void SetParent (ISpawner parent);

    public void Despawn (bool callParent) {
        if (callParent) {
            GetParent().RemoveObject(this);
        }
        PostDespawn(callParent);
    }
    public abstract void PostDespawn (bool calledParent);

}
