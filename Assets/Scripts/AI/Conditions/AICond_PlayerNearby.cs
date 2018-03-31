using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI {

    [CreateAssetMenu(fileName = "PlayerNearby", menuName = "AI/Conditions/Player Nearby")]
    public class AICond_PlayerNearby : Condition {

        public override bool Evaluate (AIBrain brain, params ConditionParam[] condParams) {
            return brain.playerNearby;
        }

    }

}