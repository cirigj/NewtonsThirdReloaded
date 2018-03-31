using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI {

    [CreateAssetMenu(fileName = "Wander", menuName = "AI/Actions/Wander")]
    public class AIAct_Wander : Action {

        public float circleDistance;
        public float circleRadius;
        public float overheatThresholdMax;
        public float overheatThresholdMin;

        bool lookRandom;

        public override void Execute (AIBrain brain) {
            if (brain.OverheatAboveThreshold(overheatThresholdMax)) {
                brain.Stop();
                if (!lookRandom) {
                    brain.LookRandomDirection();
                    lookRandom = true;
                }
            }
            else if (!brain.OverheatAboveThreshold(overheatThresholdMin)) {
                lookRandom = false;
                brain.WanderAim(circleDistance, circleRadius);
                brain.Go();
            }
        }

    }

}
