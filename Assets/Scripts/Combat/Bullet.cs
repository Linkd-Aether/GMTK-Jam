using Game.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Projectile
{
    // Constants
    float MAX_DISTANCE = 1000;

    // Variables
    public float speed = 50f;

    // Controllers
    public InputController input;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        CalcDistance();
    }

    // Generate lifetime from the mouse position
    void CalcDistance()
    {
        maxDistance = Mathf.Min(
            Vector2.Distance(origin, input.GetMouseLocation()),
            MAX_DISTANCE);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
