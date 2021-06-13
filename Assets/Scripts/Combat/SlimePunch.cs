using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Character;


    namespace Game.Combat {
    public class SlimePunch : Projectile
    {
        // Constants
        private static float SIZE_TO_STRENGTH_FACTOR = 10f;

        // Variables
        private List<SlimeController> hitSlimes = new List<SlimeController>();


        protected override void Start() {
            // TODO: SLIMEPUNCH SFX !!!
        }

        protected override void HitSlime(SlimeController slime, Vector2 contactPoint) {
            if (!hitSlimes.Contains(slime)) {
                base.HitSlime(slime, contactPoint);

                ProjectileKnockbackOnSlime(slime, contactPoint, SIZE_TO_STRENGTH_FACTOR);
                hitSlimes.Add(slime);
            }
        }
    }
}