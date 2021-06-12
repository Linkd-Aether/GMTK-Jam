using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Movement;
using Game.Utils;


namespace Game.Character {
    [RequireComponent(typeof(Mover))]
    public abstract class SlimeController : MonoBehaviour
    {
        // Constants
        private static float MOVEMENT_THRESHOLD = 2f;
        
        // Components & References
        protected Mover mover;
        private Animator animator;
        private Transform GFXobject;


        protected virtual void Awake() {
            mover = GetComponent<Mover>();
            animator = GetComponentInChildren<Animator>();

            GFXobject = SearchUtils.GetChildGFX(transform);
        }

        protected virtual void Update() {
            Vector2 moveDir = mover.GetMoverDirection();
            float speed = mover.GetVelocity().magnitude;

            animator.SetBool("Moving", (moveDir != Vector2.zero || speed >= MOVEMENT_THRESHOLD));
            
            if (moveDir.x != 0) {
                GFXobject.localScale = new Vector3(Mathf.Sign(moveDir.x), 1, 1);   
            }
        }
    }
}