using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour {

    public Ship ship;

    [Header("Engine Stats")]
    public float thrust;
    public float maxSpeed;
    public float overheatTime;
    public float overheatRate;
    public float cooldownRate;
    public float overheatCooldown;
    public float overheatDamage;
    public float maxCoolant;
    public float coolantUseRate;
    public float kickbackMitigation;
    public bool shutDownOnOverheat;

    [Header("Ram Booster Stats")]
    public float ramMaxSpeedMultiplier;
    public float ramSpeedFalloff;

    [Header("Runtime")]
    public float overheat;
    public float cooldown;
    public float coolant;
    public List<ParticleSystem> particles;
    public ParticleSystem burstParticles;

    void Update () {
        if (cooldown > 0f) {
            cooldown = Mathf.Clamp(cooldown - Time.fixedDeltaTime, 0f, overheatCooldown);
        }
        if (cooldown == 0f && overheat > 0f) {
            overheat = Mathf.Clamp(overheat - cooldownRate * Time.fixedDeltaTime, 0f, overheatTime);
        }
    }

    public void AddCoolant (float extra) {
        coolant = Mathf.Clamp(coolant + extra, 0f, maxCoolant);
        ship.coolantParticles.Play();
    }

    public Vector3 GetDeltaThrust (float shipMass) {
        if (shutDownOnOverheat && (overheat >= overheatTime && coolant == 0f)) {
            return Vector3.zero;
        }
        return -transform.forward * (thrust / shipMass) * Time.fixedDeltaTime;
    }

    public Vector3 GetKickbackMitigation (float shipMass) {
        if (shutDownOnOverheat && (overheat >= overheatTime && coolant == 0f)) {
            return Vector3.zero;
        }
        return -transform.forward * kickbackMitigation / shipMass;
    }

    public void OverheatFromThrust (Vector3 deltaThrust) {
        overheat = Mathf.Clamp(overheat + (deltaThrust.magnitude / thrust) * overheatRate, 0f, overheatTime);
        cooldown = overheatCooldown;
    }

    public bool IsOverheating () {
        return overheat >= overheatTime;
    }

    public float GetOverheatDamage () {
        if (coolant > 0f) {
            coolant = Mathf.Clamp(coolant - coolantUseRate * Time.fixedDeltaTime, 0f, maxCoolant);
            return 0f;
        }
        return overheatDamage * Time.fixedDeltaTime;
    }

    public void TurnOnParticles () {
        particles.ForEach(p => p.Play());
    }

    public void TurnOffParticles () {
        particles.ForEach(p => p.Stop());
    }

    public void PlayParticleBurst () {
        burstParticles.Play();
    }

}
