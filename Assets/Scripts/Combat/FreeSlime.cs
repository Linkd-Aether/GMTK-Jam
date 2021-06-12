using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Character;


namespace Game.Combat {
    public class FreeSlime : MonoBehaviour
    {
        // Variables
        public float slimeValue = 1f;


        private void Awake() {
            
        }

        public void PickedUp() {
            // TODO: Slime Removal !!!
            Destroy(this.gameObject);
        }

        
        private void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy") {
                SlimeController slimeController = other.gameObject.GetComponent<SlimeController>();
                if (slimeController != null) {
                    slimeController.ChangeHealth(slimeValue);
                    PickedUp();
                }
            }
        }
    }
}