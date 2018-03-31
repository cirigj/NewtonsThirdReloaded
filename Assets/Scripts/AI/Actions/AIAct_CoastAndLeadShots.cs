using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI {

    [CreateAssetMenu(fileName = "CoastAndLeadShots", menuName = "AI/Actions/Coast and Lead Shots")]
    public class AIAct_CoastAndLeadShots : Action {

        public override void Execute (AIBrain brain) {
            brain.Coast();
            if (brain.extrapolatedPlayerPosition.HasValue && brain.lastKnownPlayerVelocity.HasValue) {
                float lookAheadSeconds = (brain.extrapolatedPlayerPosition.Value - brain.transform.position).magnitude / brain.ship.weapon.projectileSpeed;
                brain.LookAt(brain.extrapolatedPlayerPosition.Value + brain.lastKnownPlayerVelocity.Value * lookAheadSeconds);
                brain.Fire();
            }
        }

    }

}
