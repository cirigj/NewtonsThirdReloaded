using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JBirdEngine;

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

    public static class AIHelper {

        #region Conditions

        public static Vector3? ObstacleAhead (this AIBrain brain, float maxDistance) {
            RaycastHit hit;
            Physics.Raycast(
                brain.transform.position,
                brain.ship.thrustVelocity.normalized,
                out hit,
                maxDistance,
                LayerMask.GetMask("EnemyShip","NeutralObject")
            );

            if (hit.collider != null) {
                return hit.point;
            }
            return null;
        }

        public static bool OverheatAboveThreshold (this AIBrain brain, float threshold) {
            return brain.ship.engine.overheat > threshold;
        }

        #endregion

        #region Actions

        public static void Stop (this AIBrain brain) {
            brain.ship.ActivateDrift();
            brain.ship.DeactivateThruster();
        }

        public static void Go (this AIBrain brain) {
            brain.ship.DeactivateDrift();
            brain.ship.ActivateThruster();
        }

        public static void LookAt (this AIBrain brain, Vector3 pos) {
            brain.ship.ActivateDrift();
            Vector3 direction = (pos - brain.transform.position).normalized;
            brain.ship.SetTargetYaw(new Vector2(direction.x, direction.z));
        }

        #endregion

        #region Behaviours

        public static void WanderAim (this AIBrain brain, float circleDistance, float circleRadius) {
            Vector3 target = brain.transform.position + brain.transform.forward * circleDistance + Random.onUnitSphere.SetY(0f).normalized * circleRadius;
            brain.LookAt(target);
        }

        public static void LookRandomDirection (this AIBrain brain) {
            brain.LookAt(brain.transform.position + Random.onUnitSphere.SetY(0f).normalized * 50f);
        }

        #endregion

    }

}
