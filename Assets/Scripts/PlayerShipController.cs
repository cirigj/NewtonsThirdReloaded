using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour {

    public Ship ship;

    public virtual void WeaponTrigger () {

    }

    public virtual void WeaponRelease () {

    }

    public virtual void AbilityTrigger () {

    }
    
    public virtual void AbilityRelease () {

    }

    public virtual void EngineTrigger () {

    }

    public virtual void EngineRelease () {

    }

    public virtual void GotoYaw (float x, float y) {
        ship.SetTargetYaw(new Vector2(x, y));
    }

}

public class PlayerShipController : ShipController {

    [SerializeField]
    [JBirdEngine.EditorHelper.ViewOnly]
    private bool fireControl;

    [SerializeField]
    [JBirdEngine.EditorHelper.ViewOnly]
    private bool thrustControl;

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

        // Yaw Control
        if (yawH != 0f || yawV != 0f) {
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