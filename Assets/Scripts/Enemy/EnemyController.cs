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
            // Pounce
        }

        // Variables
        private EnemyState state = EnemyState.Patrol;
        public Transform target;

        public float searchDistance = 10f;
        public float withinTargetRange = 3f;
        public float withinWaypointRange = 1.2f;
        public float pathGenerationRate = 1f;


        // public float pouncingPower = 500f; // remove later
        // public float pouncingPreTime = .75f;
        // public float pouncingPostTime = .25f;

        private int currentWaypoint;


        // Components & References
        private Seeker seeker;

        private Path path;
        
        
        protected override void Awake() {
            base.Awake();
            seeker = GetComponent<Seeker>();
        }

        private void FixedUpdate() {
            switch (state) {
                case (EnemyState.Patrol):
                    PatrolUpdate();
                    break;
                case (EnemyState.Pursue):
                    PursueUpdate();
                    break;
                // case (EnemyState.Pounce):
                //     break;
            }
        }

        #region State Changes
            private void StateToPatrol() {
                Debug.Log($"{transform.name} Shifting to Patrol State!");
                ResetPath();
                state = EnemyState.Patrol;
            }

            private void StateToPursue() {
                Debug.Log($"{transform.name} Shifting to Pursue State!");
                StartCoroutine(PathGenerate());
                state = EnemyState.Pursue;
            }

            // private void StateToPounce() {
            //     Debug.Log($"{transform.name} Shifting to Pounce State!");
            //     ResetPath();
            //     state = EnemyState.Pounce;
            //     StartCoroutine(PounceOnTarget());
            // }
        #endregion

        #region Patrol State
            // Update logic for the Patrol state
            private void PatrolUpdate() {
                if (PatrolForTarget()) {
                    StateToPursue();
                }
            }

            private bool PatrolForTarget() {
                Vector2 direction = DirectionToTarget();
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, searchDistance);
                
                return (hit.collider != null && hit.collider.tag == "Player");
            }
        #endregion

        #region Pursue State & Path Generation
            // Update logic for the Pursue state
            private void PursueUpdate() {
                if (path == null || currentWaypoint >= path.vectorPath.Count) return;

                float distanceToTarget = Vector2.Distance(transform.position, target.position);
                // if (distanceToTarget < withinTargetRange) {
                //     StateToPounce();
                //     return;
                // }

                float distanceToWaypoint = Vector2.Distance(transform.position, path.vectorPath[currentWaypoint]);
                if (distanceToWaypoint < withinWaypointRange) {
                    currentWaypoint++;
                }

                Vector2 moveDirection = ((Vector2) path.vectorPath[currentWaypoint] - (Vector2) transform.position).normalized;
                mover.UpdateMoverDirection(moveDirection);
            }
        
            // Generate a path to the target; is generally called via a InvokeRepeating
            private IEnumerator PathGenerate() {
                float timeSinceLastPath = pathGenerationRate;
                while (true) {
                    if (timeSinceLastPath >= pathGenerationRate && seeker.IsDone()) {
                        seeker.StartPath(transform.position, target.position, OnPathGenerated);
                        timeSinceLastPath = 0f;
                    } else {
                        timeSinceLastPath += Time.fixedDeltaTime;
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
                path = null;
                currentWaypoint = 0;
            }
        #endregion
    
        // #region Pounce State
        //     // Action for the Patrol state
        //     private IEnumerator PounceOnTarget() {
        //         Vector2 direction = DirectionToTarget();
        //         yield return new WaitForSeconds(pouncingPreTime);
        //         Pounce(direction); //change to mover
        //         yield return new WaitForSeconds(pouncingPostTime);
        //         StateToPursue();
        //     }
        // #endregion

        #region Util Function
            // Returns the direction to the target
            private Vector2 DirectionToTarget() {
                return ((Vector2) target.position - (Vector2) transform.position).normalized;
            }
        #endregion
    }
}