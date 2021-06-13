using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Scripting {
    public class ChildTrigger : MonoBehaviour
    {
        // Variables
        private Trigger parentTrigger;


        public void SetParentTrigger(Trigger parent) {
            parentTrigger = parent;
        }

        private void OnCollisionEnter2D(Collision2D other) {
            parentTrigger.OnChildTriggered(other.collider); 
        }
    }
}