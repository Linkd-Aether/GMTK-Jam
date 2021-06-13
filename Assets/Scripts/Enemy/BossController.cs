using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Game.Character;
using Game.Combat;


namespace Game.Enemy {
    public class BossController : SlimeController
    {
        // Constants
        private enum BossState {
            Idle,
            Jump,
        }

        private static float IN_AIR_TIME_AVERAGE = 1.5f;
        private static float IN_AIR_TIME_VARIATION = 1f;
        private static float BETWEEN_JUMP_TIME_AVERAGE = 4f;
        private static float BETWEEN_JUMP_TIME_VARIATION = 1f;
        private static float BETWEEN_MOVE_TIME_AVERAGE = .5f;
        private static float BETWEEN_MOVE_TIME_VARIATION = .25f;
        private static float MOVEMENT_STRENGTH_AVERAGE = 15f;
        private static float MOVEMENT_STRENGTH_VARIATION = 5f;

        private static float BOSS_BASE_MASS = 2.5f;
        private static float BOSS_MASS_PER_SLIME = .05f;

        private static float JUMP_BASE_DAMAGE = 2f;
        private static float IDLE_BASE_DAMAGE = 1f;
        private static float SIZE_TO_DAMAGE_FACTOR = .05f;
        private static float DAMAGE_TO_KNOCKBACK_FACTOR = 5f;

        // Variables
        public SlimeController playerTarget;
        public UnityEvent slimeDeathEvents;
        public bool active = false;

        private BossState state;
        private float timeToNextJump;
        private float timeToNextMove;
        private bool readyToJump;

        // Components & References
        private new ParticleSystem particleSystem;

        public AudioClip[] roar;

        public MusicController musicController;

        private AudioSource audioSource;

        protected override void Awake() {
            base.Awake();

            particleSystem = GetComponentInChildren<ParticleSystem>();
        }

        protected override void Start()
        {
            baseMass = BOSS_BASE_MASS;
            massPerSlime = BOSS_MASS_PER_SLIME;
            
            base.Start();

            timeToNextJump = 2 * CalculateWithAvgVar(BETWEEN_JUMP_TIME_AVERAGE, BETWEEN_JUMP_TIME_VARIATION);
            timeToNextMove = 2 * CalculateWithAvgVar(BETWEEN_MOVE_TIME_AVERAGE, BETWEEN_MOVE_TIME_VARIATION);

            this.audioSource = GetComponentInChildren<AudioSource>();
        }

        protected override void Update() {
            if (alive && active) {
                base.Update();

                switch (state) {
                    case (BossState.Idle):
                        IdleMovement();
                        break;
                    case (BossState.Jump):
                        break;
                }
            }
        }

        #region Animation/Activation Hooks
            public void ActivateBoss() {
                if (!active) {
                    active = true;
                    animator.SetBool("Roar", true);
                    state = BossState.Idle;
                }
            }

            private void BeginJumpCycle() {
                animator.SetBool("Roar", true);
                animator.SetBool("Jumping", true); 
                state = BossState.Jump;
            }

            private void EndRoar() { 
                animator.SetBool("Roar", false);
            }

            private void EndJump() {
                animator.SetBool("Jumping", false); 
                StartCoroutine(PrepareLanding());
            }

            private void RoarSound()
            {
                audioSource.clip = roar[Random.Range(0, roar.Length)];
                audioSource.Play();
            }

            private void SpawnLanding() {
                int totalSpawns = Random.Range(0, 4);
                int freeSpawns = Random.Range(0, totalSpawns);

                FreeSlime.SpawnFreeSlimes(freeSpawns, transform.position, Vector2.down, slimeHealth.GetCurrentColor());
                EnemyController.SpawnSlimes(totalSpawns - freeSpawns, transform.position, Vector2.down, playerTarget);
            }

            private void EndLand() {
                animator.SetBool("Landing", false); 
                state = BossState.Idle;
            }

