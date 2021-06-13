using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class GFXobject : MonoBehaviour
{
    protected SpriteRenderer spriteRenderer;
    protected Color baseColor;


    protected virtual void Awake() {
        LoadComponents();
    }

    protected virtual void Start() {
        baseColor = spriteRenderer.color;
    }

    public void SetBaseColor(Color color) {
        baseColor = color;
        SetColor(color);
    }

    public void SetColor(Color color) {
        spriteRenderer.color = color;
    }

    public void SetAlpha(float alpha) {
        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }

    public void SetSprite(Sprite sprite) {
        spriteRenderer.sprite = sprite;
    }

    public virtual void LoadComponents() {
        foreach (Transform child in transform) {
            if (child.tag == "GFX") {
                spriteRenderer = child.GetComponent<SpriteRenderer>();
            }
        }
    }
}
