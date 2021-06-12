using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Utils;


namespace Game.Character {
    public class SlimeHealthOld : MonoBehaviour
    {
        // Constants
        private static float MINIMUM_SLIME_SIZE = .5f;
        private static float OVERFLOW_MULTIPLIER = 1.05f;
        private static float UNDERFLOW_MULTIPLIER = 0.95f;
        private static float REFLOW_CORRECTION_TIME = .1f;
        private static float SIZE_CHANGE_TIME_MODIFIER = .2f;
        private static float STAGE_CHANGE_TIME = .3f;

        // Variables
        public float slimeSizeChangeRate = .25f;
        public float slimeCount = 5f;

        public bool stageChanges;
        public float stageSizeChangeRate = .5f;
        public float stageChangeInterval = 10f;
        public Color[] stageColors;
        
        private float currentSize;
        private int currentStage;
        private float maxSlimeAmount = float.MaxValue;

        // Components & References
        private SpriteRenderer spriteRenderer;


        private void Awake() {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void Start() {
            currentSize = CalculateSize(slimeCount);
            ChangeSize(currentSize);

            if (stageChanges) {
                maxSlimeAmount = (stageColors.Length - 1) * stageChangeInterval;
                currentStage = CalculateStage(slimeCount);
                spriteRenderer.color = stageColors[currentStage];
            }
        }

        #region External Functions
            public void IncreaseSlime(float slimeChange) {
                float newSlimeCount = Mathf.Clamp(slimeCount + slimeChange, 0, maxSlimeAmount);

                if (newSlimeCount == slimeCount) {
                    // TODO: No change, Maximum Slime Health !!!
                } else {
                    StartCoroutine(SizeChange(slimeCount, newSlimeCount, OVERFLOW_MULTIPLIER));
                    if (stageChanges) {
                        int newStage = CalculateStage(newSlimeCount);
                        if (newStage > currentStage) ChangeStage(newStage);
                    }
                }
                
                slimeCount = newSlimeCount;
            }
            
            public void DecreaseSlime(float slimeChange) {
                float newSlimeCount = Mathf.Clamp(slimeCount - slimeChange, 0, maxSlimeAmount);

                if (newSlimeCount == 0) {
                    // TODO: Death !!!
                } else {
                    StartCoroutine(SizeChange(slimeCount, newSlimeCount, UNDERFLOW_MULTIPLIER));
                    if (stageChanges) {
                        int newStage = CalculateStage(newSlimeCount);
                        if (newStage < currentStage) ChangeStage(newStage);
                    }
                }

                slimeCount = newSlimeCount;
            }
        #endregion 

        #region Internal Functions
            private IEnumerator SizeChange(float prevSlime, float newSlime, float reflow_multiplier) {
                float newSize = CalculateSize(newSlime);
                float lerpTime = CalculateLerpTime(newSize);
                
                yield return LerpUtils.LerpCoroutine(ChangeSize, currentSize, newSize * reflow_multiplier, lerpTime);
                yield return LerpUtils.LerpCoroutine(ChangeSize, newSize * reflow_multiplier, newSize, REFLOW_CORRECTION_TIME);
            }
        #endregion

        #region Helper Functions
            // Calculate a size corresponding to the given amount of slime
            private float CalculateSize(float slimeValue) {
                if (stageChanges) {
                    return MINIMUM_SLIME_SIZE + (slimeValue % stageChangeInterval) * slimeSizeChangeRate + (int)(slimeValue / stageChangeInterval) * stageSizeChangeRate;
                } else {
                    return MINIMUM_SLIME_SIZE + slimeValue * slimeSizeChangeRate;
                }
            }

            // Calculate a stage number corresponding to the given amount of slime
            private int CalculateStage(float slimeValue) {
                return (int) Mathf.Floor(slimeValue / stageChangeInterval);
            }

            // Calculate a time to lerp given the new size compared to the current slime size
            private float CalculateLerpTime(float newSize) {
                return Mathf.Sqrt(Mathf.Abs(newSize - currentSize)) * SIZE_CHANGE_TIME_MODIFIER;
            }

            // Update slime size
            private void ChangeSize(float value) {
                transform.localScale = value * Vector3.one;
                currentSize = value;
            }

            // Update to the new stage
            private void ChangeStage(int stage) {
                currentStage = stage;
                
                LerpUtils.LerpDelegate ColorLerp = (lerpTime) => {
                    spriteRenderer.color = LerpUtils.InterpolateColors(lerpTime, spriteRenderer.color, stageColors[stage]);
                };
                StartCoroutine(LerpUtils.LerpCoroutine(ColorLerp, 0, 1, STAGE_CHANGE_TIME));
            }
        #endregion
    }
}