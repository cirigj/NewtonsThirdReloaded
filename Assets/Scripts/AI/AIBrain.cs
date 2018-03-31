using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AI {

    public class AIBrain : ISpawnable, IKillable {

        public Ship ship;

        #region ISpawnable Implementation

        [Header("Spawn Control")]
        protected ISpawner parent;
        public float spawnRadius;
        public ParticleSystem deathParticles;

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
            if (calledParent) {
                Explode();
            }
            else {
                Destroy(gameObject);
            }
        }

        #endregion

        #region IKillable Implementation

        public void Kill () {
            Despawn(true);
        }

        #endregion

        public void Explode () {
            Instantiate(deathParticles, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        [Header("AI Logic")]
        public bool isActive;

        public List<Preference> preferences;
        public AIBehaviour defaultBehaviour;

        void FixedUpdate () {
            if (isActive) {
                Act();
            }
        }

        void Act () {
            Action currentAction = null;
            foreach (Preference pref in preferences) {
                if (pref.conditions.Aggregate(true, (b, c) => b && c.Evaluate(this))) {
                    currentAction = pref.behaviour.GetAction(this);
                    break;
                }
            }
            if (currentAction == null) {
                currentAction = defaultBehaviour.GetAction(this);
            }
            if (currentAction != null) {
                currentAction.Execute(this);
            }
        }

    }

}
