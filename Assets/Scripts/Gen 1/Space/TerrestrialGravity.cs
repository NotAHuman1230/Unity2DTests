using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrestrialGravity : MonoBehaviour
{
    public int trajectoryLength;
    public Vector2 beginningAcceleration;

    float gravitationalConstant;
    Rigidbody2D rb;
    TerrestrialGravity[] bodies;
    bool start = false;
    [HideInInspector]
    public Transform biggest;

    void Start()
    {
        start = true;
        gravitationalConstant = 1f;
        rb = GetComponent<Rigidbody2D>();
        bodies = FindObjectsOfType<TerrestrialGravity>();
        rb.AddForce(beginningAcceleration * rb.mass);
    }
    private void FixedUpdate()
    {
        float biggests = 0f;
        Transform biggestRb = null;
        foreach (TerrestrialGravity body in bodies)
        {
            if(body != this && body.tag != "Ground")
            {
                Vector2 direction = body.transform.position - transform.position;
                float forceMagnitude = gravitationalConstant * ((rb.mass * body.rb.mass) / Mathf.Pow(direction.magnitude, 2f));
                rb.AddForce(direction.normalized * forceMagnitude);
                if (forceMagnitude > biggests)
                {
                    biggests = forceMagnitude;
                    biggestRb = body.transform;
                }
            }
        }
        biggest = biggestRb;
    }

    private void OnDrawGizmosSelected()
    {
        if (!start)
        {
            List<Vector2> positions = new List<Vector2>();
            gravitationalConstant = 1f;
            rb = GetComponent<Rigidbody2D>();
            bodies = FindObjectsOfType<TerrestrialGravity>();

            Vector2 velocity = beginningAcceleration * rb.mass * Time.fixedDeltaTime / rb.mass;
            Vector2 position = transform.position;
            positions.Add(position);
            for (int i = 0; i < trajectoryLength; i++)
            {
                foreach (TerrestrialGravity body in bodies)
                {
                    if (body != this)
                    {
                        Rigidbody2D otherRb = body.gameObject.GetComponent<Rigidbody2D>();
                        Vector2 direction = (Vector2)body.transform.position - position;
                        float forceMagnitude = gravitationalConstant * (rb.mass * otherRb.mass / Mathf.Pow(direction.magnitude, 2f));
                        velocity += direction.normalized * forceMagnitude * Time.fixedDeltaTime / rb.mass;
                    }
                }
                position += velocity * Time.fixedDeltaTime;
                positions.Add(position);
            }

            for (int i = 0; i < positions.Count; i++)
            {
                if (i != 0)
                    Debug.DrawLine(positions[i - 1], positions[i], Color.white);
            }
            positions.Clear();
        }
    }
}
