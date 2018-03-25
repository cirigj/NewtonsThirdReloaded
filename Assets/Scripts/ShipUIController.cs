using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public Coroutine flashRoutine;
    public float currentFlashTime;

    public IEnumerator FlashWarningColor (float startFlashTime, Image additionalImage = null) {
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

    public override void SetProgress (MonoBehaviour parent, float percent, float warningTime = 0) {
        float modifiedPercent = minPercent + percent * (maxPercent - minPercent);
        base.SetProgress(parent, modifiedPercent, warningTime);
    }

}

[System.Serializable]
public class CoolantUiBar : UiBar {

    public override void SetProgress (MonoBehaviour parent, float percent, float warningTime = 0) {
        percent = Mathf.Clamp01(percent);
        if (progress.material.GetFloat("_Fill") != percent) {
            var newMat = new Material(progress.material);
            newMat.SetFloat("_Fill", percent);
            progress.material = newMat;
        }
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
public class HealthUiBar : UiBar {

    public Image progressExtra;
    public Image progressExtraArmor;

    public UiBar armor;

    public List<Image> extraBits;
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
        extraBits.ForEach(e => e.rectTransform.sizeDelta = new Vector2(Mathf.FloorToInt((maxVisualHealth - healthInRing) * barPixelsPerHP) + extraBuffer, e.rectTransform.sizeDelta.y));

        float visualArmor = visualHealth + ship.armor;
        float visualShield = visualArmor + (hullShield ? ship.shield.health : 0f);

        float ringHealthPercent = minRingPercent + Mathf.Clamp(visualHealth / healthInRing, 0f, 1f) * (maxRingPercent - minRingPercent);
        progress.fillAmount = ringHealthPercent;
        float ringArmorPercent = minRingPercent + Mathf.Clamp(visualArmor / healthInRing, 0f, 1f) * (maxRingPercent - minRingPercent);
        armor.progress.fillAmount = ringArmorPercent;

        float extraHealthPercent = Mathf.Clamp((visualHealth - healthInRing) / (maxVisualHealth - healthInRing), 0f, 1f);
        progressExtra.fillAmount = extraHealthPercent;
        float extraArmorPercent = Mathf.Clamp((visualArmor - healthInRing) / (maxVisualHealth - healthInRing), 0f, 1f);
        progressExtraArmor.fillAmount = extraArmorPercent;

        percent = Mathf.Clamp01(percent);

        if (percent <= warningZoneMax && percent >= warningZoneMin) {
            if (flashRoutine == null) {
                flashRoutine = parent.StartCoroutine(FlashWarningColor(warningTime, progressExtra));
                armor.flashRoutine = parent.StartCoroutine(armor.FlashWarningColor(warningTime, progressExtraArmor));
            }
        }
        else if (flashRoutine != null) {
            parent.StopCoroutine(flashRoutine);
            parent.StopCoroutine(armor.flashRoutine);
            flashRoutine = null;
            armor.flashRoutine = null;
            progress.color = normalColor;
            progressExtra.color = normalColor;
            armor.progress.color = armor.normalColor;
            progressExtraArmor.color = armor.normalColor;
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
    public CoolantUiBar coolantBar;

    public UiBar warningLight;

    public AnimationCurve coolantCorrectionCurve;

    float VolumetricCorrection (float percent) {
        return coolantCorrectionCurve.Evaluate(percent);
    }

    void Start () {
        coolantBar.progress.material = new Material(coolantBar.progress.material);
    }

    void Update () {
        if (target.engine.IsOverheating()) {
            coolantBar.warningZoneMax = 1f;
            warningLight.SetProgress(this, 1f);
            if (target.engine.overheatDamage > 0f && target.engine.coolant == 0f) {
                healthBar.warningZoneMax = 1f;
            }
            else {
                healthBar.warningZoneMax = healthDangerZone;
            }
        }
        else {
            warningLight.SetProgress(this, 0f);
            healthBar.warningZoneMax = healthDangerZone;
            coolantBar.warningZoneMax = 0f;
        }
        overheatBar.SetProgress(this, target.engine.overheat / target.engine.overheatTime);
        coolantBar.SetProgress(this, VolumetricCorrection(target.engine.coolant / target.engine.maxCoolant));
        shieldBar.SetProgress(this, target.shield.health / target.shield.maxHealth);
        healthBar.SetProgress(this, target, overheatBar.currentFlashTime, false);
        abilityBar.SetProgress(this, 1f - target.abilityCooldown / target.GetAbilityCooldown());
    }

}
