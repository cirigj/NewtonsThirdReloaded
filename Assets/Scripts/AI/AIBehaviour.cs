using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI {

    [CreateAssetMenu(fileName = "Behaviour", menuName = "AI/Behaviour")]
    public class AIBehaviour : ScriptableObject {

        public List<Response> responses;
        public Action defaultAction;

        public Action GetAction (AIBrain brain) {
            foreach (Response resp in responses) {
                if (resp.condition.Evaluate(brain)) {
                    return resp.action;
                }
            }
            return defaultAction;
        }

    }

}
