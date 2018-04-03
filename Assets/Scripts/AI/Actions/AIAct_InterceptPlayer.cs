using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI {

    [CreateAssetMenu(fileName = "InterceptPlayer", menuName = "AI/Actions/Intercept Player")]
    public class AIAct_InterceptPlayer : Action {

        public override void Execute (AIBrain brain) {
            brain.CeaseFire();
            if (brain.extrapolatedPlayerPosition.HasValue && brain.lastKnownPlayerVelocity.HasValue) {
                brain.LookAt(brain.extrapolatedPlayerPosition.Value + brain.lastKnownPlayerVelocity.Value, true);
            }
            brain.Go();
        }

    }

}
