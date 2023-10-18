using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PendulumController : MonoBehaviour
{
    [Header("Parameters")]
    public LayerMask wall;
    public LineRenderer lineRenderer;
    public Transform pivot;
    public float gravity;
    public float friction;

    [Header("Forces")]
    public bool velocityBlue;
    public bool accelerationRed;
    public bool tentionGreen;
    public bool centrifigulWhite;
    public bool frictionBlack;

    Vector2 tensionAcceleration;
    Vector2 centrifigulAcceleration;
    Vector2 velocity;

    float radius;
    List<Vector2> pivots = new List<Vector2>();

    private void Start()
    {
        pivots.Add(pivot.position);
        Vector2 dir = pivot.position - transform.position;
        radius = dir.magnitude;
    }
    private void Update()
    {
        lineSet();

        pendulumForces();
        stringCollide();
    }

    void pendulumForces()
    {
        Vector2 direction = pivots[pivots.Count-1] - (Vector2)transform.position;

        float theta = Mathf.Atan2(direction.x, direction.y);
        float magnitude = gravity * Mathf.Cos(theta);
        tensionAcceleration = direction.normalized * magnitude;

        float centrifigulMagnitude = Mathf.Pow(velocity.magnitude, 2f) / radius;
        centrifigulAcceleration = direction.normalized * centrifigulMagnitude;

        Vector2 acceleration = new Vector2(0f, -gravity) + tensionAcceleration + centrifigulAcceleration + (-velocity * friction);
        velocity += acceleration * Time.deltaTime;
        if (Mathf.Abs(velocity.x) < 0.05f && Mathf.Abs(velocity.y) < 0.05f && Mathf.Abs(theta * Mathf.Rad2Deg) < 0.01f)
            velocity = Vector2.zero;
        else
            transform.Translate(velocity * Time.deltaTime);

        Vector2 dir = (Vector2)transform.position - pivots[pivots.Count - 1];
        transform.position = pivots[pivots.Count - 1] + (dir.normalized * radius);

        if (accelerationRed)
            Debug.DrawRay(transform.position, acceleration, Color.red);
        if (velocityBlue)
            Debug.DrawRay(transform.position, velocity, Color.blue);
        if (tentionGreen)
            Debug.DrawRay(transform.position, tensionAcceleration, Color.green);
        if (centrifigulWhite)
            Debug.DrawRay(transform.position, centrifigulAcceleration, Color.white);
        if (frictionBlack)
            Debug.DrawRay(transform.position, -velocity * friction, Color.black);
    }
    void stringCollide()
    {
        Vector2 direction = (Vector2)transform.position - pivots[pivots.Count - 1];
        float offsetMagnitusde = 0.1f;

        if (pivots.Count != 1)
        {
            float theta1 = Mathf.Atan2(pivots[pivots.Count - 1].y - pivots[pivots.Count - 2].y, pivots[pivots.Count - 1].x - pivots[pivots.Count - 2].x);
            float theta2 = Mathf.Atan2(transform.position.y - pivots[pivots.Count - 2].y, transform.position.x - pivots[pivots.Count - 2].x);
            if ((theta1 * Mathf.Rad2Deg) - 180f > (theta2 * Mathf.Rad2Deg) - 180f)
            {
                Vector2 dir = (Vector2)transform.position - pivots[pivots.Count - 2];
                Vector2 tweak = dir.normalized * offsetMagnitusde;
                RaycastHit2D hit1 = Physics2D.Raycast(pivots[pivots.Count - 2] + tweak, dir.normalized, dir.magnitude - tweak.magnitude, wall);
                if (hit1.collider == null)
                {
                    pivots.RemoveAt(pivots.Count - 1);
                    Vector2 d = pivots[pivots.Count - 1] - (Vector2)transform.position;
                    radius = d.magnitude;
                }
            }   
        }

        Vector2 offset = direction.normalized * offsetMagnitusde;
        RaycastHit2D hit2 = Physics2D.Raycast(pivots[pivots.Count - 1] + offset, direction.normalized, direction.magnitude - offset.magnitude, wall);
        if (hit2.collider != null)
        {
            pivots.Add(hit2.point);
            Vector2 dir = pivots[pivots.Count - 1] - (Vector2)transform.position;
            radius = dir.magnitude;
        }
    }
    void lineSet()
    {
        lineRenderer.positionCount = pivots.Count + 1;
        for (int i = 0; i < lineRenderer.positionCount-1; i++)
        {
            lineRenderer.SetPosition(i, pivots[i]);
        }
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, transform.position);
    }
}
