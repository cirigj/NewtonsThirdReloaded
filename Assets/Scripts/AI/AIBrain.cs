using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI {

    public class AIBrain : ISpawnable {

        #region ISpawnable Implementation

        protected ISpawner parent;
        public float spawnRadius;

        public override ISpawner GetParent () {
            return parent;
        }

        public override void SetParent (ISpawner parent) {
            this.parent = parent;
        }

        public override float GetSpawnRadius () {
            return spawnRadius;
        }

        public override void PostDespawn (bool calledParent) {
            Destroy(gameObject);
        }

        #endregion

        public List<Preference> preferences;
        public AIBehaviour defaultBehaviour;

        void Act () {
            Action currentAction = null;
            foreach (Preference pref in preferences) {
                if (pref.condition.Evaluate(this)) {
                    currentAction = pref.behaviour.GetAction(this);
                    break;
                }
            }
            if (currentAction == null) {
                currentAction = defaultBehaviour.GetAction(this);
            }
            currentAction.Execute(this);
        }

    }

}
