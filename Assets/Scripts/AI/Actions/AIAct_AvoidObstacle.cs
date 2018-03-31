using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI {

    [CreateAssetMenu(fileName = "AvoidObstacle", menuName = "AI/Actions/Avoid Obstacle")]
    public class AIAct_AvoidObstacle : Action {

        public float lookAhead = 100f;
        public float normalMultiplier = 3f;

        public override void Execute (AIBrain brain) {

            Vector3? hitNormal;
            Vector3? hitPos = brain.ObstacleAhead(lookAhead, out hitNormal);
            if (hitPos.HasValue && hitNormal.HasValue) {
                brain.LookAt(hitPos.Value + hitNormal.Value.normalized * normalMultiplier);
            }

        }

    }

}
