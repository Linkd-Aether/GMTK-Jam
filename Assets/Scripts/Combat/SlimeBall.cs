using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Character;


namespace Game.Combat {
    [RequireComponent(typeof(Rigidbody))]
    public class SlimeBall : Projectile
    {
        // Constants
        private float SLIMEBALL_LIFETIME = 2.5f;

        // Variables
        public float speed = 400f;
        private float lifetime = 0;

        // Components & References
        private Animator animator;
        private Rigidbody2D rb;


        protected override void Awake()
        {
            base.Awake();
            
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
        }

        protected override void Start() {       
            rb.AddForce(direction * speed);
            // TODO: SLIMEBALL SFX !!!
        }

        private void FixedUpdate() {
            if (lifetime < SLIMEBALL_LIFETIME) {
                lifetime += Time.fixedDeltaTime;
            } else {
                EndProjectile();
            }
        }

        protected override void OnCollisionEnter2D(Collision2D other)
        {
            base.OnCollisionEnter2D(other);
            
            Vector2 contactPoint = other.GetContact(0).point;
            if (other.collider.tag == "Wall") {
                HitWall(contactPoint);
            }
        }

        protected virtual void HitWall(Vector2 contactPoint) {
            // Play Wall Hit SFX !!!
            HitSomething(contactPoint);
        }

        protected override void HitSomething(Vector2 contactPoint) {
            base.HitSomething(contactPoint);
            DespawnProjectile();
        }

        private void EndProjectile() {
            animator.SetBool("Ending", true);
        }
    }
}