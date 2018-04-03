using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JBirdEngine;

public class GameController : MonoBehaviour {

    public static GameController Instance;

    public static float armorDamageReduction = 0.5f;

    public Ship playerShip;
    public FloatingTextController textController;
    public ShipUIController shipUI;
    public GameOverController gameOverController;
    public SoundController soundController;

    void Awake () {
        Singleton.Manage(this, ref Instance);
    }

    public void GameOver () {
        shipUI.SetActive(false);
        gameOverController.FadeIn();
    }

    public void RestartGame () {
        UnityEngine.SceneManagement.SceneManager.LoadScene("TestScene");
    }

}
