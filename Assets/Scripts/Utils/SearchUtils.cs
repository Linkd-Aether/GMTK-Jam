using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Utils {
    public static class SearchUtils {
        

        public static Transform GetChildGFX(Transform parent) {
            foreach(Transform child in parent) {
                if (child.tag == "GFX") {
                    return child;
                }
            }
            return null;
        }
    }
}