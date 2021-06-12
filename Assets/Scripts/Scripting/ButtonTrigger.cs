using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Utils;


namespace Game.Scripting {
    public class ButtonTrigger : Trigger
    {
        // Constants
        private static Sprite BUTTON_PRESSED_SPRITE;
        private static Sprite BUTTON_UNPRESSED_SPRITE;
        
        // Variables
        public bool buttonReleases = false;
        public bool onlyPlayerActivates = false;
        
        // Components & References
        private SpriteRenderer spriteRenderer;


        private void Awake() {
            BUTTON_PRESSED_SPRITE = Resources.Load<Sprite>("Sprites/Prototyping/Circle");//!!!
            BUTTON_UNPRESSED_SPRITE = Resources.Load<Sprite>("Sprites/Prototyping/Square");//!!!

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
            spriteRenderer.sprite = BUTTON_PRESSED_SPRITE;
        }
        
        protected override void TriggerOff() {
            base.TriggerOff();
            spriteRenderer.sprite = BUTTON_UNPRESSED_SPRITE;
        }
    }
}