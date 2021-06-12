using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Utils;


namespace Game.Character {
    public class SlimeHealth : MonoBehaviour
    {
        // Constants
        private static float COLOR_CHANGE_TIME = .5f;
        private static float SIZE_CHANGE_TIME = .75f;
        private static float OVERFLOW_MULTIPLIER = 1.05f;
        private static float OVERFLOW_TIME = .1f;
        
        // Variables
        public float slimeCount = 5f;
        
        public float sizeMinimum = .35f;
        public float sizeChangePerSlime = .05f;
        
        public bool usesStages = false;
        public float slimePerStage = .5f;
        public float sizeChangePerStage = .3f;
        public Color[] colorPerStage;

        private Coroutine currentCoroutine;
        private float currentSize = 1;
        private int currentStage;
        private float maxSlimeCount = float.MaxValue;

        // Components & References
        SpriteRenderer spriteRenderer;


        private void Awake() {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void Start() {
            if (usesStages) {
                currentStage = CalculateStage(slimeCount);
                spriteRenderer.color = CalculateColor(slimeCount);
                maxSlimeCount = (colorPerStage.Length - 1) * slimePerStage;
            }

            currentSize = CalculateSize(slimeCount);
            SetSize(currentSize);
        }

        #region Slime Count Changes
            public float ChangeSlime(float slimeChange) {
                float newSlimeCount = Mathf.Clamp(slimeCount + slimeChange, 0, maxSlimeCount);

                if (newSlimeCount != slimeCount) {
                    slimeCount = newSlimeCount;
                    if (currentCoroutine != null) StopCoroutine(currentCoroutine);

                    if (usesStages) {
                        int newStage = CalculateStage(slimeCount);
                        if (newStage != currentStage) {
                            ChangeColor(currentStage, newStage);
                            SetStage(newStage);
                        }
                    }

                    currentCoroutine = StartCoroutine(ChangeSize());
                }

                return slimeCount;
            }
        #endregion

        #region Color Functions
            private void SetColor(Color newColor) {
                spriteRenderer.color = newColor;
            }

            // Calculate Color based off slimeValue
            private Color CalculateColor(float slimeValue) {
                int stage = CalculateStage(slimeValue);
                return colorPerStage[stage];
            }

            // Changes color between stages using Lerp
            private void ChangeColor(int prevStage, int newStage) {
                print($"{prevStage}");
                Color fromColor = colorPerStage[prevStage];
                Color toColor = colorPerStage[newStage];

                LerpUtils.LerpDelegate ColorLerp = (lerpTime) => {
                    spriteRenderer.color = LerpUtils.InterpolateColors(lerpTime, fromColor, toColor);
                };
                StartCoroutine(LerpUtils.LerpCoroutine(ColorLerp, 0, 1, COLOR_CHANGE_TIME));
            }
        #endregion

        #region Size Functions
            private void SetSize(float size) {
                transform.localScale = size * Vector3.one;
                currentSize = size;
            }

            // Calculate Size based off slimeValue
            private float CalculateSize(float slimeValue) {
                if (usesStages) {
                    return sizeMinimum + slimeValue * sizeChangePerSlime + (slimeValue / slimePerStage) * sizeChangePerStage;
                } else {
                    return sizeMinimum + slimeValue * sizeChangePerSlime;
                }
            }

            // Calculate SizeChangeTime based off difference in sizes
            private float CalculateSizeChangeTime(float fromSize, float toSize) {
                return Mathf.Sqrt(Mathf.Abs(toSize - fromSize)) * SIZE_CHANGE_TIME;
            }

            // Change Size to current slimeCount
            private IEnumerator ChangeSize() {
                float fromSize = currentSize;
                float toSize = CalculateSize(slimeCount);
                float sizeChangeTime = CalculateSizeChangeTime(fromSize, toSize);

                yield return StartCoroutine(LerpUtils.LerpCoroutine(SetSize, fromSize, toSize * OVERFLOW_MULTIPLIER, sizeChangeTime));
                yield return StartCoroutine(LerpUtils.LerpCoroutine(SetSize, toSize * OVERFLOW_MULTIPLIER, toSize, OVERFLOW_TIME));
            }
        #endregion

        #region Stage Functions
            private void SetStage(int newStage) {
                currentStage = newStage;
            }

            // Calculate Stage based off slimeValue
            private int CalculateStage(float slimeValue) {
                return (int) Mathf.Floor(slimeValue / slimePerStage);
            }
        #endregion
    }
}