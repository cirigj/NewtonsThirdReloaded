using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class CloakMaterial {

    public Renderer renderer;
    public Material normalMat;
    public Material cloakedMat;

    public void Cloak () {
        renderer.material = cloakedMat;
    }

    public void Uncloak () {
        renderer.material = normalMat;
    }

}

[System.Serializable]
public class CloakParticles {

    public ParticleSystemRenderer particleSystem;
    public Material normalMat;
    public Material cloakedMat;

    public void Cloak () {
        particleSystem.material = cloakedMat;
    }

    public void Uncloak () {
        particleSystem.material = normalMat;
    }

}

public class CloakVisualHandler : MonoBehaviour {

    public List<CloakMaterial> materials;
    public List<CloakParticles> particles;
    public List<GameObject> hideObjects;

    public void Cloak () {
        materials.ForEach(m => m.Cloak());
        particles.ForEach(p => p.Cloak());
        hideObjects.ForEach(o => o.SetActive(false));
    }

    public void Uncloak () {
        materials.ForEach(m => m.Uncloak());
        particles.ForEach(p => p.Uncloak());
        hideObjects.ForEach(o => o.SetActive(true));
    }

}
