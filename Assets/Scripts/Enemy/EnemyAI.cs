using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace Game.Enemy {
    public class EnemyAI : MonoBehaviour
    {
        private enum EnemyState {
            Patrol,
            Pursue,
            Pounce
        }

        // Variables
        private EnemyState state = EnemyState.Patrol;
        public float speed = 200f; // remove later
        public Transform target;

        public float patrolSearchDistance = 10f;
        public float pursueWaypointRange = 1.2f;
        public float pursuePathGenerateRate = 0.5f;
        public float pursueTargetPounceRange = 3f;
        public float pouncingPower = 500f; // remove later
        public float pouncingPreTime = .75f;
        public float pouncingPostTime = .25f;
        private int currentWaypoint;

        // Components & References
        private Seeker seeker;
        private Rigidbody2D rb;

        private Path path;
        
        
        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            seeker = GetComponent<Seeker>();
        }

        private void FixedUpdate()
        {
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
                Debug.Log($"{transform.name} Shifting to Patrol State!");
                ResetPath();
                state = EnemyState.Patrol;
            }

            private void StateToPursue() {
                Debug.Log($"{transform.name} Shifting to Pursue State!");
                StartCoroutine(PathGenerate());
                state = EnemyState.Pursue;
            }

            private void StateToPounce() {
                Debug.Log($"{transform.name} Shifting to Pounce State!");
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
                }
            }

            private bool PatrolForTarget() {
                Vector2 direction = DirectionToTarget();
                RaycastHit2D hit = Physics2D.Raycast(rb.position, direction, patrolSearchDistance);
                
                return (hit.collider != null && hit.collider.tag == "Player");
            }
        #endregion

        #region Pursue State & Path Generation
            // Update logic for the Pursue state
            private void PursueUpdate() {
                if (path == null) return;

                float distanceToTarget = Vector2.Distance(rb.position, target.position);
                if (distanceToTarget < pursueTargetPounceRange) {
                    StateToPounce();
                    return;
                }

                float distanceToWaypoint = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
                if (distanceToWaypoint < pursueWaypointRange) {
                    currentWaypoint++;
                }

                Vector2 direction = ((Vector2) path.vectorPath[currentWaypoint] - rb.position).normalized;
                // pass direction to mover
                
                Vector2 force = direction * speed * Time.fixedDeltaTime; //remove later
                rb.AddForce(force);
            }
        
            // Generate a path to the target; is generally called via a InvokeRepeating
            private IEnumerator PathGenerate() {
                float timeSinceLastPath = pursuePathGenerateRate;
                while (true) {
                    if (timeSinceLastPath >= pursuePathGenerateRate && seeker.IsDone()) {
                        seeker.StartPath(rb.position, target.position, OnPathGenerated);
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
    
        #region Pounce State
            // Action for the Patrol state
            private IEnumerator PounceOnTarget() {
                Vector2 direction = DirectionToTarget();
                yield return new WaitForSeconds(pouncingPreTime);
                Pounce(direction); //change to mover
                yield return new WaitForSeconds(pouncingPostTime);
                StateToPursue();
            }

            // Move to mover
            private void Pounce(Vector2 dir) {
                Vector2 force = dir * pouncingPower; //remove later
                rb.AddForce(force);
            }
        #endregion

        #region Util Function
            // Returns the direction to the target
            private Vector2 DirectionToTarget() {
                return ((Vector2) target.position - rb.position).normalized;
            }
        #endregion
    }
}