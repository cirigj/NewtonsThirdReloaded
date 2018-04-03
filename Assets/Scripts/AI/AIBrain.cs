using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using JBirdEngine;

namespace AI {

    public class AIBrain : ISpawnable, IKillable {

        public Ship ship;

        #region ISpawnable Implementation

        [Header("Spawn Control")]
        protected ISpawner parent;
        public float spawnRadius;
        public ParticleSystem deathParticles;

        bool _exploded;

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
            if (!_exploded) {
                var explosion = Instantiate(deathParticles, transform.position, Quaternion.identity);
                SoundHandler sound = explosion.GetComponent<SoundHandler>();
                if (sound) sound.Play();
                SpawnPowerUps();
                Destroy(gameObject);
                _exploded = true;
            }
        }

        [Header("Brain Parameters")]
        public bool isActive = true;
        public float awarenessRadius = 10f;
        public float awarenessAngle = 360f;
        public float extrapolationRadius = 25f;
        public float extrapolationTime = 5f;
        public Color foundColor;
        public Color nearbyColor;
        public Color lostColor;

        [Header("Behaviours")]
        public List<Preference> preferences;
        public AIBehaviour defaultBehaviour;

        [Header("Runtime")]
        public Vector3? extrapolatedPlayerPosition;
        public Vector3? lastKnownPlayerVelocity;
        public bool playerPositionKnown;
        public bool playerNearby;
        public float timeSincePlayerPositionKnown;
        FloatingText currentIndicator;

        [Header("Drops")]
        public LootTable lootTable;
        public float dropsMin;
        public float dropsMax;

        private Ship _playerShip;
        public Ship playerShip {
            get {
                if (_playerShip == null) {
                    _playerShip = GameController.Instance.playerShip;
                }
                return _playerShip;
            }
        }

        void Start () {
            ship.onProjectileHit += ReactToBullet;
        }

        void FixedUpdate () {
            if (isActive) {
                TrackPlayer();
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

        void TrackPlayer () {

            timeSincePlayerPositionKnown = Mathf.Clamp(timeSincePlayerPositionKnown + Time.fixedDeltaTime, 0f, extrapolationTime);

            if (playerShip.health > 0f
              && !playerShip.cloaked
              && this.ObjectWithinConeOfVision(playerShip.transform, awarenessRadius, awarenessAngle) && this.PlayerVisible()) {
                extrapolatedPlayerPosition = playerShip.transform.position;
                lastKnownPlayerVelocity = playerShip.thrustVelocity;
                PlayerFound();
            }

            else if (timeSincePlayerPositionKnown < extrapolationTime
              && playerShip.health > 0f
              && extrapolatedPlayerPosition.HasValue && this.PositionWithinRange(extrapolatedPlayerPosition.Value, extrapolationRadius)) {
                extrapolatedPlayerPosition = extrapolatedPlayerPosition + lastKnownPlayerVelocity * Time.fixedDeltaTime;
                PlayerNearby();
            }

            else {
                extrapolatedPlayerPosition = null;
                lastKnownPlayerVelocity = null;
                PlayerLost();
            }

        }

        void ResetSearchTimer () {
            timeSincePlayerPositionKnown = 0f;
        }

        void ReactToBullet (Projectile proj) {
            extrapolatedPlayerPosition = transform.position - proj.velocity / 5f;
            lastKnownPlayerVelocity = proj.velocity.normalized * -playerShip.engine.maxSpeed;
            PlayerNearby(true);
            ResetSearchTimer();
        }

        void PlayerFound () {
            ResetSearchTimer();
            if (!playerPositionKnown) {
                SpawnIndicator("!!!", foundColor);
                playerPositionKnown = true;
                playerNearby = true;
            }
        }

        void PlayerNearby (bool reactionary = false) {
            if ((!reactionary && playerPositionKnown) || (reactionary && !playerNearby)) {
                if (playerShip.health > 0f) {
                    SpawnIndicator("!?", nearbyColor);
                }
                playerPositionKnown = false;
                playerNearby = true;
            }
        }

        void PlayerLost () {
            if (playerNearby) {
                if (playerShip.health > 0f) {
                    SpawnIndicator("???", lostColor);
                }
                playerPositionKnown = false;
                playerNearby = false;
            }
        }

        void SpawnIndicator (string str, Color color) {
            if (currentIndicator) {
                currentIndicator.Kill();
                currentIndicator = null;
            }
            currentIndicator = GameController.Instance.textController.SpawnText(str, transform.position, color, 2, parent: transform);
        }

        void SpawnPowerUps () {
            int drops = Mathf.FloorToInt(UnityEngine.Random.Range(dropsMin, dropsMax));
            PowerUp choice = lootTable.GetLoot(drops).FirstOrDefault();
            if (choice != null) {
                Instantiate(choice, transform.position + UnityEngine.Random.onUnitSphere.SetY(0f).normalized, Quaternion.identity);
            }
        }

    }

}
