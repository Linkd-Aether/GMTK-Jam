using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Character;
using Game.Utils;


namespace Game.UI {
    public class SlimeMeter : MonoBehaviour
    {
        // Constants
        private static GameObject BAR_PREFAB;
        private static float BAR_FILL_SCALE = 6.5f;
        private static float FILL_FACTOR = 1f;

        private static float DECREASE_STAGE_EFFECT_SIZE = 0.95f;
        private static float DECREASE_STAGE_EFFECT_LENGTH = .1f;

        private static float INCREASE_STAGE_EFFECT_SIZE = 1.05f;
        private static float INCREASE_STAGE_EFFECT_LENGTH = .1f;
        
        // Variables
        private Coroutine currentCoroutine;
        private float currentFill;
        
        // Components & References
        private Transform barHolder;
        private SpriteRenderer[] bars;

        
        private void Awake() {
            BAR_PREFAB = Resources.Load<GameObject>("Prefabs/UI/Bar");

            foreach (Transform child in transform) {
                if (child.name == "Bars") {
                    barHolder = child;
                }
            }
        }

        // Initialize the meter according to the state of SlimeHealth
        public void InitializeMeter(SlimeHealth slimeHealth) {
            bars = new SpriteRenderer[slimeHealth.colorPerStage.Length-1];

            for (int i = 0; i < bars.Length; i++) {
                GameObject barObj = Instantiate(BAR_PREFAB);
                barObj.transform.parent = barHolder;
                barObj.transform.localPosition = Vector3.zero;
                barObj.transform.localScale = Vector3.zero;

                SpriteRenderer bar = barObj.GetComponentInChildren<SpriteRenderer>();
                bar.color = slimeHealth.colorPerStage[i+1];
                bar.sortingLayerName = "UI";
                bar.sortingOrder = i;
                
                bars[i] = bar;
            }

            float targetFill = CalculateFillTarget(slimeHealth);
            SetFill(targetFill);
        }

        // Update the meter according to the state of SlimeHealth
        public void UpdateMeter(SlimeHealth slimeHealth) {
            float targetFill = CalculateFillTarget(slimeHealth);
            float timeToFill = CalculateFillTime(currentFill, targetFill);

            if (currentCoroutine != null) StopCoroutine(currentCoroutine);
            currentCoroutine = StartCoroutine(LerpUtils.LerpCoroutine(SetFill, currentFill, targetFill, timeToFill));
        }

        #region Calculate Values
            // Calculate target meter filling from state of SlimeHealth
            private float CalculateFillTarget(SlimeHealth slimeHealth) {
                return slimeHealth.slimeCount / slimeHealth.slimePerStage;
            }

            // Calculate time to fill from change
            private float CalculateFillTime(float fromFill, float toFill) {
                return Mathf.Abs(toFill - fromFill) * FILL_FACTOR;
            }
        #endregion

        #region Update Bars
            private void SetFill(float fill) {
                if (Mathf.Floor(currentFill) > Mathf.Floor(fill)) {
                    // Decreased a stage
                    StartCoroutine(DecreaseStageEffect((int) Mathf.Floor(currentFill)));
                } else if (Mathf.Floor(currentFill) < Mathf.Floor(fill)) {
                    // Increased a stage
                    StartCoroutine(IncreaseStageEffect((int) Mathf.Floor(fill)));
                }
                
                SetMeterToFill(fill);
                currentFill = fill;
            }

            private void SetMeterToFill(float fill) {
                int meter = 0;
                while (fill > 0) {
                    float fillPercent = Mathf.Clamp(fill, 0, 1);
                    SetBarToFill(meter, fillPercent);
                    fill -= fillPercent;
                    meter++;
                }
                for (int i = meter; i < bars.Length; i++) {
                    SetBarToFill(i, 0);
                }
            }

            private void SetBarToFill(int meter, float fillPercent) {
                bars[meter].transform.parent.localScale = new Vector3(fillPercent * BAR_FILL_SCALE, 1, 1);

            }
        #endregion

        #region Effects
            // Bar pulse brighter and larger
            private IEnumerator IncreaseStageEffect(int increaseToStage) {
                LerpUtils.LerpDelegate effect = (lerpTime) => {
                    SetMeterSize(1, INCREASE_STAGE_EFFECT_SIZE, lerpTime);
                };

                yield return StartCoroutine(LerpUtils.LerpCoroutine(effect, 0, 1, INCREASE_STAGE_EFFECT_LENGTH));
                yield return StartCoroutine(LerpUtils.LerpCoroutine(effect, 1, 0, INCREASE_STAGE_EFFECT_LENGTH / 4));
            }

            // Bar pulse shrivels and darker
            private IEnumerator DecreaseStageEffect(int decreaseFromStage) {
                LerpUtils.LerpDelegate effect = (lerpTime) => {
                    SetMeterSize(1, DECREASE_STAGE_EFFECT_SIZE, lerpTime);
                };

                yield return StartCoroutine(LerpUtils.LerpCoroutine(effect, 0, 1, DECREASE_STAGE_EFFECT_LENGTH));
                yield return StartCoroutine(LerpUtils.LerpCoroutine(effect, 1, 0, DECREASE_STAGE_EFFECT_LENGTH / 4));
            }

            private void SetMeterSize(float startSize, float endSize, float percent) {
                float size = Mathf.Lerp(startSize, endSize, percent);
                transform.localScale = Vector3.one * size;
            }

            private void SetBarColor(float percent, Color targColor, SpriteRenderer spriteRenderer) {

            }
        #endregion
    }
}