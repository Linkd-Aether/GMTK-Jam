using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Pathfinding;
using Game.Character;
using Game.Utils;


namespace Game.Enemy {
    [RequireComponent(typeof(Seeker))]
    public class EnemyController : SlimeController
    {
        // Constants
        private enum EnemyState {
            Patrol,
            Pursue,
            Pounce
        }
        private static float MIN_DAMANGE_FROM_HIT = .75f;
        private static float SIZE_TO_DAMAGE_FACTOR = .15f;
        private static float DAMAGE_TO_KNOCKBACK_FACTOR = 3f;

        private static float MIN_SPAWN_DIST = 1f;
        private static float MAX_SPAWN_DIST = 1.5f;
        private static float SPAWN_TIME = .75f;
        private static GameObject SLIME_PREFAB;

        // Variables
        private EnemyState state = EnemyState.Patrol;
        public SlimeController playerTarget;
        public UnityEvent slimeDeathEvents;

        public float searchDistance = 10f;
        public float chaseDistance = 15f;
        public float withinTargetRange = 3f;
        public float withinWaypointRange = 1.2f;
        public float pathGenerationRate = 1f;
        public float pouncingStrength = 200f;
        public float pouncingPreTime = .75f;
        public float pouncingPostTime = .25f;
        public float timeRandomMovementAverage = .5f;
        public float timeRandomMovementVariation = .5f;

        private int currentWaypoint;
        private float timeRemainingMovement;

        // Components & References
        private Seeker seeker;

        private Path path;
        
        
        protected override void Awake() {
            base.Awake();

            seeker = GetComponent<Seeker>();
            LoadPrefab();
        }

        private static void LoadPrefab() {
            SLIME_PREFAB = Resources.Load<GameObject>("Prefabs/Characters/EnemySlime");
        }

        protected override void Update() {
            if (alive) {
                base.Update();
                
                switch (state) {
                    case (EnemyState.Patrol):
                        PatrolUpdate();
                        break;
                    case (EnemyState.Pursue):
                        PursueUpdate();
                        break;
                    case (EnemyState.Pounce):
                        break;
                }
            }
        }

        #region State Changes
            private void StateToPatrol() {
                ResetPath();
                state = EnemyState.Patrol;
                timeRemainingMovement = timeRandomMovementAverage * 3;
            }

            private void StateToPursue() {
                StartCoroutine(PathGenerate());
                state = EnemyState.Pursue;
            }

            private void StateToPounce() {
                ResetPath();
                state = EnemyState.Pounce;
                StartCoroutine(PounceOnTarget());
            }
        #endregion

        #region Patrol State
            // Update logic for the Patrol state
            private void PatrolUpdate() {
                if (playerTarget != null && playerTarget.isAlive() && PatrolForTarget()) {
                    StateToPursue();
                } else {
                    if (timeRemainingMovement > 0) {
                        timeRemainingMovement -= Time.deltaTime;
                    } else {
                        timeRemainingMovement = timeRandomMovementAverage + Random.Range(-1f, 1f) * timeRandomMovementVariation;
                        PatrolMovement();
                    }
                }
            }

            // Raycast in search of the target
            private bool PatrolForTarget() {
                Vector2 direction = DirectionToTarget();
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, searchDistance);
                
                return (hit.collider != null && hit.collider.tag == "Player");
            }

            // Perform a weighted random movement in an open direction
            private void PatrolMovement() {
                Vector2[] directions = new Vector2[5];
                float[] weights = new float[directions.Length];
                int bestWeightIndex = 0;

                for (int i = 0; i < directions.Length; i++) {
                    directions[i] = RandomDirection();
                    weights[i] = WeightDirection(directions[i]);
                    if (weights[i] > weights[bestWeightIndex]) {
                        bestWeightIndex = i;
                    }
                }

                float movementStrength = Random.Range(pouncingStrength / 4, pouncingStrength / 3);
                mover.ImpulseForce(directions[bestWeightIndex], movementStrength);
            }
        #endregion

        #region Pursue State & Path Generation
            // Update logic for the Pursue state
            private void PursueUpdate() {
                if (path == null || currentWaypoint >= path.vectorPath.Count) return;
                if (!playerTarget.isAlive()) {
                    StateToPatrol();
                    return;
                }

                float distanceToTarget = Vector2.Distance(transform.position, playerTarget.transform.position);
                if (distanceToTarget < withinTargetRange) {
                    StateToPounce();
                    return;
                } else if (distanceToTarget > chaseDistance) {
                    StateToPatrol();
                    return;
                }

                Vector2 moveDirection = ((Vector2) path.vectorPath[currentWaypoint] - (Vector2) transform.position).normalized;
                mover.UpdateMoverDirection(moveDirection);

                float distanceToWaypoint = Vector2.Distance(transform.position, path.vectorPath[currentWaypoint]);
                if (distanceToWaypoint < withinWaypointRange) {
                    currentWaypoint++;
                }
            }
        
