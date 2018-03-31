using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JBirdEngine;

public class FloatingTextController : MonoBehaviour {

    public FloatingText textPrefab;
    public Color defaultColor;
    public float yDistance;
    public Vector3 eulers;

    [Header("Damage Numbers")]
    public DamageNumber numberPrefab;
    public Gradient defenseGradient;
    public Gradient playerDefenseGradient;
    public float sizeMin;
    public float sizeMax;
    public AnimationCurve sizeRamp;
    public float valueClampMin;
    public float valueClampMax;
    public float randomDistance;

    public DamageNumber SpawnDamageNumber (float dmg, float dmgReduction, Vector3 pos, bool isPlayer = false) {
        Vector3 randomPos = pos + (Random.onUnitSphere * randomDistance).SetY(yDistance);
        DamageNumber newNumber = Instantiate(numberPrefab, randomPos, Quaternion.Euler(eulers));
        newNumber.SetColor((isPlayer ? playerDefenseGradient : defenseGradient).Evaluate((dmgReduction + 1f) / 2f));
        newNumber.SetDamage(Mathf.CeilToInt(dmg));
        float sizeT = (Mathf.Clamp(dmg, valueClampMin, valueClampMax) - valueClampMin) / (valueClampMax - valueClampMin);
        newNumber.SetSize(sizeRamp.Evaluate(sizeT) * (sizeMax - sizeMin) + sizeMin);
        return newNumber;
    }

    public FloatingText SpawnText (string str, Vector3 pos, Color color, int? fontSize = null, float? lifetime = null, Transform parent = null) {
        FloatingText newText = Instantiate(textPrefab, pos.SetY(yDistance), Quaternion.Euler(eulers));
        if (lifetime.HasValue) {
            newText.lifeTime = lifetime.Value;
        }
        if (fontSize.HasValue) {
            newText.SetSize(fontSize.Value);
        }
        if (parent != null) {
            newText.SetParent(parent);
        }
        newText.SetString(str);
        newText.SetColor(color);
        return newText;
    }

    public FloatingText SpawnText (string str, Vector3 pos, int? fontSize = null, float? lifetime = null) {
        return SpawnText(str, pos, defaultColor, fontSize, lifetime);
    }
	
}
