using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Movement;
using Game.Input;


namespace Game.Player {
    [RequireComponent(typeof(Mover)), RequireComponent(typeof(InputController))]
    public class Player : MonoBehaviour
    {
        // Components & References
        private Mover mover;
        private InputController inputController;


        private void Start() {
            mover = GetComponent<Mover>();
            inputController = GetComponent<InputController>();
        }

        private void FixedUpdate() {
            Vector2 input = inputController.GetMovementInput();
            mover.UpdatePosition(input);
        }
    }
}