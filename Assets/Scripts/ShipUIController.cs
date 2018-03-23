using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JBirdEngine.ColorLibrary;

[System.Serializable]
public class UiBar {

    public Image progress;
    public Color normalColor;
    public Color warningColor;
    public float flashTime;
    public float warningZoneMin;
    public float warningZoneMax;

    protected Coroutine flashRoutine;
    public float currentFlashTime;

    protected IEnumerator FlashWarningColor (float startFlashTime, Image additionalImage = null) {
        currentFlashTime = startFlashTime;
        float flashDirection = 1f;
        while (true) {
            currentFlashTime += (1f / flashTime) * Time.deltaTime * flashDirection;
            if (currentFlashTime >= 1f) {
                flashDirection = -1f;
                currentFlashTime = 1f;
            }
            if (currentFlashTime <= -1f) {
                flashDirection = 1f;
                currentFlashTime = -1f;
            }
            progress.color = ColorHelper.LerpHSV(normalColor, warningColor, currentFlashTime);
            if (additionalImage != null) {
                additionalImage.color = ColorHelper.LerpHSV(normalColor, warningColor, currentFlashTime);
            }
            yield return null;
        }
    }

    public virtual void SetProgress (MonoBehaviour parent, float percent, float warningTime = 0f) {
        percent = Mathf.Clamp01(percent);
        progress.fillAmount = percent;
        if (percent <= warningZoneMax && percent >= warningZoneMin) {
            if (flashRoutine == null) {
                flashRoutine = parent.StartCoroutine(FlashWarningColor(warningTime));
            }
        }
        else if (flashRoutine != null) {
            parent.StopCoroutine(flashRoutine);
            flashRoutine = null;
            progress.color = normalColor;
        }
    }
}

[System.Serializable]
public class ShieldUiBar : UiBar {

    public float minPercent;
    public float maxPercent;

    public Image shieldBase;
    public Image shieldFrame;

    public override void SetProgress (MonoBehaviour parent, float percent, float warningTime = 0) {
        float modifiedPercent = minPercent + percent * (maxPercent - minPercent);
        base.SetProgress(parent, modifiedPercent, warningTime);
    }

    public void Hide () {
        progress.enabled = false;
        shieldBase.enabled = false;
        shieldFrame.enabled = false;
    }

    public void Show () {
        progress.enabled = true;
        shieldBase.enabled = true;
        shieldFrame.enabled = true;
    }

}

[System.Serializable]
public class HealthUiBar : UiBar {

    public Image progressArmor;
    public Image progressShield;
    public Image progressExtra;
    public Image progressExtraArmor;
    public Image progressExtraShield;

    public Image extraBase;
    public Image extraFrame;
    public int extraBuffer;

    public float healthInRing;
    public float minRingPercent;
    public float maxRingPercent;
    public float barPixelsPerHP;

