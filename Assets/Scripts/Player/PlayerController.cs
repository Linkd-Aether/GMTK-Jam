using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Input;
using Game.Character;


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
        private InputController inputController;


        protected override void Awake() {
            base.Awake();
            inputController = GetComponent<InputController>();
        }

        private void FixedUpdate() {
            if (alive) {
                Vector2 input = inputController.GetMovementInput();
                mover.UpdateMoverDirection(input);
            }
        }

        protected override void Update() {
            if (alive) {
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
                    ChangeHealth(-1);
                } else if (inputController.GetKeyDown(KeyCode.E)) {
                    ChangeHealth(1);
                }
            }
        }

        protected override void SlimeDeathEnded()
        {
            // TODO: Player Death !!!
            base.SlimeDeathEnded();
        }
    }
}