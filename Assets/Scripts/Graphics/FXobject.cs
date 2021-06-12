using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FXobject : GFXobject
{
    static private GameObject FXLayer;

    static protected Color BLACK = new Color(0,0,0,0);
    static protected Color RED = new Color(165/255f,0,0,0);
    static protected Color WHITE = new Color(1,1,1,0);

    static private float FX_RADIUS = .53f;


    protected override void Start() {
        base.Start();
        FXLayer = (GameObject) Resources.Load("Prefabs/Mobs/Mobs/FXLayer");
    }


    private void SetAlphaFX(SpriteRenderer FXRenderer, float alpha) {
        Color color = FXRenderer.color;
        color.a = alpha;
        FXRenderer.color = color;
    }

    protected SpriteRenderer CreateFXLayer(Color color) {
        GameObject FXLayerObj = Instantiate(FXLayer);
        FXLayerObj.transform.parent = this.transform;
        FXLayerObj.transform.localPosition = Vector3.zero;
        FXLayerObj.transform.localScale = Vector3.one * FX_RADIUS;

        SpriteRenderer FXRenderer = FXLayerObj.GetComponent<SpriteRenderer>();
        FXRenderer.color = color;
        SetAlphaFX(FXRenderer, 0);

        return FXRenderer;
    }

    #region FX Tools
        protected IEnumerator ColorFlash(Color color, float fadeTime, float targetAlpha) {
            SpriteRenderer FXRenderer = CreateFXLayer(color);

            yield return StartCoroutine(AlphaLerpFX(FXRenderer, fadeTime, targetAlpha));
            yield return new WaitForEndOfFrame();
            yield return StartCoroutine(AlphaLerpFX(FXRenderer, fadeTime, 0));
            Destroy(FXRenderer.gameObject);
        }

        protected IEnumerator AlphaLerpFX(SpriteRenderer FXRenderer, float fadeTime, float targetAlpha) {
            float startingAlpha = FXRenderer.color.a;
            float lerpT = 0;

            while (lerpT < 1) {
                lerpT += Time.deltaTime / fadeTime;
                lerpT = Mathf.Clamp(lerpT, 0, 1);
                float alpha = Mathf.Lerp(startingAlpha, targetAlpha, lerpT);
                SetAlphaFX(FXRenderer, alpha);
                yield return new WaitForEndOfFrame();
            }
        }
    #endregion
}
