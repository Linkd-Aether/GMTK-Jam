using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Character;
using Game.Interactables;


namespace Game.Combat {
    public abstract class Projectile : GFXobject
    {
        // Constants
        private static GameObject SLIME_SPLATTER_PREFAB;
        
        // Variables
        public Vector2 direction;

        protected float slimeValue;
        protected SlimeController shooter;


        protected override void Awake() {
            base.Awake();

            SLIME_SPLATTER_PREFAB = Resources.Load<GameObject>("Prefabs/Combat/SlimeSplatter");
        }

        public void SetupProjectile(SlimeController shotBy, Vector2 dir, float slimeCost, Color slimeColor) {
            shooter = shotBy;
            direction = dir;
            slimeValue = slimeCost;
            SetBaseColor(slimeColor);
        }

        #region Collision & Projectile Handling
            protected virtual void OnCollisionEnter2D(Collision2D other) {
                if (other.gameObject != shooter.gameObject) {
                    Vector2 contactPoint = other.GetContact(0).point;
                    HitSomething(contactPoint);

                    if (other.collider.tag == "Enemy" || other.collider.tag == "Player") {
                        SlimeController slimeController = other.collider.GetComponent<SlimeController>();
                        if (slimeController != shooter) {
                            HitSlime(slimeController, contactPoint);
                        }
                    } else if (other.collider.tag == "Breakable") {
                        BreakableObject breakableObject = other.collider.GetComponent<BreakableObject>();
                        HitBreakable(breakableObject, contactPoint);
                    }
                }
            }

            protected virtual void HitSlime(SlimeController slime, Vector2 contactPoint) {
                slime.ChangeHealth(-slimeValue);
                // Play Slime Hit SFX !!!
            }
            protected virtual void HitBreakable(BreakableObject breakable, Vector2 contactPoint) {
                breakable.TakeDamage(slimeValue);
                // Play Breakable Object Hit SFX !!!
            }
            protected virtual void HitSomething(Vector2 contactPoint) {
                SpawnSplatter(contactPoint);
            }
        #endregion

        #region Projectile Logic
            protected virtual void DespawnProjectile() {
                FreeSlime.SpawnFreeSlimes(Mathf.FloorToInt(slimeValue), transform.position, direction, baseColor);
                Destroy(this.gameObject);
            }

            protected virtual void SpawnSplatter(Vector2 position) {
                GameObject splatterObj = Instantiate(SLIME_SPLATTER_PREFAB, position, Quaternion.Euler(0, 0, Random.Range(-180, 180)));
                splatterObj.transform.localScale = transform.localScale;

                SlimeSplatter splatter = splatterObj.GetComponent<SlimeSplatter>();
                splatter.SetBaseColor(baseColor);
            }

            protected virtual void ProjectileKnockbackOnSlime(SlimeController slime, Vector2 contactPoint, float strengthFactor) {
                float strength = transform.localScale.x * strengthFactor;
                Vector2 hitDir = (contactPoint - (Vector2) transform.position).normalized;
                slime.HitByKnockback(hitDir, strength);
            }
        #endregion
    }  
}