using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JBirdEngine;

public class GameController : MonoBehaviour {

    public static GameController instance;

    public Ship playerShip;
    public DamageNumberController dmgNumController;
    public ShipUIController shipUI;
    public GameOverController gameOverController;

    void Awake () {
        Singleton.Manage(this, ref instance);
    }

    public void GameOver () {
        shipUI.SetActive(false);
        gameOverController.FadeIn();
    }

}
