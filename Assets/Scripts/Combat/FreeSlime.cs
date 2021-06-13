using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Character;
using Game.Utils;


namespace Game.Combat {
    public class FreeSlime : GFXobject
    {
        // Constant
        private static GameObject FREE_SLIME_PREFAB;
        private static Sprite[] FREE_SLIME_VARIANTS;
        private static string LOAD_PATH = "Sprites/Slime/Combat/Slime_Ball_Projectile_Sprite_Sheet";

        private static float MIN_SPAWN_DIST = .2f;
        private static float MAX_SPAWN_DIST = .8f;
        private static float SPAWN_SIZE_AVG = 1f;
        private static float SPAWN_SIZE_VAR = .2f;

        private static float SPAWN_TIME = .5f;

        
        // Variables
        public float slimeValue;

        private bool pickable = false;


        protected override void Awake() {
            base.Awake();

            FREE_SLIME_VARIANTS = new Sprite[3];
            Sprite[] spriteSheet = Resources.LoadAll<Sprite>(LOAD_PATH);
            for (int i = 0; i < FREE_SLIME_VARIANTS.Length; i++) {
                FREE_SLIME_VARIANTS[i] = spriteSheet[i];
            }
        }

        protected override void Start() {
            base.Start();

            spriteRenderer.sprite = FREE_SLIME_VARIANTS[Random.Range(0, FREE_SLIME_VARIANTS.Length)];
        }

        private static void LoadPrefab() {
            FREE_SLIME_PREFAB = Resources.Load<GameObject>("Prefabs/Combat/FreeSlime");
        }

        public void PickedUp() {
            // TODO: Slime Removal !!!
            Destroy(this.gameObject);
        }
        
        private void OnTriggerEnter2D(Collider2D other) {
            if (pickable && other.gameObject.tag == "Player") {
                SlimeController slimeController = other.gameObject.GetComponent<SlimeController>();
                if (slimeController != null) {
                    slimeController.ChangeHealth(slimeValue * 1.5f);
                    PickedUp();
                }
            }
        }

        #region Spawning 
            public static void SpawnFreeSlimes(int freeSlimes, Vector2 origin, Vector2 dir, Color color) {
                LoadPrefab();
                for (int i = 0; i < freeSlimes; i++) {
                    FreeSlime.SpawnFreeSlime(origin, dir, color);
                }
            }

            private static void SpawnFreeSlime(Vector2 origin, Vector2 initialDir, Color color) {
                Vector2 dir = FindFreeDirection(origin, initialDir, MAX_SPAWN_DIST - MIN_SPAWN_DIST);
                float range = Random.Range(MIN_SPAWN_DIST, MAX_SPAWN_DIST);

                GameObject freeSlimeObj = Instantiate(FREE_SLIME_PREFAB, (Vector2) origin + dir * range, Quaternion.Euler(0,0,0));
                
                FreeSlime freeSlime = freeSlimeObj.GetComponent<FreeSlime>();
                freeSlime.slimeValue = Random.Range(SPAWN_SIZE_AVG-SPAWN_SIZE_VAR, SPAWN_SIZE_AVG+SPAWN_SIZE_VAR);
                freeSlime.SetBaseColor(color);
                freeSlime.StartCoroutine(freeSlime.SpawnIn());

                freeSlimeObj.transform.localScale = Vector3.one * freeSlime.slimeValue;
            }

            private IEnumerator SpawnIn() {
                yield return StartCoroutine(LerpUtils.LerpCoroutine(SetAlpha, 0, baseColor.a, SPAWN_TIME));
                pickable = true;
            }
        #endregion
    }
}