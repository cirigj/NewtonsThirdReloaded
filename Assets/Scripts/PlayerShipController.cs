using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShipController : MonoBehaviour, IKillable {

    public Ship ship;
    public ParticleSystem deathParticles;

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
    private bool ability1Control;

    [SerializeField]
    [JBirdEngine.EditorHelper.ViewOnly]
    private bool ability2Control;

    void Update () {
        GetInputs();
    }

    void GetInputs () {
        // Get Raw Axes
        float yawH = Input.GetAxis("Horizontal");
        float yawV = Input.GetAxis("Vertical");
        float fire = Input.GetAxis("Fire");
        float thrust = Input.GetAxis("Thrust");
        float ability1 = Input.GetAxis("Ability1");
        float ability2 = Input.GetAxis("Ability2");
        float drift = Input.GetAxis("Drift");

        // Yaw Control
        if (yawH >= yawDeadzone || yawH <= -yawDeadzone
         || yawV >= yawDeadzone || yawV <= -yawDeadzone) {
            ship.SetTargetYaw(new Vector2(yawH, yawV));
        }

        // Weapon Trigger/Release
        if (!fireControl) {
            if (fire > 0) {
                fireControl = true;
                ship.ActivateWeapon();
            }
        }
        else {
            if (fire <= 0) {
                fireControl = false;
                ship.DeactivateWeapon();
            }
        }

        // Engine Trigger/Release
        if (!thrustControl) {
            if (thrust > 0) {
                thrustControl = true;
                ship.ActivateThruster();
            }
        }
        else {
            if (thrust <= 0) {
                thrustControl = false;
                ship.DeactivateThruster();
            }
        }

        // Drift Trigger/Release
        if (!driftControl) {
            if (drift > 0) {
                driftControl = true;
                ship.ActivateDrift();
            }
        }
        else {
            if (drift <= 0) {
                driftControl = false;
                ship.DeactivateDrift();
            }
        }

        // Ability 1 Trigger/Release
        if (!ability1Control) {
            if (ability1 > 0 && ship.ActivateAbility(1)) {
                ability1Control = true;
            }
        }
        else {
            if (ability1 <= 0) {
                ability1Control = false;
                ship.DeactivateAbility(1);
            }
        }

        // Ability 2 Trigger/Release
        if (!ability2Control) {
            if (ability2 > 0 && ship.ActivateAbility(2)) {
                ability2Control = true;
            }
        }
        else {
            if (ability2 <= 0) {
                ability2Control = false;
                ship.DeactivateAbility(2);
            }
        }
    }

    public void Kill() {
        Instantiate(deathParticles, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
        GameController.instance.GameOver();
    }

}