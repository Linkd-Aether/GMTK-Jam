using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyInput : MonoBehaviour
{
    // Get movement vector
    public static Vector3 GetKeyVector()
    {
        Vector2 kvect = new Vector2();
        kvect.x = Input.GetAxisRaw("Horizontal");
        kvect.y = Input.GetAxisRaw("Vertical");
        return kvect.normalized;
    }

    // Get aim location
    public static Vector3 GetMouseLocation()
    {
        return Input.mousePosition;
    }

    // Get mouse button status
    public static bool GetClick(int buttonID)
    {
        return Input.GetMouseButtonDown(buttonID);
    }

}
