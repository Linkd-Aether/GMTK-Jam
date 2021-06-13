using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Movement;
using Game.Utils;
using Game.Combat;
using Game.UI;


namespace Game.Character {
    [RequireComponent(typeof(Mover)), RequireComponent(typeof(SlimeHealth))]
    public abstract class SlimeController : GFXobject
    {
        // Constants
        private static float MOVEMENT_THRESHOLD = 2f;
        private static GameObject SLIMEBALL_PREFAB, SLIMEPUNCH_PREFAB;
        private static float SLIMEBALL_COST = 1f, SLIMEPUNCH_COST = 2f;
        private static float DROP_SLIME_CHANCE = .5f;

        // Variables
        public float baseMass = .5f;
        public float massPerSlime = .05f;
        public bool losesSlimeFromProjectile = false;

        protected bool alive = true;
        
        // Components & References
        public SlimeMeter healthBar;
        
        protected Mover mover;
        protected SlimeHealth slimeHealth;
        protected Animator animator;
        private Transform GFXobject;


        protected override void Awake() {
            base.Awake();

            mover = GetComponent<Mover>();
            slimeHealth = GetComponent<SlimeHealth>();
            animator = GetComponentInChildren<Animator>();

            GFXobject = SearchUtils.GetChildGFX(transform);

            SLIMEBALL_PREFAB = Resources.Load<GameObject>("Prefabs/Combat/SlimeBall");
            SLIMEPUNCH_PREFAB = Resources.Load<GameObject>("Prefabs/Combat/SlimePunch");
        }

        protected override void Start() {
            base.Start();

            float newMass = CalculateMassFromSlime(slimeHealth.slimeCount);
            mover.UpdateMass(newMass);

            if (healthBar) healthBar.InitializeMeter(slimeHealth);
        }

        protected virtual void Update() {
            Vector2 moveDir = mover.GetMoverDirection();
            float speed = mover.GetVelocity().magnitude;

            animator.SetBool("Moving", (moveDir != Vector2.zero || speed >= MOVEMENT_THRESHOLD));
            
            if (moveDir.x != 0) {
                GFXobject.localScale = new Vector3(Mathf.Sign(moveDir.x), 1, 1);   
            }
        }

        #region Control Functions
            // Update SlimeHealth and RigidBody Mass
            public virtual void ChangeHealth(float healthChange, bool fromUse = false) {
                float slimeAmount = slimeHealth.ChangeSlime(healthChange);

                if (slimeAmount > 0) {
                    if (healthChange < 0 && !fromUse) {
                        // Slime is damaged
                        PlayDamageEffect(Mathf.Abs(healthChange));
                    } else if (healthChange > 0) {
                        // Slime is healed
                        PlayHealEffect(healthChange);
                    }
                    float newMass = CalculateMassFromSlime(slimeAmount);
                    mover.UpdateMass(newMass);
                } else {
                    SlimeDeathBegin();
                    alive = false;
                }

                if (healthBar) healthBar.UpdateMeter(slimeHealth);
            }

            // Spawn a projectile based slimeball attack
            protected virtual void SlimeBallAttack(Vector2 direction) {
                Vector2 spawnPosition = transform.position + (Vector3) direction * transform.localScale.x * 1/2;

                GameObject slimeBallObj = Instantiate(SLIMEBALL_PREFAB, spawnPosition, OtherUtils.DirectionToAngle(direction));
                slimeBallObj.transform.localScale = transform.localScale * 1/2;

                SlimeBall slimeBall = slimeBallObj.GetComponent<SlimeBall>();
                slimeBall.SetupProjectile(this, direction, SLIMEBALL_COST, spriteRenderer.color);

                if (losesSlimeFromProjectile) ChangeHealth(-SLIMEBALL_COST, true);
            }

            // Spawn a melee based slimepunch attack
            protected virtual void SlimePunchAttack(Vector2 direction) {
                Vector2 spawnPosition = transform.position + (Vector3) direction * transform.localScale.x * 1/2;

                GameObject slimePunchObj = Instantiate(SLIMEPUNCH_PREFAB, spawnPosition, OtherUtils.DirectionToAngle(direction));
                slimePunchObj.transform.localScale = transform.localScale;
                slimePunchObj.transform.parent = transform;

                SlimePunch slimePunch = slimePunchObj.GetComponent<SlimePunch>();
                slimePunch.SetupProjectile(this, direction, SLIMEPUNCH_COST, spriteRenderer.color);


                if (losesSlimeFromProjectile) ChangeHealth(-SLIMEPUNCH_COST, true);
            }
        #endregion

        #region Response Functions
            protected virtual void SlimeDeathBegin() {
                mover.UpdateMoverDirection(Vector3.zero);
                animator.SetBool("Death", true);
            }

            protected virtual void SlimeDeathEnded() {
                Destroy(this.gameObject);
            }

            private void PlayDamageEffect(float magnitude) {
                // TODO: Damage Effect !!!

                int freeSlimesToSpawn = 0;
                for (int i = 0; i < Mathf.FloorToInt(magnitude); i++) {
                    float randomValue = Random.Range(0f,1f);
                    if (DROP_SLIME_CHANCE > randomValue) {
                        freeSlimesToSpawn++;
                    }
                }
                FreeSlime.SpawnFreeSlimes(freeSlimesToSpawn, transform.position, -mover.GetVelocity(), baseColor);

            }

            private void PlayHealEffect(float magnitude) {
                // TODO: Heal Effect !!!
            }

            public void HitByKnockback(Vector2 knockbackDir, float knockbackStrength) {
                mover.ImpulseForce(knockbackDir, knockbackStrength);
            }

            public void HitByKnockbackOppositeDirection(float knockbackStrength) {
                Vector2 currentDirection = mover.GetMoverDirection();
                if (currentDirection == Vector2.zero) currentDirection = mover.GetVelocity().normalized;
                
                HitByKnockback(-currentDirection, knockbackStrength);
            }
        #endregion

        protected float CalculateMassFromSlime(float slimeAmount) {
            return baseMass + slimeAmount * massPerSlime;
        }

        public bool isAlive() { return alive; }
    }
}