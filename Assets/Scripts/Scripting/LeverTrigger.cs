using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Scripting {
    public class LeverTrigger : Trigger
    {
        // Variables
        public bool leverReleases = true;
        
        // Components & References
        private Animator animator;


        private void Awake() {
            animator = GetComponentInChildren<Animator>();
            GetComponentInChildren<ChildTrigger>().SetParentTrigger(this);
        }

        public override void OnChildTriggered(Collider2D other) {
            if (other.tag == "Player" || other.tag == "Projectile") {
                SwitchState();
            }
        }

        private void SwitchState() {
            triggerState = !triggerState;

            if (triggerState) TriggerOn();
            else TriggerOff();

            animator.SetBool("Pulled", triggerState);
        }
    }
}