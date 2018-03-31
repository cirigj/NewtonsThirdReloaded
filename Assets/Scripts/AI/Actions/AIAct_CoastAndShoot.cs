using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI {

    [CreateAssetMenu(fileName = "CoastAndShoot", menuName = "AI/Actions/Coast and Shoot")]
    public class AIAct_CoastAndShoot : Action {

        public override void Execute (AIBrain brain) {
            brain.Coast();
            if (brain.extrapolatedPlayerPosition.HasValue && brain.lastKnownPlayerVelocity.HasValue) {
                brain.LookAt(brain.extrapolatedPlayerPosition.Value);
                brain.Fire();
            }
        }

    }

}
