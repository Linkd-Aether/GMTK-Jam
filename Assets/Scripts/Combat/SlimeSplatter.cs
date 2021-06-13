using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Combat {
    public class SlimeSplatter : GFXobject
    {

        public AudioClip[] clips;
        private AudioSource source;


        protected override void Start() {
            base.Start();
        
            // ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
            // var main = ps.main;
            // main.startColor = baseColor;
            // ps.transform.localScale *= transform.localScale.x;

            source = GetComponent<AudioSource>();
            source.clip = clips[Random.Range(0, clips.Length)];
            source.Play();
        }

        protected virtual void EndSplatter() {
            Destroy(this.gameObject);
        }
    }
}