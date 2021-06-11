using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Movement {
    public class Mover : MonoBehaviour
    {
        public Rigidbody2D rb;
        public float speed = 100;

        // Change position based on speed and vector
        public void UpdatePosition(Vector3 dir)
        {
            Vector2 force = dir * speed * Time.fixedDeltaTime;
            rb.AddForce(force, ForceMode2D.Impulse);
        }
    }
}