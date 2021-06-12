using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Interactables {
    public class SpikesContainer : MonoBehaviour
    {
        // Components & References
        private Spikes[] childSpikes;


        private void Start() {
            childSpikes = GetComponentsInChildren<Spikes>();
        }

        public void RaiseAllSpikes() {
            foreach (Spikes spikes in childSpikes) spikes.RaiseSpikes();
        }

        public void ReleaseAllSpikes() {
            foreach (Spikes spikes in childSpikes) spikes.ReleaseSpikes();
        }
    }
}