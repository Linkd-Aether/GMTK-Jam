using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Input;
using Game.Character;
using Game.UI;


namespace Game.Player {
    [RequireComponent(typeof(InputController))]
    public class PlayerController : SlimeController
    {
        // Constants
        private float SHOOTING_DELAY = .5f;
        private float PUNCHING_DELAY = 1f;

        // Variables
        private float timeToAttack = 0f;

        // Components & References
        public SlimeMeter healthBar;

        private InputController inputController;


        protected override void Awake() {
            base.Awake();
            inputController = GetComponent<InputController>();
        }

        protected override void Start() {
            base.Start();
            if (healthBar) healthBar.InitializeMeter(slimeHealth);
        }

        private void FixedUpdate() {
            Vector2 input = inputController.GetMovementInput();
            mover.UpdateMoverDirection(input);
        }

        protected override void Update() {
            base.Update();
            
            if (timeToAttack > 0) {
                timeToAttack -= Time.deltaTime;
            } else {
                if (inputController.GetClick(0)) {
                    timeToAttack = SHOOTING_DELAY;
                    
                    Vector2 dir = inputController.GetMouseDirection(transform.position);
                    SlimeBallAttack(dir);
                } else if (inputController.GetClick(1)) {
                    timeToAttack = PUNCHING_DELAY;

                    Vector2 dir = inputController.GetMouseDirection(transform.position);
                    SlimePunchAttack(dir);
                } 
            }

            // Testing
            if (inputController.GetKeyDown(KeyCode.Q)) {
                print("Slime decreased by 1");
                ChangeHealth(-1);
            } else if (inputController.GetKeyDown(KeyCode.E)) {
                print("Slime increased by 1");
                ChangeHealth(1);
            }
        }
        
        public override void ChangeHealth(float healthChange) {
            base.ChangeHealth(healthChange);
            if (healthBar) healthBar.UpdateMeter(slimeHealth);
        }
    }
}