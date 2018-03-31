using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageNumber : FloatingText {

    public void SetDamage (int dmg) {
        SetString(string.Format("{0}", dmg.ToString()));
    }

}
