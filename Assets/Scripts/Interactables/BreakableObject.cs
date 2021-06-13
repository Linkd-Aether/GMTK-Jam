using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Interactables {
    [RequireComponent(typeof(Rigidbody2D))]
    public class BreakableObject : GFXobject
    {
        // Variables
        public float objectHealth = 3f;

        private float healthRemaining; 
        
        // Components & References
        private Animator animator;


        protected override void Awake() {
            base.Awake();

            animator = GetComponent<Animator>();
        }

        protected override void Start() {
            base.Start();

            healthRemaining = objectHealth;    
        }

        public void TakeDamage(float damage) {
            healthRemaining -= damage;
            animator.SetBool("Hit", true);
            animator.SetFloat("Health", healthRemaining);
        }

        private void HitObjectEnd() {
            animator.SetBool("Hit", false);
        }

        private void BreakObjectEnd() {
            Destroy(this.gameObject);
        }
    }
}