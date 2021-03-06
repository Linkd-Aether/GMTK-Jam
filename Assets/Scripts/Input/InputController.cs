using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Input {
    public class InputController : MonoBehaviour
    {

        public SFXController sfx;

        private bool mouseEnabled = false;

        // Get movement vector
        public Vector2 GetMovementInput() {
            Vector2 input = new Vector2();
            input.x = UnityEngine.Input.GetAxisRaw("Horizontal");
            input.y = UnityEngine.Input.GetAxisRaw("Vertical");
            if (input.x != 0 || input.y != 0)
            {
                sfx.Play(11 + Random.Range(0, 1));
            }
            return input.normalized;
        }

        // Get aim location
        public Vector2 GetMouseLocation() {
            Vector2 screenPos = UnityEngine.Input.mousePosition;
            Vector2 worldPos =  Camera.main.ScreenToWorldPoint(screenPos);
            return worldPos;
        }

        // Get mouse direction from origin
        public Vector2 GetMouseDirection(Vector2 origin) {
            Vector2 mousePos = GetMouseLocation();
            return (mousePos - origin).normalized;
        }

        // Get mouse button status
        public bool GetClick(int buttonID) {
            if (!mouseEnabled) return false;
            return UnityEngine.Input.GetMouseButtonDown(buttonID);
        }

        // Get key status
        public bool GetKeyDown(KeyCode key) {
            return UnityEngine.Input.GetKeyDown(key);
        }

        public void MouseEnable(bool state) {
            mouseEnabled = state;
        }
    }
}
