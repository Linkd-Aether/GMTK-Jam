using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Input {
    public class InputController : MonoBehaviour
    {
        // Get movement vector
        public Vector2 GetMovementInput() {
            Vector2 input = new Vector2();
            input.x = UnityEngine.Input.GetAxisRaw("Horizontal");
            input.y = UnityEngine.Input.GetAxisRaw("Vertical");
            return input.normalized;
        }

        // Get aim location
        public Vector2 GetMouseLocation() {
            // must translate mouse position to world coordinates !!!
            return UnityEngine.Input.mousePosition;
        }

        // Get mouse button status
        public bool GetClick(int buttonID) {
            return UnityEngine.Input.GetMouseButtonDown(buttonID);
        }

        // Get key status
        public bool GetKeyDown(KeyCode key) {
            return UnityEngine.Input.GetKeyDown(key);
        }
    }
}
