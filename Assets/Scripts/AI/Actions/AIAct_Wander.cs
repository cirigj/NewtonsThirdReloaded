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
        public float randomLookTimeMin;
        public float randomLookTimeMax;

        float randomLookTime = 0f;

        public override void Execute (AIBrain brain) {
            brain.CeaseFire();
            randomLookTime -= Time.fixedDeltaTime;
            if (randomLookTime <= 0f) {
                brain.LookRandomDirection();
                randomLookTime = UnityEngine.Random.Range(randomLookTimeMin, randomLookTimeMax);
            }

            if (brain.OverheatAboveThreshold(overheatThresholdMax)) {
                brain.Coast();
            }
            else if (!brain.OverheatAboveThreshold(overheatThresholdMin)) {
                brain.WanderAim(circleDistance, circleRadius);
                brain.Go();
            }
        }

    }

}
