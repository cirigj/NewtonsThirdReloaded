using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI {

    [CreateAssetMenu(fileName = "PlayerInRange", menuName = "AI/Conditions/Player in Range")]
    public class AICond_PlayerWithinRange : Condition {

        public float range;

        public override bool Evaluate (AIBrain brain, params ConditionParam[] condParams) {
            try {
                if (condParams.Length > 1) range = condParams[0].AsFloat();
            }
            catch (System.Exception e) {
                Debug.LogWarningFormat("Improper condParams. {0}", e.Message);
            }

            return brain.extrapolatedPlayerPosition.HasValue ? brain.PositionWithinRange(brain.extrapolatedPlayerPosition.Value, range) : false;
        }

    }

}
