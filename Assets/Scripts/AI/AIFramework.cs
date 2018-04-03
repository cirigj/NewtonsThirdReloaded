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
            Vector3? normal;
            return brain.ObstacleAhead(maxDistance, out normal);
        }

        public static Vector3? ObstacleAhead (this AIBrain brain, float maxDistance, out Vector3? normal) {
            RaycastHit hit;
            Physics.SphereCast(
                brain.transform.position,
                1f,
                brain.ship.thrustVelocity.magnitude > 0.5f ? brain.ship.thrustVelocity.normalized : brain.transform.forward,
                out hit,
                maxDistance,
                LayerMask.GetMask("EnemyShip","NeutralObject")
            );

            if (hit.collider != null) {
                normal = hit.normal;
                return hit.point;
            }
            normal = null;
            return null;
        }

        public static bool PlayerVisible (this AIBrain brain) {
            return !Physics.Raycast(
                brain.transform.position,
                (brain.playerShip.transform.position - brain.transform.position).normalized,
                (brain.playerShip.transform.position - brain.transform.position).magnitude,
                LayerMask.GetMask("EnemyShip", "NeutralObject")
            );
        }

        public static bool OverheatAboveThreshold (this AIBrain brain, float threshold) {
            return brain.ship.engine.overheat > threshold;
        }

        public static bool ObjectWithinConeOfVision (this AIBrain brain, Transform tf, float radius, float angle) {
            return brain.PositionWithinConeOfVision(tf.position, radius, angle);
        }

        public static bool PositionWithinConeOfVision (this AIBrain brain, Vector3 pos, float radius, float angle) {
            float azimuthToObject = VectorHelper.GetAzimuth(pos, brain.transform.position, Vector3.up, brain.transform.forward);
            if (Mathf.Abs(azimuthToObject) <= angle / 2f) {
                return brain.PositionWithinRange(pos, radius);
            }
            return false;
        }

        public static bool ObjectWithinRange (this AIBrain brain, Transform tf, float radius) {
            return Vector3.Distance(brain.transform.position, tf.position) <= radius;
        }

        public static bool PositionWithinRange (this AIBrain brain, Vector3 pos, float radius) {
            return Vector3.Distance(brain.transform.position, pos) <= radius;
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

        public static void Coast (this AIBrain brain) {
            brain.ship.DeactivateDrift();
            brain.ship.DeactivateThruster();
        }

        public static void LookAt (this AIBrain brain, Vector3 pos, bool brake = false) {
            if (brake) {
                brain.ship.ActivateDrift();
            }
            Vector3 direction = (pos - brain.transform.position).normalized;
            brain.ship.SetTargetYaw(new Vector2(direction.x, direction.z));
        }

        public static void Fire (this AIBrain brain) {
            brain.ship.ActivateWeapon();
        }

        public static void CeaseFire (this AIBrain brain) {
            brain.ship.DeactivateWeapon();
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
