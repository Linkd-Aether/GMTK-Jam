using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Input;
using Game.Character;


namespace Game.Player {
    [RequireComponent(typeof(InputController))]
    public class PlayerController : SlimeController
    {
        // Components & References
        private InputController inputController;


        protected override void Awake() {
            base.Awake();
            inputController = GetComponent<InputController>();
        }

        private void FixedUpdate() {
            Vector2 input = inputController.GetMovementInput();
            mover.UpdateMoverDirection(input);
        }

        protected override void Update() {
            base.Update();
            
            // Testing
            if (inputController.GetKeyDown(KeyCode.Q)) {
                print("Slime decreased by 2");
                ChangeHealth(-5);
            } else if (inputController.GetKeyDown(KeyCode.E)) {
                print("Slime increased by 2");
                ChangeHealth(5);
            }
        }
    }
}