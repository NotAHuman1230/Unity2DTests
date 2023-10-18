using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Parameters")]
    public float gravity;
    public float mass;
    public float runAcceleration;
    public float friction;
    public float terminalX;
    public float terminalY;

    [Header("Movement")]
    public float jumpPower;

    [Header("Collsion")]
    public LayerMask wall;
    public float boundingBoxX;
    public float boundingBoxY;
    public float collisionOffset;
    public int collisionSubdivision;

    public Vector2 velocity;
    public Vector2 resultantForce;
    Vector2 gravityForce;
    public Vector2 contactForce;
    Vector2 runForce;
    Vector2 frictionForce;
    Vector2 jumpForce;

    bool onGround;

    private void Start()
    {

    }
    private void Update()
    {
        gravityForce = new Vector2(0f, -gravity * mass);
        contactForceCalculation();
        run();
        jump();
        frictionCalculation();
        velocityCalculation();
    }

    void velocityCalculation()
    {
        resultantForce = Vector2.zero;
        resultantForce += gravityForce + contactForce + runForce + frictionForce;

        Vector2 acceleration = resultantForce / mass;
        velocity += (acceleration * Time.deltaTime) + (jumpForce / mass);
        velocity = new Vector2(Mathf.Clamp(velocity.x, -terminalX, terminalX), Mathf.Clamp(velocity.y, -terminalY, terminalY));
        transform.Translate(velocity * Time.deltaTime);
    }
    void contactForceCalculation()
    {
        bool hasCollidedRight = false;
        bool hasCollidedLeft = false;
        bool hasCollidedUp = false;
        bool hasCollidedDown= false;
        for (int i = 0; i < collisionSubdivision; i++)
        {
            float offsetX = transform.position.x + Mathf.Lerp(-boundingBoxX + collisionOffset, boundingBoxX - collisionOffset, (float)i / (collisionSubdivision - 1));
            float offsetY = transform.position.y + Mathf.Lerp(-boundingBoxY + collisionOffset, boundingBoxY - collisionOffset, (float)i / (collisionSubdivision - 1));
            RaycastHit2D hitR = Physics2D.Raycast(new Vector2(transform.position.x, offsetY), Vector2.right * boundingBoxY, boundingBoxX, wall);
            RaycastHit2D hitL = Physics2D.Raycast(new Vector2(transform.position.x, offsetY), Vector2.left * boundingBoxY, boundingBoxX, wall);
            RaycastHit2D hitU = Physics2D.Raycast(new Vector2(offsetX, transform.position.y), Vector2.up * boundingBoxX, boundingBoxY, wall);
            RaycastHit2D hitD = Physics2D.Raycast(new Vector2(offsetX, transform.position.y), Vector2.down * boundingBoxX, boundingBoxY, wall);

            if (hitR.collider != null)
                hasCollidedRight = true;
            if (hitL.collider != null)
                hasCollidedLeft = true;
            if (hitU.collider != null)
                hasCollidedUp = true;
            if (hitD.collider != null)
                hasCollidedDown = true;
        }

        Vector2 withoutContact = resultantForce - contactForce;
        contactForce = Vector2.zero;
        if (hasCollidedRight && withoutContact.x > 0)
        {
            contactForce += new Vector2(-withoutContact.x, 0f);
            velocity.x = 0f;
        }
        if (hasCollidedLeft && withoutContact.x < 0)
        {
            contactForce += new Vector2(-withoutContact.x, 0f);
            velocity.x = 0f;
        }
        if (hasCollidedUp && withoutContact.y > 0)
        {
            contactForce += new Vector2(0f, -withoutContact.y);
            velocity.y = 0f;
        }
        if (hasCollidedDown)
        {
            onGround = true;
            if (withoutContact.y < 0 && jumpForce.y == 0f)
            {
                contactForce += new Vector2(0f, -withoutContact.y);
                velocity.y = 0f;
            }
        }
        else
            onGround = false;
    }
    void frictionCalculation()
    {
        if (onGround && runForce.x == 0f)
        {
            frictionForce = new Vector2(-friction * velocity.x * mass, 0f);
            if (Mathf.Abs(velocity.x) <= 0.3f)
                velocity.x = 0f;
        }
        else
            frictionForce = Vector2.zero;
    }
    void run()
    {
        float x = Input.GetAxisRaw("Horizontal") * runAcceleration;
        runForce = new Vector2(x, 0f);
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) && onGround)
            velocity = new Vector2(0f, velocity.y);
    }
    void jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && onGround)
        {
            velocity = new Vector2(velocity.x, 0f);
            jumpForce = new Vector2(0f, jumpPower);
        }
        else
            jumpForce = Vector2.zero;
    }

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < collisionSubdivision; i++)
        {
            float offsetX = transform.position.x + Mathf.Lerp(-boundingBoxX + collisionOffset, boundingBoxX - collisionOffset, (float)i / (collisionSubdivision - 1));
            float offsetY = transform.position.y + Mathf.Lerp(-boundingBoxY + collisionOffset, boundingBoxY - collisionOffset, (float)i / (collisionSubdivision - 1));

            Debug.DrawRay(new Vector2(transform.position.x, offsetY), Vector2.right * boundingBoxY, Color.blue);
            Debug.DrawRay(new Vector2(transform.position.x, offsetY), Vector2.left * boundingBoxY, Color.blue);
            Debug.DrawRay(new Vector2(offsetX, transform.position.y), Vector2.up * boundingBoxX, Color.blue);
            Debug.DrawRay(new Vector2(offsetX, transform.position.y), Vector2.down * boundingBoxX, Color.blue);
        }
    }
}
