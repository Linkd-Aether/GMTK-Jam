using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Movement;
using Game.Utils;


namespace Game.Character {
    [RequireComponent(typeof(Mover)), RequireComponent(typeof(SlimeHealth))]
    public abstract class SlimeController : MonoBehaviour
    {
        // Constants
        private static float MOVEMENT_THRESHOLD = 2f;

        // Variables
        public float baseMass = .5f;
        public float massPerSlime = .05f;
        
        // Components & References
        protected Mover mover;
        protected SlimeHealth slimeHealth;
        private Animator animator;
        private Transform GFXobject;


        protected virtual void Awake() {
            mover = GetComponent<Mover>();
            slimeHealth = GetComponent<SlimeHealth>();
            animator = GetComponentInChildren<Animator>();

            GFXobject = SearchUtils.GetChildGFX(transform);
        }

        protected virtual void Start() {
            float newMass = CalculateMassFromSlime(slimeHealth.slimeCount);
            mover.UpdateMass(newMass);
        }

        protected virtual void Update() {
            Vector2 moveDir = mover.GetMoverDirection();
            float speed = mover.GetVelocity().magnitude;

            animator.SetBool("Moving", (moveDir != Vector2.zero || speed >= MOVEMENT_THRESHOLD));
            
            if (moveDir.x != 0) {
                GFXobject.localScale = new Vector3(Mathf.Sign(moveDir.x), 1, 1);   
            }
        }

        protected virtual void ChangeHealth(float healthChange) {
            float slimeAmount = slimeHealth.ChangeSlime(healthChange);
            float newMass = CalculateMassFromSlime(slimeAmount);
            mover.UpdateMass(newMass);
        }

        private float CalculateMassFromSlime(float slimeAmount) {
            return baseMass + slimeAmount * massPerSlime;
        }
    }
}