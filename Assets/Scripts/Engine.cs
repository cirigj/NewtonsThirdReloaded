using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour {

    [Header("Engine Stats")]
    public float thrust;
    public float overheatTime;
    public float overheatRate;
    public float cooldownRate;
    public float overheatCooldown;
    public float overheatDamage;
    public float kickbackMitigation;
    public bool shutDownOnOverheat;

    [Header("Runtime")]
    public float overheat;
    public float cooldown;
    public List<ParticleSystem> particles;

    void Update () {
        if (cooldown > 0f) {
            cooldown = Mathf.Clamp(cooldown - Time.fixedDeltaTime, 0f, overheatCooldown);
        }
        if (cooldown == 0f && overheat > 0f) {
            overheat = Mathf.Clamp(overheat - cooldownRate * Time.fixedDeltaTime, 0f, overheatTime);
        }
    }

    public Vector3 GetDeltaThrust (float shipMass) {
        if (shutDownOnOverheat && overheat >= overheatTime) {
            return Vector3.zero;
        }
        return -transform.forward * (thrust / shipMass) * Time.fixedDeltaTime;
    }

    public Vector3 GetKickbackMitigation (float shipMass) {
        if (shutDownOnOverheat && overheat >= overheatTime) {
            return Vector3.zero;
        }
        return -transform.forward * kickbackMitigation / shipMass;
    }

    public void OverheatFromThrust (Vector3 deltaThrust) {
        overheat = Mathf.Clamp(overheat + (deltaThrust.magnitude / thrust) * overheatRate, 0f, overheatTime);
        cooldown = overheatCooldown;
    }

    public bool IsCausingOverheatDamage () {
        return overheat >= overheatTime && overheatDamage > 0f;
    }

    public float GetOverheatDamage () {
        return overheatDamage * Time.fixedDeltaTime;
    }

    public void TurnOnParticles () {
        particles.ForEach(p => p.Play());
    }

    public void TurnOffParticles () {
        particles.ForEach(p => p.Stop());
    }

}
