using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCharacter : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed = 100;

    // Change position based on speed and vector
    public void UpdatePosition(Vector3 dir)
    {
        dir = dir * speed * Time.deltaTime;
        rb.AddForce(dir, ForceMode2D.Impulse);
    }
}
