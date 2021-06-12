using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Utils {
    public static class LerpUtils
    {
        public delegate void LerpDelegate(float value);


        public static IEnumerator LerpCoroutine(LerpDelegate method, float startValue, float endValue, float lerpDuration) 
        {
            float lerpT = 0;

            while (lerpT < 1) {
                lerpT += Time.deltaTime / lerpDuration;
                lerpT = Mathf.Clamp(lerpT, 0, 1);
                float value = Mathf.Lerp(startValue, endValue, lerpT);
                method(value);
                yield return new WaitForEndOfFrame();
            }
        }

        public static Color InterpolateColors(float interpolation, Color initColor, Color finalColor) {
            Color color = new Color();
            color.r = Mathf.Lerp(initColor.r, finalColor.r, interpolation);
            color.g = Mathf.Lerp(initColor.g, finalColor.g, interpolation);
            color.b = Mathf.Lerp(initColor.b, finalColor.b, interpolation);
            color.a = Mathf.Lerp(initColor.a, finalColor.a, interpolation);
            return color;
        }
    }
}


