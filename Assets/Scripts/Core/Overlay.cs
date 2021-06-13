using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Utils;

namespace Game.Core {
    public class Overlay : GFXobject
    {
        // Constants
        private static float SHOW_ALPHA = 175f/255f;
        private static float PAUSE_TIME = .5f;
        private static float UNPAUSE_TIME = .4f;


        public void SetState(bool raised) {
            if (raised) SetAlpha(SHOW_ALPHA);
            else SetAlpha(0);
        }

        public IEnumerator RaiseOverlay() {
            yield return StartCoroutine(LerpUtils.LerpCoroutine(SetAlpha, 0, SHOW_ALPHA, PAUSE_TIME));
        }

        public IEnumerator LowerOverlay() {
            yield return StartCoroutine(LerpUtils.LerpCoroutine(SetAlpha, SHOW_ALPHA, 0, UNPAUSE_TIME));
        }
    }
}