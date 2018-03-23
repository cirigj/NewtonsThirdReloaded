using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageNumber : MonoBehaviour {

    public Text text;
    public float lifeTime;
    public Vector3 movementDirection;
    public AnimationCurve alphaFalloff;

    float life;

    void Start () {
        life = 0f;
    }

    public void SetDamage (int dmg) {
        text.text = string.Format("{0}", dmg.ToString());
    }

    public void SetColor (Color color) {
        text.color = color;
    }

    public void SetSize (float size) {
        text.fontSize = Mathf.RoundToInt(size);
    }

    void FixedUpdate () {
        MovePerStep();
    }

    void MovePerStep () {
        life += Time.fixedDeltaTime;
        if (life >= lifeTime) {
            Kill();
            return;
        }
        transform.position += movementDirection * Time.fixedDeltaTime;
        text.color = new Color(text.color.r, text.color.g, text.color.b, alphaFalloff.Evaluate(life / lifeTime));
    }

    void Kill () {
        Destroy(gameObject);
    }

}
