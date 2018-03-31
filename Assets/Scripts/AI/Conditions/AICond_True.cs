using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI {

    [CreateAssetMenu(fileName = "True", menuName = "AI/Conditions/True")]
    public class AICond_True : Condition {

        public override bool Evaluate (AIBrain brain, params ConditionParam[] condParams) {
            return true;
        }

    }

}
