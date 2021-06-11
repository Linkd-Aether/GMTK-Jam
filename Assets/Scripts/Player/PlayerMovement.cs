using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MoveCharacter
{
    // Update is called once per frame
    void Update()
    {
        UpdatePosition(KeyInput.GetKeyVector());
    }
}