    public void SetProgress (MonoBehaviour parent, Ship ship, float warningTime = 0, bool hullShield = false) {
        float lowHealthDiff = ship.lowHealthActual - ship.lowHealthVisual;
        float visualHealth = ship.health;
        if (visualHealth <= ship.lowHealthActual) {
            visualHealth = (visualHealth / ship.lowHealthActual) * ship.lowHealthVisual;
        }
        else {
            visualHealth -= lowHealthDiff;
        }
        float percent = visualHealth / ship.maxHealth;

        float maxVisualHealth = (ship.maxHealth - lowHealthDiff) + ship.armor + (hullShield ? ship.shield.maxHealth : 0f);
        extraBase.rectTransform.sizeDelta = new Vector2 (Mathf.FloorToInt((maxVisualHealth - healthInRing) * barPixelsPerHP) + extraBuffer, extraBase.rectTransform.sizeDelta.y);
        extraFrame.rectTransform.sizeDelta = new Vector2 (Mathf.FloorToInt((maxVisualHealth - healthInRing) * barPixelsPerHP) + extraBuffer, extraFrame.rectTransform.sizeDelta.y);
        progressExtra.rectTransform.sizeDelta = new Vector2(Mathf.FloorToInt((maxVisualHealth - healthInRing) * barPixelsPerHP) + extraBuffer, progressExtra.rectTransform.sizeDelta.y);
        progressExtraArmor.rectTransform.sizeDelta = new Vector2(Mathf.FloorToInt((maxVisualHealth - healthInRing) * barPixelsPerHP) + extraBuffer, progressExtraArmor.rectTransform.sizeDelta.y);
        progressExtraShield.rectTransform.sizeDelta = new Vector2(Mathf.FloorToInt((maxVisualHealth - healthInRing) * barPixelsPerHP) + extraBuffer, progressExtraShield.rectTransform.sizeDelta.y);

        float visualArmor = visualHealth + ship.armor;
        float visualShield = visualArmor + (hullShield ? ship.shield.health : 0f);

        float ringHealthPercent = minRingPercent + Mathf.Clamp(visualHealth / healthInRing, 0f, 1f) * (maxRingPercent - minRingPercent);
        progress.fillAmount = ringHealthPercent;
        float ringArmorPercent = minRingPercent + Mathf.Clamp(visualArmor / healthInRing, 0f, 1f) * (maxRingPercent - minRingPercent);
        progressArmor.fillAmount = ringArmorPercent;
        float ringShieldPercent = minRingPercent + Mathf.Clamp(visualShield / healthInRing, 0f, 1f) * (maxRingPercent - minRingPercent);
        progressShield.fillAmount = ringShieldPercent;

        float extraHealthPercent = Mathf.Clamp((visualHealth - healthInRing) / (maxVisualHealth - healthInRing), 0f, 1f);
        progressExtra.fillAmount = extraHealthPercent;
        float extraArmorPercent = Mathf.Clamp((visualArmor - healthInRing) / (maxVisualHealth - healthInRing), 0f, 1f);
        progressExtraArmor.fillAmount = extraArmorPercent;
        float extraShieldPercent = Mathf.Clamp((visualShield - healthInRing) / (maxVisualHealth - healthInRing), 0f, 1f);
        progressExtraShield.fillAmount = extraShieldPercent;

        percent = Mathf.Clamp01(percent);

        if (percent <= warningZoneMax && percent >= warningZoneMin) {
            if (flashRoutine == null) {
                flashRoutine = parent.StartCoroutine(FlashWarningColor(warningTime, progressExtra));
            }
        }
        else if (flashRoutine != null) {
            parent.StopCoroutine(flashRoutine);
            flashRoutine = null;
            progress.color = normalColor;
            progressExtra.color = normalColor;
        }
    }

}

public class ShipUIController : MonoBehaviour {

    public Ship target;
    public float healthDangerZone;

    public HealthUiBar healthBar;
    public ShieldUiBar shieldBar;
    public UiBar overheatBar;
    public UiBar abilityBar;
    public UiBar coolantBar;

    void Update () {
        overheatBar.SetProgress(this, target.engine.overheat / target.engine.overheatTime);
        coolantBar.SetProgress(this, target.engine.coolant / target.engine.maxCoolant);
        if (target.engine.IsOverheating() && target.engine.coolant == 0f) {
            healthBar.warningZoneMax = 1f;
        }
        else {
            healthBar.warningZoneMax = healthDangerZone;
        }
        if (target.shield is HullShield) {
            healthBar.SetProgress(this, target, overheatBar.currentFlashTime, true);
            shieldBar.Hide();
        }
        else {
            shieldBar.Show();
            shieldBar.SetProgress(this, target.shield.health / target.shield.maxHealth);
            healthBar.SetProgress(this, target, overheatBar.currentFlashTime, false);
        }
        abilityBar.SetProgress(this, 1f - target.abilityCooldown / target.GetAbilityCooldown());
    }

}
