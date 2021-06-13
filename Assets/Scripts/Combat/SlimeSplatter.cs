using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Utils;


namespace Game.Combat {
    public class SlimeSplatter : GFXobject
    {
    
        protected override void Start() {
            base.Start();
            // TODO: Play Splatter SFX !!!    
        }

        protected virtual void DestroySplatter() {
            Destroy(this.gameObject);
        }
    }
}