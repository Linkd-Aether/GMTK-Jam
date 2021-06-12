using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch : Projectile
{

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        maxDistance = 100; // TODO: Measure how far the fist should go
        damage = 2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
