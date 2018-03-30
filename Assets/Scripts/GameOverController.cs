using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JBirdEngine.ColorLibrary;

public class GameOverController : MonoBehaviour {

    public Text gameOverText;
    public float waitTime;
    public float fadeTime;
    public AnimationCurve fadeCurve;

	public void FadeIn () {
        StartCoroutine(FadeInOverTime());
    }

    IEnumerator FadeInOverTime () {
        yield return new WaitForSeconds(waitTime);
        float t = 0f;
        while (t < 1f) {
            t += Time.fixedDeltaTime / fadeTime;
            gameOverText.color = gameOverText.color.ChangeAlpha(fadeCurve.Evaluate(t));
            yield return new WaitForFixedUpdate();
        }
        yield break;
    }

}
