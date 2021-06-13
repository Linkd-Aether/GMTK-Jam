using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Combat {
    public class SlimeSplatter : GFXobject
    {


        protected override void Start() {
            base.Start();
            // TODO: Play Splatter SFX !!!    
        }

        protected virtual void EndSplatter() {
            Destroy(this.gameObject);
        }
    }
}