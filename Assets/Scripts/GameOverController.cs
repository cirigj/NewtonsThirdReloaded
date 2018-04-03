using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JBirdEngine.ColorLibrary;

public class GameOverController : MonoBehaviour {

    public List<Graphic> gameOverGraphics;
    public float waitTime;
    public float fadeTime;
    public AnimationCurve fadeCurve;

    bool pressAnyToRestart = false;

	public void FadeIn () {
        StartCoroutine(FadeInOverTime());
    }

    IEnumerator FadeInOverTime () {
        yield return new WaitForSeconds(waitTime);
        pressAnyToRestart = true;
        float t = 0f;
        while (t < 1f) {
            t += Time.fixedDeltaTime / fadeTime;
            gameOverGraphics.ForEach(gt => gt.color = gt.color.ChangeAlpha(fadeCurve.Evaluate(t)));
            yield return new WaitForFixedUpdate();
        }
        yield break;
    }

    void Update () {
        if (pressAnyToRestart && Input.anyKey) {
            pressAnyToRestart = false;
            GameController.Instance.RestartGame();
        }
    }

}