            // Generate a path to the target; is generally called via a InvokeRepeating
            private IEnumerator PathGenerate() {
                float timeSinceLastPath = pathGenerationRate;
                while (playerTarget != null) {
                    if (timeSinceLastPath >= pathGenerationRate && seeker.IsDone()) {
                        seeker.StartPath(transform.position, playerTarget.transform.position, OnPathGenerated);
                        timeSinceLastPath = 0f;
                    } else {
                        timeSinceLastPath += Time.deltaTime;
                    }

                    yield return new WaitForEndOfFrame();
                }
            }

            // Function called post generation that updates the Enemy path to the new path
            private void OnPathGenerated(Path p) {
                if (!p.error) {
                    path = p;
                    currentWaypoint = 0;
                }
            }

            // Reset path and path components
            private void ResetPath() {
                StopCoroutine(PathGenerate());
                mover.UpdateMoverDirection(Vector2.zero);
                path = null;
                currentWaypoint = 0;
            }
        #endregion
    
        #region Pounce State
            // Action for the Patrol state
            private IEnumerator PounceOnTarget() {
                yield return new WaitForSeconds(pouncingPreTime/2);
                Vector2 direction = DirectionToTarget();
                yield return new WaitForSeconds(pouncingPreTime/2);
                if (alive) mover.ImpulseForce(direction, pouncingStrength);
                yield return new WaitForSeconds(pouncingPostTime);
                StateToPursue();
            }
        #endregion

        #region Helper Functions
            // Returns the direction to the target
            private Vector2 DirectionToTarget() {
                return ((Vector2) playerTarget.transform.position - (Vector2) transform.position).normalized;
            }

            // Generate a random Vector2
            private Vector2 RandomDirection() {
                Vector2 direction = new Vector2();
                direction.x = Random.Range(-1f, 1f);
                direction.y = Random.Range(-1f, 1f);
                return direction.normalized;
            }

            // Calculate a weighted value for the given direction based off factors in that direction and open distance
            private float WeightDirection(Vector2 direction) {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, searchDistance);
                
                float openDistance = hit.distance;
                
                float colliderType; // 1: slime, 1.5: wall/other, 2: player, 3: open
                if (hit.collider == null) colliderType = 3f;
                else if (hit.collider.tag == "Enemy") colliderType = 0f;
                else if (hit.collider.tag == "Player") colliderType = 2f;
                else colliderType = 1f;

                float weight = colliderType * openDistance;

                return weight;
            }
        #endregion

        #region Collision & Damage {
            private void OnCollisionEnter2D(Collision2D other) {
                if (other.collider.gameObject.tag == "Player") {
                    SlimeController playerSlime = other.gameObject.GetComponent<SlimeController>();

                    float damage = CalculateDamage();
                    playerSlime.ChangeHealth(-damage);

                    Vector2 knockbackDir = (other.GetContact(0).point - (Vector2) transform.position).normalized;
                    float knockbackStrength = damage * DAMAGE_TO_KNOCKBACK_FACTOR;
                    playerSlime.HitByKnockback(knockbackDir, knockbackStrength);
                }
            }

            private float CalculateDamage() {
                return MIN_DAMANGE_FROM_HIT + SIZE_TO_DAMAGE_FACTOR * slimeHealth.slimeCount;
            }
        #endregion

        #region Death
            protected override void SlimeDeathBegin() {
                base.SlimeDeathBegin();
                
                ResetPath();
            }

            protected override void SlimeDeathEnded() {
                base.SlimeDeathEnded();

                slimeDeathEvents.Invoke();
            }
        #endregion

        #region Spawning 
            public static void SpawnSlimes(int slimes, Vector2 origin, Vector2 dir, SlimeController player) {
                LoadPrefab();
                for (int i = 0; i < slimes; i++) {
                    EnemyController.SpawnSlime(origin, dir, player);
                }
            }

            public static void SpawnSlime(Vector2 origin, Vector2 initialDir, SlimeController player) {
                Vector2 dir = FindFreeDirection(origin, initialDir, MAX_SPAWN_DIST - MIN_SPAWN_DIST);
                float range = Random.Range(MIN_SPAWN_DIST, MAX_SPAWN_DIST);

                GameObject slimeObj = Instantiate(SLIME_PREFAB, (Vector2) origin + dir * range, Quaternion.Euler(0,0,0));
                
                EnemyController slime = slimeObj.GetComponent<EnemyController>();
                slime.SetInitValues(player);
                slime.StartCoroutine(slime.SpawnIn());
            }

            private void SetInitValues(SlimeController player) {
                playerTarget = player;
                alive = false;
            }

            private IEnumerator SpawnIn() {
                yield return StartCoroutine(LerpUtils.LerpCoroutine(SetAlpha, 0, baseColor.a, SPAWN_TIME));
                alive = true;
            }
        #endregion
    }
}