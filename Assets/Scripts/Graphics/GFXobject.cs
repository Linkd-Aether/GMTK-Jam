using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Utils;


public abstract class GFXobject : MonoBehaviour
{
    protected SpriteRenderer spriteRenderer;
    protected Color baseColor;

    private static float FREE_ANGLE_VAR = 10f;
    private static int MAX_CHECKS_FOR_FREE_DIR = 3;


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

    // THIS SHOULD NOT BE HERE BUT I NEEDED ACCESS TO IT IN TWO CHILDREN !!!
    protected static Vector2 FindFreeDirection(Vector2 origin, Vector2 dir, float checkDistance) {
        float angleVar = Random.Range(-FREE_ANGLE_VAR, FREE_ANGLE_VAR);
        float checkAngle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + angleVar) % 360;
        int checks = 0;

        while (checks < MAX_CHECKS_FOR_FREE_DIR) {
            Vector2 checkDir = OtherUtils.AngleToDirection(checkAngle);

            if (!Physics2D.Raycast((Vector2) origin - checkDir, checkDir, checkDistance)) {
                return checkDir;
            }

            checkAngle = (checkAngle - angleVar) % 360;
            checks++;
        } 
        return Vector2.zero;
    }
}
