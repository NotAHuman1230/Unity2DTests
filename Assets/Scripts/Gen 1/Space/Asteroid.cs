using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public Transform sun;
    public float minMagnitude;
    public float maxMagnitude;

    Rigidbody2D rb;
    Collider2D col;

    private void Start()
    {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        Vector2 direction = (sun.position - transform.position).normalized;
        rb.AddForce(-Vector2.Perpendicular(direction) * Random.Range(minMagnitude,maxMagnitude) * rb.mass);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
            Physics2D.IgnoreCollision(col, collision.collider);
    }
}