            private IEnumerator PrepareLanding() {
                float timeInAir = CalculateWithAvgVar(IN_AIR_TIME_AVERAGE, IN_AIR_TIME_VARIATION);
                transform.position = playerTarget.transform.position;

                while (timeInAir > 0) {
                    mover.UpdateMoverDirection(DirectionToTarget());

                    timeInAir -= Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }
                
                mover.UpdateMoverDirection(Vector2.zero);

                var main = particleSystem.main;
                main.playOnAwake = true;
                main.startColor = slimeHealth.GetCurrentColor();
                
                animator.SetBool("Landing", true);
            }
        #endregion

        #region Behavior
            private void IdleMovement() {
                timeToNextMove -= Time.deltaTime;
                timeToNextJump -= Time.deltaTime;

                if (timeToNextMove < 0) {
                    if (timeToNextJump < 0) {
                        BeginJumpCycle();
                        timeToNextJump = CalculateWithAvgVar(BETWEEN_JUMP_TIME_AVERAGE, BETWEEN_JUMP_TIME_VARIATION);
                    } else {
                        RandomMovement();
                    }
                    timeToNextMove = CalculateWithAvgVar(BETWEEN_MOVE_TIME_AVERAGE, BETWEEN_MOVE_TIME_VARIATION);
                }
            }

            private void RandomMovement() {
                Vector2 direction = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f)).normalized;
                if (playerTarget != null && playerTarget.isAlive()) {
                    float rage = CalculateRage();
                    float randomValue = Random.Range(0f, 1f);

                    if (rage > randomValue) direction = DirectionToTarget();
                }                

                float strength = CalculateWithAvgVar(MOVEMENT_STRENGTH_AVERAGE, MOVEMENT_STRENGTH_VARIATION);

                mover.ImpulseForce(direction, strength);
            }
        #endregion

        #region Helper Functions
            // Returns the direction to the target
            private Vector2 DirectionToTarget() {
                return ((Vector2) playerTarget.transform.position - (Vector2) transform.position).normalized;
            }

            private float CalculateWithAvgVar(float avg, float var) {
                return avg + Random.Range(-var * CalculateRage(), var);
            }

            private float CalculateRemainingHealthPercent() {
                return (slimeHealth.slimeCount) / ((slimeHealth.colorPerStage.Length - 1) * slimeHealth.slimePerStage);
            }

            private float CalculateRage() {
                // Low during beginning of fight, high during middle, low again at end
                float percentLeft = CalculateRemainingHealthPercent();

                if (percentLeft < .1f || percentLeft == 1f) {
                    return .05f;
                } else {
                    return Mathf.Sqrt(Mathf.Clamp(1 - percentLeft, .15f, 1f));
                }
            }

            public void bossMusic()
            {
                musicController.ChangeSong(2);
            }
        #endregion

        #region Collision & Damage
            private void OnCollisionEnter2D(Collision2D other) {
                if (other.collider.gameObject.tag == "Player") {
                    SlimeController playerSlime = other.gameObject.GetComponent<SlimeController>();

                    float damage = CalculateDamage();
                    playerSlime.ChangeHealth(-damage);

                    Vector2 knockbackDir = (other.GetContact(0).point - (Vector2) transform.position).normalized;
                    float knockbackStrength = damage * DAMAGE_TO_KNOCKBACK_FACTOR;
                    playerSlime.HitByKnockback(knockbackDir, knockbackStrength);
                }
            }

            private float CalculateDamage() {
                float baseDamage, massDamage;
                if (state == BossState.Jump) baseDamage = JUMP_BASE_DAMAGE;
                else baseDamage = IDLE_BASE_DAMAGE;
                
                massDamage = SIZE_TO_DAMAGE_FACTOR * slimeHealth.slimeCount;

                return baseDamage + massDamage;
            }
        #endregion

        protected override void SlimeDeathEnded() {
            base.SlimeDeathEnded();

            slimeDeathEvents.Invoke();
        }
    }
}