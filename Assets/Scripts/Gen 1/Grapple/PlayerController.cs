using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float stopDis;
    public float lungeStrength;
    public float speed;
    public float jumpF;
    public int maxJumps;
    public float smoothness;
    public LayerMask mask;

    int currentJumps;
    bool canLunge;
    Rigidbody2D rb;
    DistanceJoint2D distanceJoint;
    Vector2 vel = Vector2.zero;

    private void Start()
    {
        distanceJoint = GetComponent<DistanceJoint2D>();
        rb = GetComponent<Rigidbody2D>();
        currentJumps = maxJumps;
        canLunge = true;
    }

    private void Update()
    {
        if (!distanceJoint.enabled)
        {
            float x = Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime;
            RaycastHit2D rightHit = Physics2D.Raycast(transform.position, Vector2.right, stopDis, mask);
            RaycastHit2D leftHit = Physics2D.Raycast(transform.position, Vector2.left, stopDis, mask);

            if (rb.velocity.x > 0 && Input.GetKey(KeyCode.D))
            {
                x *= 0;
            }
            else if (rb.velocity.x < 0 && Input.GetKey(KeyCode.A))
            {
                x *= 0;
            }

            if (rightHit.collider != null && x > 0)
            {
                x *= 0;
            }
            else if (leftHit.collider != null && x < 0)
            {
                x *= 0;
            }
            transform.Translate(new Vector2(x, 0));
        }
        else if(distanceJoint.enabled && canLunge)
        {
            float x = Input.GetAxisRaw("Horizontal") * lungeStrength;
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
                StartCoroutine(lunge(x));
        }

        if(rb.velocity.x > 0 && Input.GetKeyDown(KeyCode.A))
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else if (rb.velocity.x < 0 && Input.GetKeyDown(KeyCode.D))
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        if (Input.GetKeyDown(KeyCode.Space) && currentJumps >= 1)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            currentJumps--;
            rb.AddForce(Vector2.up * jumpF);
        }

        Vector2 pos = Vector2.SmoothDamp(Camera.main.transform.position, transform.position, ref vel, smoothness);
        Camera.main.transform.position = new Vector3(pos.x, pos.y, -10);

        if (distanceJoint.enabled)
        {
            currentJumps = maxJumps - 1;
        }
    }

    IEnumerator lunge(float x)
    {
        canLunge = false;
        rb.AddForce(Vector2.right * x);
        yield return new WaitForSeconds(1f);
        canLunge = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            rb.velocity = Vector2.zero;
            currentJumps = maxJumps;
        }
    }
}
