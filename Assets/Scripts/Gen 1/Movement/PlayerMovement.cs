using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float runAcceleration;
    public float friction;
    public float airControl;
    public float airResistance;
    public float terminalX;
    public float terminalY;

    [Header("Jumping")]
    public float airTime;
    public float maxHeight;
    public float fallMultiplyer;

    [Header("Collision")]
    public LayerMask wall;
    public float boundingBoxX;
    public float boundingBoxY;
    public float collisionOffset;
    public int collisionSubdivision;

    Vector2 acceleration;
    Vector2 velocity;

    float gravity;
    float jumpingVelocity;

    bool onground;

    private void Update()
    {
        gravity = (8f * maxHeight) / Mathf.Pow(airTime, 2f);
        jumpingVelocity = gravity * (airTime / 2f);

        if (velocity.y >= 0f)
            acceleration = new Vector2(0f, -gravity);
        else
            acceleration = new Vector2(0f, -gravity * fallMultiplyer);

        run();
        jump();
        collisions();

        velocity += acceleration * Time.deltaTime;
        velocity = new Vector2(Mathf.Clamp(velocity.x, -terminalX, terminalX), Mathf.Clamp(velocity.y, -terminalY, terminalY));
        transform.Translate(velocity * Time.deltaTime);
        acceleration = Vector2.zero;
    }

    void collisions()
    {
        bool hasCollidedRight = false;
        bool hasCollidedLeft = false;
        bool hasCollidedUp = false;
        bool hasCollidedDown = false;

        float objectRight = 0f;
        float objectLeft = 0f;
        float objectUp = 0f;
        float objectDown = 0f;

        int rightNum = 0;
        int leftNum = 0;
        int upNum = 0;
        int downNum = 0;

        for (int i = 0; i < collisionSubdivision; i++)
        {
            float offsetX = transform.position.x + Mathf.Lerp(-boundingBoxX + collisionOffset, boundingBoxX - collisionOffset, (float)i / (collisionSubdivision - 1));
            float offsetY = transform.position.y + Mathf.Lerp(-boundingBoxY + collisionOffset, boundingBoxY - collisionOffset, (float)i / (collisionSubdivision - 1));
            RaycastHit2D hitR = Physics2D.Raycast(new Vector2(transform.position.x, offsetY), Vector2.right * boundingBoxY, boundingBoxX, wall);
            RaycastHit2D hitL = Physics2D.Raycast(new Vector2(transform.position.x, offsetY), Vector2.left * boundingBoxY, boundingBoxX, wall);
            RaycastHit2D hitU = Physics2D.Raycast(new Vector2(offsetX, transform.position.y), Vector2.up * boundingBoxX, boundingBoxY, wall);
            RaycastHit2D hitD = Physics2D.Raycast(new Vector2(offsetX, transform.position.y), Vector2.down * boundingBoxX, boundingBoxY, wall);

            if (hitR.collider != null)
            {
                hasCollidedRight = true;
                objectRight = hitR.transform.position.x - (hitR.transform.lossyScale.x / 2f) - (transform.lossyScale.x / 2f);
                rightNum++;
            }
            if (hitL.collider != null)
            {
                hasCollidedLeft = true;
                objectLeft = hitL.transform.position.x + (hitL.transform.lossyScale.x / 2f) + (transform.lossyScale.x / 2f);
                leftNum++;
            }
            if (hitU.collider != null)
            {
                hasCollidedUp = true;
                objectUp = hitU.transform.position.y - (hitU.transform.lossyScale.y / 2f) - (transform.lossyScale.y / 2f);
                upNum++;
            }
            if (hitD.collider != null)
            {
                hasCollidedDown = true;
                objectDown = hitD.transform.position.y + (hitD.transform.lossyScale.y / 2f) + (transform.lossyScale.y / 2f);
                downNum++;
            }
        }

        int biggest = 0;
        if (rightNum > biggest)
            biggest = rightNum;
        if (leftNum > biggest)
            biggest = leftNum;
        if (upNum > biggest)
            biggest = upNum;
        if (downNum > biggest)
            biggest = downNum;

        if(hasCollidedRight && (velocity.x > 0f || acceleration.x > 0f))
        {
            if(rightNum >= biggest)
                transform.position = new Vector2(objectRight, transform.position.y);
            velocity = new Vector2(0f, velocity.y);
            if (acceleration.x > 0f)
                acceleration = new Vector2(0f, acceleration.y);
        }
        if (hasCollidedLeft && (velocity.x < 0f || acceleration.x < 0f))
        {
            if(leftNum >= biggest)
                transform.position = new Vector2(objectLeft, transform.position.y);
            velocity = new Vector2(0f, velocity.y);
            if (acceleration.x < 0f)
                acceleration = new Vector2(0f, acceleration.y);
        }
        if (hasCollidedDown && (velocity.y < 0f || acceleration.y < 0f))
        {
            if(downNum >= biggest)
                transform.position = new Vector2(transform.position.x, objectDown);
            velocity = new Vector2(velocity.x, 0f);
            if (acceleration.y < 0f)
                acceleration = new Vector2(acceleration.x, 0f);
        }
        if (hasCollidedUp && (velocity.y > 0f || acceleration.y > 0f))
        {
            if(upNum >= biggest)
                transform.position = new Vector2(transform.position.x, objectUp);
            velocity = new Vector2(velocity.x, 0f);
            if (acceleration.y > 0f)
                acceleration = new Vector2(acceleration.x, 0f);
        }
        if (hasCollidedDown)
            onground = true;
        else
            onground = false;
    }
    void run()
    {
        float x = Input.GetAxisRaw("Horizontal");
        if (x != 0f)
        {
            velocity = velocity.x > 0f && x > 0f || velocity.x < 0f && x < 0f ? velocity : new Vector2(0f, velocity.y);
            if(onground)
                acceleration = new Vector2(x * runAcceleration, acceleration.y);
            else
                acceleration = new Vector2(x * airControl, acceleration.y);
        }
        else if (Mathf.Abs(velocity.x) > 0.3f)
        {
            if(onground)
                acceleration = new Vector2(-velocity.x * friction, acceleration.y);
            else
                acceleration = new Vector2(-velocity.x * airResistance, acceleration.y);
        }
        else
            velocity = new Vector2(0f, velocity.y);
    }
    void jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && onground)
        {
            velocity = new Vector2(velocity.x, jumpingVelocity);
            acceleration = new Vector2(velocity.x, 0f);
        }
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
