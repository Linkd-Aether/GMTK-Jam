using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Utils {
    public class OtherUtils : MonoBehaviour
    {

        
        static public Quaternion DirectionToAngle(Vector2 direction) {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            return Quaternion.Euler(0, 0, angle);
        }

        static public Vector2 AngleToDirection(float angle) {
            Vector2 dir = new Vector2();
            dir.x = Mathf.Cos(angle);
            dir.y = Mathf.Sin(angle);
            return dir.normalized;
        }
    }
}