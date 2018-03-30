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
    public float maxPercentPerStep;

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
        if (percent > progress.fillAmount) {
            progress.fillAmount = Mathf.Min(progress.fillAmount + maxPercentPerStep, percent);
        }
        else {
            progress.fillAmount = Mathf.Max(progress.fillAmount - maxPercentPerStep, percent);
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
        float fill = progress.material.GetFloat("_Fill");
        if (fill != percent) {
            var newMat = new Material(progress.material);
            if (percent > fill) {
                newMat.SetFloat("_Fill", Mathf.Min(fill + maxPercentPerStep, percent));
            }
            else {
                newMat.SetFloat("_Fill", Mathf.Max(fill - maxPercentPerStep, percent));
            }
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
    public int maxBarPixelsPerStep;

    float clampedArmor = 0;
    float clampedHealth = 120;

    public void SetProgress (MonoBehaviour parent, Ship ship, float warningTime = 0) {
        float lowHealthDiff = ship.lowHealthActual - ship.lowHealthVisual;

        if (ship.health > clampedHealth) {
            clampedHealth = Mathf.Min(clampedHealth + maxBarPixelsPerStep / barPixelsPerHP, ship.health);
        }
        else {
            clampedHealth = Mathf.Max(clampedHealth - maxBarPixelsPerStep / barPixelsPerHP, ship.health);
        }

        float visualHealth = clampedHealth;

        if (visualHealth <= ship.lowHealthActual) {
            visualHealth = (visualHealth / ship.lowHealthActual) * ship.lowHealthVisual;
        }
        else {
            visualHealth -= lowHealthDiff;
        }

        if (ship.armor > clampedArmor) {
            clampedArmor = Mathf.Min(clampedArmor + maxBarPixelsPerStep / barPixelsPerHP, ship.armor);
        }
        else {
            clampedArmor = Mathf.Max(clampedArmor - maxBarPixelsPerStep / barPixelsPerHP, ship.armor);
        }

        float maxVisualHealth = (ship.maxHealth - lowHealthDiff) + clampedArmor;
        int barSize = Mathf.FloorToInt((maxVisualHealth - healthInRing) * barPixelsPerHP);
        extraBits.ForEach(e => e.rectTransform.sizeDelta = new Vector2(
                (barSize > e.rectTransform.sizeDelta.x
                    ? Mathf.Min(e.rectTransform.sizeDelta.x + maxBarPixelsPerStep, barSize)
                    : Mathf.Max(e.rectTransform.sizeDelta.x - maxBarPixelsPerStep, barSize)
                ) + extraBuffer,
                e.rectTransform.sizeDelta.y
            ));

        float visualArmor = visualHealth + clampedArmor;
        float percent = visualArmor / maxVisualHealth;

        float ringHealthPercent = minRingPercent + Mathf.Clamp(visualHealth / healthInRing, 0f, 1f) * (maxRingPercent - minRingPercent);
        if (ringHealthPercent > progress.fillAmount) {
            progress.fillAmount = Mathf.Min(progress.fillAmount + maxPercentPerStep, ringHealthPercent);
        }
        else {
            progress.fillAmount = Mathf.Max(progress.fillAmount - maxPercentPerStep, ringHealthPercent);
        }
        float ringArmorPercent = minRingPercent + Mathf.Clamp(visualArmor / healthInRing, 0f, 1f) * (maxRingPercent - minRingPercent);
        if (ringArmorPercent > armor.progress.fillAmount) {
            armor.progress.fillAmount = Mathf.Min(armor.progress.fillAmount + maxPercentPerStep, ringArmorPercent);
        }
        else {
            armor.progress.fillAmount = Mathf.Max(armor.progress.fillAmount - maxPercentPerStep, ringArmorPercent);
        }

        float extraHealthPercent = Mathf.Clamp((visualHealth - healthInRing) / (maxVisualHealth - healthInRing), 0f, 1f);
        if (extraHealthPercent > progressExtra.fillAmount) {
            progressExtra.fillAmount = Mathf.Min(progressExtra.fillAmount + maxPercentPerStep, extraHealthPercent);
        }
        else {
            progressExtra.fillAmount = Mathf.Max(progressExtra.fillAmount - maxPercentPerStep, extraHealthPercent);
        }
        float extraArmorPercent = Mathf.Clamp((visualArmor - healthInRing) / (maxVisualHealth - healthInRing), 0f, 1f);
        if (extraArmorPercent > progressExtraArmor.fillAmount) {
            progressExtraArmor.fillAmount = Mathf.Min(progressExtraArmor.fillAmount + maxPercentPerStep, extraArmorPercent);
        }
        else {
            progressExtraArmor.fillAmount = Mathf.Max(progressExtraArmor.fillAmount - maxPercentPerStep, extraArmorPercent);
        }

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

[System.Serializable]
public class AbilityIcon {
    public ShipAbility ability;
    public Sprite Icon;
}

public class ShipUIController : MonoBehaviour {

    public Ship target;
    public float healthDangerZone;

    public HealthUiBar healthBar;
    public ShieldUiBar shieldBar;
    public UiBar overheatBar;
    public UiBar ability1Bar;
    public Image ability1Icon;
    public UiBar abilityBar2;
    public Image ability2Icon;
    public CoolantUiBar coolantBar;

    public UiBar warningLight;

    public AnimationCurve coolantCorrectionCurve;
    public List<AbilityIcon> abilityIcons;

    float VolumetricCorrection (float percent) {
        return coolantCorrectionCurve.Evaluate(percent);
    }

    void Start () {
        coolantBar.progress.material = new Material(coolantBar.progress.material);

        ability1Icon.sprite = GetIcon(target.ability1.type);
        ability2Icon.sprite = GetIcon(target.ability2.type);
    }

    Sprite GetIcon (ShipAbility ability) {
        AbilityIcon abIcon = abilityIcons.Where(a => a.ability == ability).FirstOrDefault();
        if (abIcon != null) {
            return abIcon.Icon;
        }
        return null;
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
        healthBar.SetProgress(this, target, overheatBar.currentFlashTime);
        ability1Bar.SetProgress(this, 1f - target.ability1.cooldown / target.ability1.GetMaxCooldown());
        abilityBar2.SetProgress(this, 1f - target.ability2.cooldown / target.ability2.GetMaxCooldown());
    }

}
