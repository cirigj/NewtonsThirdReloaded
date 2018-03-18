using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour {

    public Ship ship;

    protected void WeaponTrigger () {
        ship.ActivateWeapon();
    }

    protected void WeaponRelease () {
        ship.DeactivateWeapon();
    }

    protected void AbilityTrigger () {
        ship.ActivateAbility();
    }

    protected void AbilityRelease () {
        ship.DeactivateAbility();
    }

    protected void EngineTrigger () {
        ship.ActivateThruster();
    }

    protected void EngineRelease () {
        ship.DeactivateThruster();
    }

    protected void DriftTrigger () {
        ship.ActivateDrift();
    }

    protected void DriftRelease () {
        ship.DeactivateDrift();
    }

    protected void GotoYaw (float x, float y) {
        ship.SetTargetYaw(new Vector2(x, y));
    }

}

public class PlayerShipController : ShipController {

    public float yawDeadzone;

    [SerializeField]
    [JBirdEngine.EditorHelper.ViewOnly]
    private bool fireControl;

    [SerializeField]
    [JBirdEngine.EditorHelper.ViewOnly]
    private bool thrustControl;

    [SerializeField]
    [JBirdEngine.EditorHelper.ViewOnly]
    private bool driftControl;

    [SerializeField]
    [JBirdEngine.EditorHelper.ViewOnly]
    private bool abilityControl;

    void Update () {
        GetInputs();
    }

    void GetInputs () {
        // Get Raw Axes
        float yawH = Input.GetAxis("Horizontal");
        float yawV = Input.GetAxis("Vertical");
        float fire = Input.GetAxis("Fire");
        float thrust = Input.GetAxis("Thrust");
        float ability = Input.GetAxis("Ability");
        float drift = Input.GetAxis("Drift");

        // Yaw Control
        if (yawH >= yawDeadzone || yawH <= -yawDeadzone
         || yawV >= yawDeadzone || yawV <= -yawDeadzone) {
            GotoYaw(yawH, yawV);
        }

        // Weapon Trigger/Release
        if (!fireControl) {
            if (fire > 0) {
                fireControl = true;
                WeaponTrigger();
            }
        }
        else {
            if (fire <= 0) {
                fireControl = false;
                WeaponRelease();
            }
        }

        // Engine Trigger/Release
        if (!thrustControl) {
            if (thrust > 0) {
                thrustControl = true;
                EngineTrigger();
            }
        }
        else {
            if (thrust <= 0) {
                thrustControl = false;
                EngineRelease();
            }
        }

        // Drift Trigger/Release
        if (!driftControl) {
            if (drift > 0) {
                driftControl = true;
                DriftTrigger();
            }
        }
        else {
            if (drift <= 0) {
                driftControl = false;
                DriftRelease();
            }
        }

        // Ability Trigger/Release
        if (!abilityControl) {
            if (ability > 0) {
                abilityControl = true;
                AbilityTrigger();
            }
        }
        else {
            if (ability <= 0) {
                abilityControl = false;
                AbilityRelease();
            }
        }
    }

}