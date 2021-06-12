using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : GFXobject
{
    // Variables
    public GameObject shooter; //TODO: Make the shooter class
    public Vector2 direction;
    protected float maxDistance;
    protected int damage;
    protected Vector2 origin;

    // Components
    protected Rigidbody2D rb;

    // Prefab
    public GameObject splat;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        rb = GetComponent<Rigidbody2D>();
        //SetBaseColor(shooter.baseColor);

        origin = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(this.transform.position, origin) > maxDistance)
        {
            StartCoroutine(Splat());
        }
    }

    public void Hit()
    {
        //AudioManager.PlaySound("BulletHit");
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "Wall")
        {
            StartCoroutine(Splat());
        }
        else if(collision.collider.tag == "Enemy" || collision.collider.tag == "Player")
        {
            // Mob mob = collision.collider.GetComponent<Mob>();
            // TODO: Deal damage on hit but don't splat
            Hit();
        }
    }

    protected IEnumerator Splat()
    {
        //yield return StartCoroutine(FadeLerp(FADEOUT_TIME, 0));
        //TODO: play splat sound effect and animation
        Instantiate(splat, this.transform.position, Quaternion.identity);
        Destroy(this.gameObject);
        yield return null;
    }
}
