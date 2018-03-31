using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI {

    [CreateAssetMenu(fileName = "ObstacleInPath", menuName = "AI/Conditions/Obstacle in Path")]
    public class AICond_ObstacleInPath : Condition {

        public float lookAhead;

        public override bool Evaluate (AIBrain brain, params ConditionParam[] condParams) {
            try {
                if (condParams.Length > 1) lookAhead = condParams[0].AsFloat();
            }
            catch (Exception e) {
                Debug.LogWarningFormat("Improper condParams. {0}", e.Message);
            }

            return brain.ObstacleAhead(lookAhead * brain.ship.thrustVelocity.magnitude) != null;
        }

    }

}
