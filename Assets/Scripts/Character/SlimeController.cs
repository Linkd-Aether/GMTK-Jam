using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Movement;
using Game.Utils;
using Game.Combat;


namespace Game.Character {
    [RequireComponent(typeof(Mover)), RequireComponent(typeof(SlimeHealth))]
    public abstract class SlimeController : GFXobject
    {
        // Constants
        private static float MOVEMENT_THRESHOLD = 2f;
        private static GameObject SLIMEBALL_PREFAB, SLIMEPUNCH_PREFAB;
        private static float SLIMEBALL_COST = 1f, SLIMEPUNCH_COST = 2f;

        // Variables
        public float baseMass = .5f;
        public float massPerSlime = .05f;
        public bool losesSlimeFromProjectile = false;
        
        // Components & References
        protected Mover mover;
        protected SlimeHealth slimeHealth;
        private Animator animator;
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
            public virtual void ChangeHealth(float healthChange) {
                float slimeAmount = slimeHealth.ChangeSlime(healthChange);
                float newMass = CalculateMassFromSlime(slimeAmount);
                mover.UpdateMass(newMass);
            }

            // Spawn a projectile based slimeball attack
            protected virtual void SlimeBallAttack(Vector2 direction) {
                Vector2 spawnPosition = transform.position + (Vector3) direction * transform.localScale.x * 1/2;

                GameObject slimeBallObj = Instantiate(SLIMEBALL_PREFAB, spawnPosition, OtherUtils.DirectionToAngle(direction));
                SlimeBall slimeBall = slimeBallObj.GetComponent<SlimeBall>();
                slimeBall.SetupProjectile(this, direction, SLIMEBALL_COST, spriteRenderer.color);

                if (losesSlimeFromProjectile) ChangeHealth(-SLIMEBALL_COST);
            }

            // Spawn a melee based slimepunch attack
            protected virtual void SlimePunchAttack(Vector2 direction) {
                Vector2 spawnPosition = transform.position + (Vector3) direction * transform.localScale.x * 1/2;

                GameObject slimePunchObj = Instantiate(SLIMEPUNCH_PREFAB, spawnPosition, OtherUtils.DirectionToAngle(direction));
                slimePunchObj.transform.localScale = transform.localScale;
                slimePunchObj.transform.parent = transform;

                SlimePunch slimePunch = slimePunchObj.GetComponent<SlimePunch>();
                slimePunch.SetupProjectile(this, direction, SLIMEPUNCH_COST, spriteRenderer.color);


                if (losesSlimeFromProjectile) ChangeHealth(-SLIMEPUNCH_COST);
            }
        #endregion

        #region 
            public void HitByKnockback(Vector2 knockbackDir, float knockbackStrength) {
                mover.ImpulseForce(knockbackDir, knockbackStrength);
            }
        #endregion

        private float CalculateMassFromSlime(float slimeAmount) {
            return baseMass + slimeAmount * massPerSlime;
        }
    }
}