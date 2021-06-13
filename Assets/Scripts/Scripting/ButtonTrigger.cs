using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Utils;


namespace Game.Scripting {
    public class ButtonTrigger : Trigger
    {
        // Constants
        private static Sprite BUTTON_PRESSED_SPRITE, BUTTON_UNPRESSED_SPRITE;
        
        // Variables
        public bool buttonReleases = false;
        public bool onlyPlayerActivates = false;
        public bool invisible = false;
        
        // Components & References
        private SpriteRenderer spriteRenderer;


        private void Awake() {
            if (!invisible)
            {
                BUTTON_PRESSED_SPRITE = Resources.Load<Sprite>("Sprites/Objects/floor button/Button (2)");
                BUTTON_UNPRESSED_SPRITE = Resources.Load<Sprite>("Sprites/Objects/floor button/Button (1)");
            }

            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (!onlyPlayerActivates || other.tag == "Player") {
                if (!triggerState) TriggerOn();
            }
        }

        private void OnTriggerExit2D(Collider2D other) {
            if (!onlyPlayerActivates || other.tag == "Player") {
                if (triggerState && buttonReleases) TriggerOff();
            }
        }

        protected override void TriggerOn() {
            base.TriggerOn();
            if(!invisible)spriteRenderer.sprite = BUTTON_PRESSED_SPRITE;
        }
        
        protected override void TriggerOff() {
            base.TriggerOff();
            if(!invisible)spriteRenderer.sprite = BUTTON_UNPRESSED_SPRITE;
        }
    }
}