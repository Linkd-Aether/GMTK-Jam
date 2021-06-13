using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Character;


namespace Game.Interactables {
    public class Spikes : MonoBehaviour
    {
        // Constants
        private static Sprite SPIKES_RAISED_SPRITE, SPIKES_LOWERED_SPRITE;
        private static float SPIKE_DAMAGE = 1.5f;
        private static float SPIKE_KNOCKBACK = 10f;

        // Variable
        public bool spikesControlledByTime = true;
        public float spikesTimeUp = 3f;

        // Components & References
        private new Collider2D collider;
        private SpriteRenderer spriteRenderer;


        private void Awake() {
            SPIKES_RAISED_SPRITE = Resources.Load<Sprite>("Sprites/Objects/trap/2");
            SPIKES_LOWERED_SPRITE = Resources.Load<Sprite>("Sprites/Objects/trap/1");
            
            collider = GetComponent<Collider2D>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        public void RaiseSpikes() {
            collider.enabled = true;
            spriteRenderer.sprite = SPIKES_RAISED_SPRITE;
            if (spikesControlledByTime) StartCoroutine(ReleaseSpikesAfterDelay());
        }

        public void ReleaseSpikes() {
            collider.enabled = false;
            spriteRenderer.sprite = SPIKES_LOWERED_SPRITE;
        }

        private IEnumerator ReleaseSpikesAfterDelay(){
            float time = 0;
            while (time < spikesTimeUp) {
                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            ReleaseSpikes();
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy") {
                SlimeController slime = other.gameObject.GetComponent<SlimeController>();
                slime.ChangeHealth(-SPIKE_DAMAGE);

                slime.HitByKnockbackOppositeDirection(SPIKE_KNOCKBACK);
            }
        }
    }
}