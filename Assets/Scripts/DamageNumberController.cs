using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JBirdEngine;

public class DamageNumberController : MonoBehaviour {

    public static DamageNumberController instance;

    public DamageNumber numberPrefab;
    public Gradient defenseGradient;
    public Gradient playerDefenseGradient;
    public AnimationCurve sizeRamp;
    public float sizeMin;
    public float sizeMax;
    public float valueClampMin;
    public float valueClampMax;
    public float randomDistance;
    public float yDistance;
    public Vector3 eulers;

    void Awake () {
        Singleton.Manage(this, ref instance);
    }

    public void SpawnDamageNumber (float dmg, float dmgReduction, Vector3 pos, bool isPlayer = false) {
        Vector3 randomPos = pos + (Random.onUnitSphere * randomDistance).SetY(yDistance);
        DamageNumber newNumber = Instantiate(numberPrefab, randomPos, Quaternion.Euler(eulers));
        newNumber.SetColor((isPlayer ? playerDefenseGradient : defenseGradient).Evaluate((dmgReduction + 1f) / 2f));
        newNumber.SetDamage(Mathf.CeilToInt(dmg));
        float sizeT = (Mathf.Clamp(dmg, valueClampMin, valueClampMax) - valueClampMin) / (valueClampMax - valueClampMin);
        newNumber.SetSize(sizeRamp.Evaluate(sizeT) * (sizeMax - sizeMin) + sizeMin);
    }
	
}
