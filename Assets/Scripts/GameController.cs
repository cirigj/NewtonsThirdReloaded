using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JBirdEngine;

public class GameController : MonoBehaviour {

    public static GameController instance;

    public Ship playerShip;
    public DamageNumberController dmgNumController;

    void Awake () {
        Singleton.Manage(this, ref instance);
    }

}
