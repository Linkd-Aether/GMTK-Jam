using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Movement {
    public class Mover : MonoBehaviour
    {
        // Variables        
        public float speed = 100;

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

        // Change the mover direction
        public void UpdateMoverDirection(Vector3 direction) {
            moveDir = direction;
        }

        // Return the current mover direction
        public Vector2 GetMoverDirection() {
            return moveDir;
        }

        // Return the current rigidbody velocity
        public Vector2 GetVelocity() {
            return rb.velocity;
        }
    }
}