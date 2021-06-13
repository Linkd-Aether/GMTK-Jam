using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Scripting {
    public class GroupFlagTrigger : Trigger
    {
        // Variables
        public int numberOfTriggersNeeded;
        private int currentlyTriggered;

        
        public void ActivateTrigger() {
            currentlyTriggered++;
            CheckTrigger();
        }

        public void DeactivateTrigger() {
            currentlyTriggered--;
            CheckTrigger();
        }

        private void CheckTrigger() {
            if (!triggerState && currentlyTriggered >= numberOfTriggersNeeded) {
                TriggerOn();
            } else if (triggerState && currentlyTriggered < numberOfTriggersNeeded) {
                TriggerOff();
            } 
        }
    }
}