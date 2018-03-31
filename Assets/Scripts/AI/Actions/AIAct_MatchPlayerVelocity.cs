using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI {

    [CreateAssetMenu(fileName = "MatchPlayerVelocity", menuName = "AI/Actions/Match Player Velocity")]
    public class AIAct_MatchPlayerVelocity : Action {

        public override void Execute (AIBrain brain) {
            if (brain.lastKnownPlayerVelocity.HasValue) {
                brain.LookAt(brain.transform.position + brain.lastKnownPlayerVelocity.Value);
            }
            brain.Go();
            brain.CeaseFire();
        }

    }

}