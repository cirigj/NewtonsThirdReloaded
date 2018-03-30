using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI {

	public abstract class Condition : ScriptableObject {

        public abstract bool Evaluate (AIBrain brain, params ConditionParam[] condParams);

    }

    [System.Serializable]
    public class ConditionParam {

        [SerializeField]
        private string param;

        public string AsString () {
            return param;
        }

        public float AsFloat () {
            float result;
            if (float.TryParse(param, out result)) {
                return result;
            }
            throw new System.ArgumentException(string.Format("Param '{0}' cannot be converted to float!"));
        }

        public int AsInt () {
            int result;
            if (int.TryParse(param, out result)) {
                return result;
            }
            throw new System.ArgumentException(string.Format("Param '{0}' cannot be converted to int!"));
        }

        public bool AsBool () {
            bool result;
            if (bool.TryParse(param, out result)) {
                return result;
            }
            throw new System.ArgumentException(string.Format("Param '{0}' cannot be converted to bool!"));
        }

    }

    [System.Serializable]
    public class ConditionInstance {

        public Condition condition;
        public List<ConditionParam> conditionParams;

        public bool Evaluate (AIBrain brain) {
            return condition.Evaluate(brain, conditionParams.ToArray());
        }

    }

    public abstract class Action : ScriptableObject {

        public abstract void Execute (AIBrain brain);

    }

    [System.Serializable]
    public class Response {

        public List<ConditionInstance> conditions;
        public Action action;

    }

    [System.Serializable]
    public class Preference {

        public List<ConditionInstance> conditions;
        public AIBehaviour behaviour;

    }

}
