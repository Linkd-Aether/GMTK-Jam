using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Game.Character;


namespace Game.Enemy {
    [RequireComponent(typeof(Seeker))]
    public class EnemyController : SlimeController
    {
        private enum EnemyState {
            Patrol,
            Pursue,
            Pounce
        }

        // Variables
        private EnemyState state = EnemyState.Patrol;
        public Transform target;

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
        }

        protected override void Update() {
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
                if (PatrolForTarget()) {
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

                float distanceToTarget = Vector2.Distance(transform.position, target.position);
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
                while (true) {
                    if (timeSinceLastPath >= pathGenerationRate && seeker.IsDone()) {
                        seeker.StartPath(transform.position, target.position, OnPathGenerated);
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
                mover.ImpulseForce(direction, pouncingStrength);
                yield return new WaitForSeconds(pouncingPostTime);
                StateToPursue();
            }
        #endregion

        #region Helper Functions
            // Returns the direction to the target
            private Vector2 DirectionToTarget() {
                return ((Vector2) target.position - (Vector2) transform.position).normalized;
            }

            // Generate a random Vector2
            private Vector2 RandomDirection() {
                Vector2 direction = new Vector2();
                direction.x = Random.Range(-1f, 1f);
                direction.y = Random.Range(-1f, 1f);
                return direction.normalized;
            }

            // Calculate a weighted value for the given direction based off slimes in that direction and open distance
            private float WeightDirection(Vector2 direction) {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, searchDistance);
                
                float openDistance = hit.distance;
                
                float colliderType; // 1: slime, 1.5: wall/other, 2: player, 3: open
                if (hit.collider == null) colliderType = 3f;
                else if (hit.collider.tag == "Enemy") colliderType = 0f;
                else if (hit.collider.tag == "Player") colliderType = 2f;
                else colliderType = 1.5f;

                float weight = colliderType * openDistance;

                return weight;
            }
        #endregion
    }
}