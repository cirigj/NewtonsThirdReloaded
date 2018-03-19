using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HullShield : Shield {

    public override void Regenerate () {
        if (rechargeCooldown > 0f) {
            rechargeCooldown = Mathf.Clamp(rechargeCooldown - Time.fixedDeltaTime, 0f, Mathf.Max(shieldHitCooldownPenalty, shieldDownCooldownPenalty));
        }
        if (rechargeCooldown == 0f) {
            if (ship.health < ship.maxHealth) {
                ship.RepairDamage(rechargeRate * Time.fixedDeltaTime);
            }
            else {
                health = Mathf.Clamp(health + rechargeRate * Time.fixedDeltaTime, 0f, maxHealth);
            }
        }
    }

    public override void TakeDamage (float dmg) {
        if (health < dmg) {
            ship.TakeDamage(dmg - health);
        }
        base.TakeDamage(dmg);
    }

    protected override void SetColliderState () {
        base.SetColliderState();
        ship.collider.enabled = health == 0f;
    }

}
