using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Combat {
    public class SlimeSplatter : GFXobject
    {
        

        protected override void Start() {
            base.Start();
        
            ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
            var main = ps.main;
            main.startColor = baseColor;
            ps.transform.localScale *= transform.localScale.x;

            // TODO: Play Splatter SFX !!!    
        }

        protected virtual void EndSplatter() {
            Destroy(this.gameObject);
        }
    }
}