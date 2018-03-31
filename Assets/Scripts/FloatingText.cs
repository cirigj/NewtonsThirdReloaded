using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JBirdEngine.ColorLibrary;
using JBirdEngine;

public class FloatingText : MonoBehaviour {

    public Text text;
    public Text shadow;
    public float lifeTime;
    public Vector3 movementDirection;
    public AnimationCurve alphaFalloff;

    float life;
    Transform parent;

    void Start () {
        life = 0f;
    }

    public void SetString (string str) {
        text.text = str;
        if (shadow != null) {
            shadow.text = str;
        }
    }

    public void SetColor (Color color) {
        text.color = color;
        if (shadow != null) {
            shadow.color = shadow.color.ChangeAlpha(color.a);
        }
    }

    public void SetSize (float size) {
        text.fontSize = Mathf.RoundToInt(size);
        if (shadow != null) {
            shadow.fontSize = Mathf.RoundToInt(size);
        }
    }

    public void SetParent (Transform parent) {
        this.parent = parent;
    }

    void FixedUpdate () {
        MovePerStep();
    }

    void MovePerStep () {
        life += Time.fixedDeltaTime;
        if (life >= lifeTime) {
            Kill();
        }
        else {
            transform.position += movementDirection * Time.fixedDeltaTime;
            if (parent != null) {
                transform.position = transform.position.SetX(parent.position.x).SetZ(parent.position.z);
            }
            text.color = new Color(text.color.r, text.color.g, text.color.b, alphaFalloff.Evaluate(life / lifeTime));
        }
    }

    public void Kill () {
        Destroy(gameObject);
    }
}
