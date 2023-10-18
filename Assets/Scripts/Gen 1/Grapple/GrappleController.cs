using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleController : MonoBehaviour
{
    public float runForce;
    public float grappleDistance;
    public float pull;
    public float grappleSpeed;
    public float swiggleSize;
    public int swiggleNum;
    public int detail;
    public Transform hookPoint;
    public LineRenderer line;
    public LayerMask mask;

    DistanceJoint2D distanceJoint;
    Rigidbody2D rb;
    [HideInInspector]
    public bool grapple;
    float space;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        distanceJoint = GetComponent<DistanceJoint2D>();
        grapple = false;
        distanceJoint.enabled = false;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            space = 0;
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dirM = mousePos - (Vector2)transform.position;
            dirM.Normalize();
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dirM, grappleDistance, mask);
            if (grapple)
                grapple = false;
            else if (hit.collider != null && !grapple)
            {
                if (rb.velocity.x != 0)
                {
                    if (Input.GetKey(KeyCode.A))
                    {
                        rb.AddForce(Vector2.left * (runForce / 50 * rb.velocity.x));
                    }
                    else if (Input.GetKey(KeyCode.D))
                    {
                        rb.AddForce(Vector2.right * (runForce / 50 * rb.velocity.x));
                    }
                }
                else
                {
                    if (Input.GetKey(KeyCode.A))
                    {
                        rb.AddForce(Vector2.left * (runForce / 1.2f));
                    }
                    else if (Input.GetKey(KeyCode.D))
                    {
                        rb.AddForce(Vector2.right * (runForce / 1.2f));
                    }
                }

                Vector2 dir = (Vector2)transform.position - hit.point;
                float dis = dir.magnitude;
                distanceJoint.distance = dis;
                hookPoint.position = hit.point;
                grapple = true;
            }
        }

        if (Input.GetKey(KeyCode.E) && distanceJoint.distance >= 0.75f)
        {
            distanceJoint.distance -= pull * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            distanceJoint.distance += pull * Time.deltaTime;
        }

        if (grapple)
        {
            distanceJoint.enabled = true;
            grappleAnim();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                grapple = false;
                space = 0;
                Vector2 dir = transform.position - hookPoint.position;
                float dis = dir.magnitude;
                distanceJoint.distance = dis;
            }
        }
        else if (!grapple)
        {
            distanceJoint.enabled = false;
            line.positionCount = 0;
        }
    }

    void grappleAnim()
    {
        if (space < 1)
        {
            space = space + grappleSpeed * Time.deltaTime;
            Vector2 target = Vector2.Lerp(transform.position, hookPoint.position, space);
            Vector2[] oripoints = new Vector2[(swiggleNum * 2) + 1];
            float xn = (target.x - transform.position.x) / swiggleNum / 2;
            float yn = (target.y - transform.position.y) / swiggleNum / 2;
            oripoints[0] = transform.position;

            for (int i = 1; i < oripoints.Length; i++)
            {
                oripoints[i] = new Vector2(transform.position.x + (xn * i), transform.position.y + (yn * i));
            }
            bool isUp = true;
            Vector2 dir = (Vector2)transform.position - target;
            dir.Normalize();
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            hookPoint.rotation = Quaternion.Euler(0, 0, angle);
            for (int i = 1; i < oripoints.Length; i += 2)
            {
                if (isUp)
                {
                    oripoints[i] += (Vector2)hookPoint.up * swiggleSize;
                    isUp = !isUp;
                }
                else
                {
                    oripoints[i] -= (Vector2)hookPoint.up * swiggleSize;
                    isUp = !isUp;
                }
            }

            List<Vector2> points = new List<Vector2>();
            points.Clear();
            for (int i = 0; i < oripoints.Length - 2; i += 2)
            {
                for (float ratio = 0; ratio <= 1; ratio += 1.0f / detail)
                {
                    Vector2 tangent1 = Vector2.Lerp(oripoints[i], oripoints[i + 1], ratio);
                    Vector2 tangent2 = Vector2.Lerp(oripoints[i + 1], oripoints[i + 2], ratio);
                    Vector2 bezierPoint = Vector2.Lerp(tangent1, tangent2, ratio);
                    points.Add(bezierPoint);
                }
            }

            line.positionCount = points.Count;
            for (int i = 0; i < points.Count; i++)
            {
                line.SetPosition(i, points[i]);
            }
        }
        else
        {
            line.positionCount = 2;
            line.SetPosition(0, transform.position);
            line.SetPosition(1, hookPoint.position);
        }
    }
}
