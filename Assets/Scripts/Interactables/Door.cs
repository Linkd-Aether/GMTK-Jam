using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Interactables {
    public class Door : MonoBehaviour
    {
        // Constants
        private enum DoorType {
            Double = 1, 
            Gate = 2,
            Single = 3
        }

        // Variables
        [SerializeField] private DoorType doorType = DoorType.Double;
        
        // Components & References
        private new Collider2D collider;
        private Animator animator;


        private void Awake() {
            collider = GetComponent<Collider2D>();
            animator = GetComponentInChildren<Animator>();
            animator.SetInteger("DoorType", (int) doorType);
        }

        public void OpenDoor() {
            collider.enabled = false;
            animator.SetBool("Open", true);
        }

        public void CloseDoor() {
            collider.enabled = true;
            animator.SetBool("Open", false);
        }
    }
}