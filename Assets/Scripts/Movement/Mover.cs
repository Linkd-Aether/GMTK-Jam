using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Movement {
    public class Mover : MonoBehaviour
    {
        // Variables        
        public float speed = 25;

        private Vector2 moveDir;

        // Components & References
        private Rigidbody2D rb;


        private void Awake() {
            rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate() {
            Vector2 force = moveDir * speed * Time.fixedDeltaTime;
            rb.AddForce(force, ForceMode2D.Impulse);
        }

        #region Forces
            // Used for adding single impulse in the given direciton
            public void ImpulseForce(Vector2 forceDir, float forceStrength) {
                Vector2 force = forceDir * forceStrength;
                rb.AddForce(force, ForceMode2D.Impulse);
            }
        #endregion

        #region Get/Set Parameters
            // Change the mover direction
            public void UpdateMoverDirection(Vector3 direction) {
                moveDir = direction;
            }

            // Change the mass
            public void UpdateMass(float newMass) {
                rb.mass = newMass;
            }

            // Return the current mover direction
            public Vector2 GetMoverDirection() {
                return moveDir;
            }

            // Return the current rigidbody velocity
            public Vector2 GetVelocity() {
                return rb.velocity;
            }
        #endregion
    }
}