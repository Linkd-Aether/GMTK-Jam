using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Game.Scripting {
    public abstract class Trigger : MonoBehaviour
    {
        // Variables
        public UnityEvent onTriggerEvent;
        public UnityEvent offTriggerEvent;

        protected bool triggerState = false;


        protected virtual void TriggerOn() {
            onTriggerEvent.Invoke();
            triggerState = true;
        }

        protected virtual void TriggerOff() {
            offTriggerEvent.Invoke();
            triggerState = false;
        }
    }
}
